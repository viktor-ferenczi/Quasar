namespace Quasar.Services.Analytics;

internal static class MetricSampleValidator
{
    private const float MaxSimSpeed = 5f;
    private const float MaxMemoryMb = 1024f * 1024f;
    private const float MaxFrameTimeMs = 60_000f;
    private const int MaxReasonableCount = 100_000_000;

    public static bool TryNormalize(in MetricSample sample, out MetricSample normalized)
    {
        normalized = default;

        if (sample.TimestampUnixSeconds <= 0)
            return false;

        if (!float.IsFinite(sample.SimSpeed)
            || !float.IsFinite(sample.CpuPercent)
            || !float.IsFinite(sample.MemoryMb)
            || !float.IsFinite(sample.FrameTimeMs))
        {
            return false;
        }

        var simSpeed = Math.Clamp(sample.SimSpeed, 0f, MaxSimSpeed);
        var frameTimeMs = sample.FrameTimeMs;
        if (frameTimeMs <= 0f && simSpeed > 0.001f)
            frameTimeMs = 1000f / (simSpeed * 60f);

        normalized = new MetricSample(
            timestampUnixSeconds: sample.TimestampUnixSeconds,
            simSpeed: simSpeed,
            cpuPercent: Math.Clamp(sample.CpuPercent, 0f, GetMaxProcessCpuPercent()),
            memoryMb: Math.Clamp(sample.MemoryMb, 0f, MaxMemoryMb),
            frameTimeMs: Math.Clamp(frameTimeMs, 0f, MaxFrameTimeMs),
            playersOnline: NormalizeCount(sample.PlayersOnline),
            usedPcu: NormalizeCount(sample.UsedPcu),
            activeGridCount: NormalizeOptionalCount(sample.ActiveGridCount),
            activeEntityCount: NormalizeOptionalCount(sample.ActiveEntityCount),
            totalBlockCount: NormalizeOptionalCount(sample.TotalBlockCount),
            floatingObjectCount: NormalizeOptionalCount(sample.FloatingObjectCount));

        return true;
    }

    private static float GetMaxProcessCpuPercent() => Math.Max(100f, Environment.ProcessorCount * 100f);

    private static int NormalizeCount(int value)
    {
        if (value <= 0)
            return 0;

        return Math.Min(value, MaxReasonableCount);
    }

    private static int NormalizeOptionalCount(int value)
    {
        if (value < 0)
            return -1;

        return Math.Min(value, MaxReasonableCount);
    }
}
