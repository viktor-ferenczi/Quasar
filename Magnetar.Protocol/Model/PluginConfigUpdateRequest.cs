namespace Magnetar.Protocol.Model;

/// <summary>
/// A request from Quasar to apply new configuration values to a plugin.
/// <see cref="ValuesJson"/> is a complete values document (a full
/// <c>SaveJson</c> envelope or a flat values object) — the agent passes it to
/// the plugin's <see cref="Bridge.IQuasarConfigProvider.ApplyConfigJson"/>.
/// </summary>
public class PluginConfigUpdateRequest
{
    public string PluginId { get; set; } = string.Empty;

    public string ValuesJson { get; set; } = string.Empty;
}
