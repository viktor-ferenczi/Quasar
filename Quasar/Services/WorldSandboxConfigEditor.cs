using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Quasar.Models;

namespace Quasar.Services;

public static class WorldSandboxConfigEditor
{
    public const string SandboxConfigFileName = "Sandbox_config.sbc";

    public static IReadOnlyList<QuasarModSelection> ReadMods(string sandboxConfigPath)
    {
        if (!File.Exists(sandboxConfigPath))
            return Array.Empty<QuasarModSelection>();

        XDocument document;
        try
        {
            document = XDocument.Load(sandboxConfigPath, LoadOptions.PreserveWhitespace);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Failed to parse '{sandboxConfigPath}'.", exception);
        }

        var modsElement = document.Root?.Element("Mods");
        if (modsElement is null)
            return Array.Empty<QuasarModSelection>();

        var results = new List<QuasarModSelection>();
        var seen = new HashSet<long>();
        foreach (var item in modsElement.Elements("ModItem"))
        {
            var idText = (item.Element("PublishedFileId")?.Value ?? string.Empty).Trim();
            if (!long.TryParse(idText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var workshopId) || workshopId <= 0)
                continue;

            if (!seen.Add(workshopId))
                continue;

            var friendlyName = item.Attribute("FriendlyName")?.Value?.Trim();
            if (string.IsNullOrWhiteSpace(friendlyName))
                friendlyName = workshopId.ToString(CultureInfo.InvariantCulture);

            results.Add(new QuasarModSelection
            {
                WorkshopId = workshopId,
                DisplayName = friendlyName,
            });
        }

        return results;
    }

    public static async Task WriteModsAsync(
        string sandboxConfigPath,
        IReadOnlyList<QuasarModSelection> mods,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(sandboxConfigPath))
            throw new FileNotFoundException(
                $"{SandboxConfigFileName} not found at '{sandboxConfigPath}'.",
                sandboxConfigPath);

        XDocument document;
        try
        {
            document = XDocument.Load(sandboxConfigPath, LoadOptions.PreserveWhitespace);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Failed to parse '{sandboxConfigPath}'.", exception);
        }

        var root = document.Root
            ?? throw new InvalidOperationException($"'{sandboxConfigPath}' has no root element.");

        var modsElement = root.Element("Mods");
        if (modsElement is null)
        {
            modsElement = new XElement("Mods");
            var settings = root.Element("Settings");
            if (settings is not null)
                settings.AddAfterSelf(modsElement);
            else
                root.Add(modsElement);
        }
        else
        {
            modsElement.RemoveNodes();
        }

        foreach (var mod in mods)
        {
            if (mod.WorkshopId <= 0)
                continue;

            var friendlyName = string.IsNullOrWhiteSpace(mod.DisplayName)
                ? mod.WorkshopId.ToString(CultureInfo.InvariantCulture)
                : mod.DisplayName.Trim();

            var idString = mod.WorkshopId.ToString(CultureInfo.InvariantCulture);
            modsElement.Add(new XElement(
                "ModItem",
                new XAttribute("FriendlyName", friendlyName),
                new XElement("Name", $"{idString}.sbm"),
                new XElement("PublishedFileId", idString),
                new XElement("PublishedServiceName", "Steam")));
        }

        var content = SerializeXml(document);
        await AtomicFileWriter.WriteTextAsync(sandboxConfigPath, content, cancellationToken);
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

    private sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    }
}
