using System;
using System.Collections.Generic;

namespace Magnetar.Protocol.Model;

public class ProfilerSnapshot
{
    public DateTimeOffset CapturedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public int WindowSeconds { get; set; }

    public ulong StartFrame { get; set; }

    public ulong EndFrame { get; set; }

    public int FrameCount { get; set; }

    public ProfilerTimingBreakdown GameLoop { get; set; } = new();

    public List<ProfilerEntrySnapshot> TopGrids { get; set; } = new();

    public List<ProfilerEntrySnapshot> TopScripts { get; set; } = new();

    public List<ProfilerEntrySnapshot> TopEntityTypes { get; set; } = new();

    public List<ProfilerEntrySnapshot> TopMethods { get; set; } = new();

    public List<ProfilerEntrySnapshot> TopPhysics { get; set; } = new();

    public List<ProfilerEntrySnapshot> TopNetworkEvents { get; set; } = new();
}

public class ProfilerTimingBreakdown
{
    public double FrameMs { get; set; }

    public double UpdateMs { get; set; }

    public double NetworkMs { get; set; }

    public double ReplicationMs { get; set; }

    public double SessionComponentsMs { get; set; }

    public double ScriptsMs { get; set; }

    public double PhysicsMs { get; set; }

    public double ParallelWaitMs { get; set; }

    public double OtherMs { get; set; }
}

public class ProfilerEntrySnapshot
{
    public string Key { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public long? EntityId { get; set; }

    public string GridName { get; set; } = string.Empty;

    public string BlockName { get; set; } = string.Empty;

    public string TypeName { get; set; } = string.Empty;

    public string MethodName { get; set; } = string.Empty;

    public double MainThreadMs { get; set; }

    public double OffThreadMs { get; set; }

    public double TotalMs { get; set; }

    public double MainThreadMsPerFrame { get; set; }

    public double OffThreadMsPerFrame { get; set; }

    public double TotalMsPerFrame { get; set; }

    public int Calls { get; set; }
}
