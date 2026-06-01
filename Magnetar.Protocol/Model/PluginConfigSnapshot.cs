using System.Collections.Generic;

namespace Magnetar.Protocol.Model;

/// <summary>
/// The set of configurable plugins reported by a single agent. Keyed by
/// <see cref="AgentId"/> on the Quasar side so editors can be matched to the
/// server they came from.
/// </summary>
public class PluginConfigSnapshot
{
    public string AgentId { get; set; } = string.Empty;

    public List<PluginConfigData> Plugins { get; set; } = new();
}
