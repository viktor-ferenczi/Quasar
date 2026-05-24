using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Quasar.Models;

namespace Quasar.Services;

public sealed class DedicatedServerRuntimePreparer
{
    private static readonly Regex IgnoreLastSessionPattern = new(@"(?<!\S)-ignorelastsession(?!\S)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
    private static readonly Regex ConsolePattern = new(@"(?<!\S)-console(?!\S)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
    private static readonly Regex NoConsolePattern = new(@"(?<!\S)-noconsole(?!\S)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
    private static readonly Regex PathPattern = new(@"(?<!\S)-path(?!\S)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
    private static readonly Regex NoSplashPattern = new(@"(?<!\S)-nosplash(?!\S)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
    private static readonly XNamespace XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";
    private static readonly XNamespace XsdNamespace = "http://www.w3.org/2001/XMLSchema";

    private readonly ILogger<DedicatedServerRuntimePreparer> _logger;
    private readonly WebServiceOptions _options;

    public DedicatedServerRuntimePreparer(
        ILogger<DedicatedServerRuntimePreparer> logger,
        WebServiceOptions options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task<PreparedDedicatedServerLaunch> PrepareAsync(
        DedicatedServerInstanceDefinition definition,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var dedicatedServerAppDataPath = RequirePath(definition.DedicatedServerAppDataPath, "DedicatedServerAppDataPath");
        var magnetarAppDataPath = RequirePath(definition.MagnetarAppDataPath, "MagnetarAppDataPath");
        var worldPath = ResolveWorldPath(definition.WorldPath);
        var runtimeConfigPath = Path.Combine(dedicatedServerAppDataPath, "SpaceEngineers-Dedicated.cfg");
        var lastSessionPath = Path.Combine(dedicatedServerAppDataPath, "Saves", "LastSession.sbl");

        Directory.CreateDirectory(dedicatedServerAppDataPath);
        Directory.CreateDirectory(magnetarAppDataPath);
        Directory.CreateDirectory(Path.Combine(dedicatedServerAppDataPath, "Saves"));

        await PrepareRuntimeConfigAsync(definition, runtimeConfigPath, cancellationToken);
        await WriteLastSessionAsync(definition, worldPath, dedicatedServerAppDataPath, lastSessionPath, cancellationToken);

        var arguments = BuildLaunchArguments(
            definition,
            dedicatedServerAppDataPath,
            magnetarAppDataPath,
            worldPath,
            runtimeConfigPath,
            _options);

        return new PreparedDedicatedServerLaunch(
            dedicatedServerAppDataPath,
            magnetarAppDataPath,
            worldPath,
            runtimeConfigPath,
            lastSessionPath,
            arguments);
    }

    private async Task PrepareRuntimeConfigAsync(
        DedicatedServerInstanceDefinition definition,
        string runtimeConfigPath,
        CancellationToken cancellationToken)
    {
        var configuredSourcePath = string.IsNullOrWhiteSpace(definition.ConfigFilePath)
            ? runtimeConfigPath
            : definition.ConfigFilePath.Trim();

        var sourcePath = File.Exists(configuredSourcePath)
            ? configuredSourcePath
            : File.Exists(runtimeConfigPath)
                ? runtimeConfigPath
                : null;

        if (string.IsNullOrWhiteSpace(sourcePath))
            return;

        XDocument document;
        try
        {
            document = XDocument.Load(sourcePath, LoadOptions.PreserveWhitespace);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Failed to load DS config '{sourcePath}'.", exception);
        }

        var root = document.Root ?? throw new InvalidOperationException($"DS config '{sourcePath}' has no root element.");
        UpsertElement(root, "IgnoreLastSession", "false");

        var content = SerializeXml(document);
        await AtomicFileWriter.WriteTextAsync(runtimeConfigPath, content, cancellationToken);

        _logger.LogInformation("Prepared runtime DS config for instance {InstanceId} at {Path}.", definition.InstanceId, runtimeConfigPath);
    }

    private async Task WriteLastSessionAsync(
        DedicatedServerInstanceDefinition definition,
        string worldPath,
        string dedicatedServerAppDataPath,
        string lastSessionPath,
        CancellationToken cancellationToken)
    {
        var savesPath = Path.Combine(dedicatedServerAppDataPath, "Saves");
        var relativePath = TryGetRelativePath(savesPath, worldPath);
        var gameName = GetWorldDisplayName(definition, worldPath);

        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(
                "MyObjectBuilder_LastSession",
                new XAttribute(XNamespace.Xmlns + "xsi", XsiNamespace),
                new XAttribute(XNamespace.Xmlns + "xsd", XsdNamespace),
                new XElement("Path", worldPath),
                new XElement("IsContentWorlds", "false"),
                new XElement("IsOnline", "false"),
                new XElement("IsLobby", "false"),
                new XElement("GameName", gameName),
                string.IsNullOrWhiteSpace(relativePath) ? null : new XElement("RelativePath", relativePath)));

        await AtomicFileWriter.WriteTextAsync(lastSessionPath, SerializeXml(document), cancellationToken);
        _logger.LogInformation("Prepared LastSession.sbl for instance {InstanceId} at {Path}.", definition.InstanceId, lastSessionPath);
    }

    private static string BuildLaunchArguments(
        DedicatedServerInstanceDefinition definition,
        string dedicatedServerAppDataPath,
        string magnetarAppDataPath,
        string worldPath,
        string runtimeConfigPath,
        WebServiceOptions options)
    {
        var baseArguments = ExpandLaunchArguments(
            definition,
            dedicatedServerAppDataPath,
            magnetarAppDataPath,
            worldPath,
            runtimeConfigPath,
            options);
        if (IgnoreLastSessionPattern.IsMatch(baseArguments))
            throw new InvalidOperationException("Launch arguments cannot include -ignorelastsession for Quasar-managed instances.");

        var additions = new List<string>();
        if (!ConsolePattern.IsMatch(baseArguments) && !NoConsolePattern.IsMatch(baseArguments))
            additions.Add("-noconsole");

        if (!PathPattern.IsMatch(baseArguments))
            additions.Add($"-path {QuoteArgument(dedicatedServerAppDataPath)}");

        if (!NoSplashPattern.IsMatch(baseArguments))
            additions.Add("-nosplash");

        if (additions.Count == 0)
            return baseArguments;

        if (string.IsNullOrWhiteSpace(baseArguments))
            return string.Join(" ", additions);

        return $"{baseArguments} {string.Join(" ", additions)}";
    }

    private static string ExpandLaunchArguments(
        DedicatedServerInstanceDefinition definition,
        string dedicatedServerAppDataPath,
        string magnetarAppDataPath,
        string worldPath,
        string runtimeConfigPath,
        WebServiceOptions options)
    {
        return (definition.LaunchArguments ?? string.Empty)
            .Trim()
            .Replace("{instanceId}", definition.InstanceId, StringComparison.OrdinalIgnoreCase)
            .Replace("{configPath}", QuoteArgument(runtimeConfigPath), StringComparison.OrdinalIgnoreCase)
            .Replace("{quasarBaseUrl}", QuoteArgument(options.BaseUrl), StringComparison.OrdinalIgnoreCase)
            .Replace("{nodeId}", options.NodeId, StringComparison.OrdinalIgnoreCase)
            .Replace("{dsAppDataPath}", QuoteArgument(dedicatedServerAppDataPath), StringComparison.OrdinalIgnoreCase)
            .Replace("{magnetarAppDataPath}", QuoteArgument(magnetarAppDataPath), StringComparison.OrdinalIgnoreCase)
            .Replace("{worldPath}", QuoteArgument(worldPath), StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolveWorldPath(string worldPath)
    {
        var path = RequirePath(worldPath, "WorldPath");
        if (File.Exists(path))
        {
            if (string.Equals(Path.GetFileName(path), "Sandbox.sbc", StringComparison.OrdinalIgnoreCase))
                path = Path.GetDirectoryName(path) ?? throw new InvalidOperationException($"Invalid world path '{worldPath}'.");
            else
                throw new InvalidOperationException($"World path '{worldPath}' points to a file instead of a world directory.");
        }

        if (!Directory.Exists(path))
            throw new InvalidOperationException($"World directory not found: {path}");

        var checkpointPath = Path.Combine(path, "Sandbox.sbc");
        if (!File.Exists(checkpointPath))
            throw new InvalidOperationException($"World directory '{path}' does not contain Sandbox.sbc.");

        return path;
    }

    private static string RequirePath(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"{name} is not configured.");

        return value.Trim();
    }

    private static string GetWorldDisplayName(DedicatedServerInstanceDefinition definition, string worldPath)
    {
        if (!string.IsNullOrWhiteSpace(definition.Name))
            return definition.Name.Trim();

        return Path.GetFileName(worldPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }

    private static string? TryGetRelativePath(string rootPath, string fullPath)
    {
        try
        {
            var relativePath = Path.GetRelativePath(rootPath, fullPath);
            if (relativePath.StartsWith("..", StringComparison.Ordinal))
                return null;

            return relativePath;
        }
        catch
        {
            return null;
        }
    }

    private static string SerializeXml(XDocument document)
    {
        using var stringWriter = new Utf8StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
        {
            Indent = true,
            Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            OmitXmlDeclaration = false,
            NewLineChars = "\n",
            NewLineHandling = NewLineHandling.Replace,
        });

        document.Save(xmlWriter);
        xmlWriter.Flush();
        return stringWriter.ToString();
    }

    private static void UpsertElement(XElement root, string name, string value)
    {
        var element = root.Element(name);
        if (element is null)
        {
            root.Add(new XElement(name, value));
            return;
        }

        element.Value = value;
    }

    private static string QuoteArgument(string value) => $"\"{value.Replace("\"", "\\\"", StringComparison.Ordinal)}\"";

    private sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    }
}

public sealed record PreparedDedicatedServerLaunch(
    string DedicatedServerAppDataPath,
    string MagnetarAppDataPath,
    string WorldPath,
    string RuntimeConfigPath,
    string LastSessionPath,
    string Arguments);
