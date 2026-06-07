namespace Quasar.Services.Analytics;

public readonly struct MetricSample
{
    public readonly long TimestampUnixSeconds;
    public readonly float SimSpeed;
    public readonly float CpuPercent;
    public readonly float MemoryMb;
    public readonly float FrameTimeMs;
    public readonly int PlayersOnline;
    public readonly int UsedPcu;
    public readonly int ActiveGridCount;
    public readonly int ActiveEntityCount;
    public readonly int TotalBlockCount;
    public readonly int FloatingObjectCount;

    public MetricSample(
        long timestampUnixSeconds,
        float simSpeed,
        float cpuPercent,
        float memoryMb,
        float frameTimeMs,
        int playersOnline,
        int usedPcu,
        int activeGridCount,
        int activeEntityCount,
        int totalBlockCount = -1,
        int floatingObjectCount = -1)
    {
        TimestampUnixSeconds = timestampUnixSeconds;
        SimSpeed = simSpeed;
        CpuPercent = cpuPercent;
        MemoryMb = memoryMb;
        FrameTimeMs = frameTimeMs;
        PlayersOnline = playersOnline;
        UsedPcu = usedPcu;
        ActiveGridCount = activeGridCount;
        ActiveEntityCount = activeEntityCount;
        TotalBlockCount = totalBlockCount;
        FloatingObjectCount = floatingObjectCount;
    }
}
