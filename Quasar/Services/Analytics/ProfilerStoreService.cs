using System.Collections.Concurrent;
using Magnetar.Protocol.Model;

namespace Quasar.Services.Analytics;

public sealed class ProfilerStoreService
{
    private const int MaxSamplesPerServer = 12 * 60;
    private const int MaxTopEntries = 50;

    private readonly ConcurrentDictionary<string, ConcurrentQueue<ProfilerSnapshot>> _samples = new(StringComparer.OrdinalIgnoreCase);

    public void Enqueue(string uniqueName, ProfilerSnapshot snapshot)
    {
        if (string.IsNullOrWhiteSpace(uniqueName) || !ProfilerSnapshotValidator.TryNormalize(snapshot, out var normalized))
            return;

        var queue = _samples.GetOrAdd(uniqueName, _ => new ConcurrentQueue<ProfilerSnapshot>());
        queue.Enqueue(normalized);
        while (queue.Count > MaxSamplesPerServer)
            queue.TryDequeue(out _);
    }

    public ProfilerSeriesResponse Build(long fromUnix, long toUnix, IReadOnlyList<string> servers)
    {
        if (toUnix <= fromUnix || servers.Count == 0)
            return new ProfilerSeriesResponse(fromUnix, toUnix, []);

        var from = DateTimeOffset.FromUnixTimeSeconds(fromUnix);
        var to = DateTimeOffset.FromUnixTimeSeconds(toUnix);
        var result = new List<ProfilerServerSeries>();

        foreach (var uniqueName in servers.Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!_samples.TryGetValue(uniqueName, out var queue))
                continue;

            var samples = queue
                .Where(sample => sample.CapturedAtUtc >= from && sample.CapturedAtUtc <= to)
                .OrderBy(sample => sample.CapturedAtUtc)
                .ToList();
            if (samples.Count == 0)
                continue;

            result.Add(new ProfilerServerSeries(uniqueName, samples));
        }

        return new ProfilerSeriesResponse(fromUnix, toUnix, result);
    }

    internal static int ClampTopCount(int count) => Math.Clamp(count, 0, MaxTopEntries);
}

public sealed record ProfilerSeriesResponse(long From, long To, IReadOnlyList<ProfilerServerSeries> Servers);

public sealed record ProfilerServerSeries(string UniqueName, IReadOnlyList<ProfilerSnapshot> Samples);
