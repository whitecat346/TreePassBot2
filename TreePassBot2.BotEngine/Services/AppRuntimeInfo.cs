using System.Diagnostics;

// ReSharper disable InconsistentNaming

namespace TreePassBot2.BotEngine.Services;

public class AppRuntimeInfo
{
    public DateTimeOffset StartTime { get; internal set; }
    public TimeSpan Uptime => DateTimeOffset.UtcNow - StartTime;
    public string CurrentVersion { get; internal set; } = "Unknown";

    public async Task<double> GetCpuUsageAsync(int delayMs = 500)
    {
        var process = Process.GetCurrentProcess();

        var cpuTimeStart = process.TotalProcessorTime;
        var timeStart = DateTime.UtcNow;

        await Task.Delay(delayMs).ConfigureAwait(false);

        var cpuTimeEnd = process.TotalProcessorTime;
        var timeEnd = DateTime.UtcNow;

        var cpuUsedMs = (cpuTimeEnd - cpuTimeStart).TotalMilliseconds;
        var totalMsPassed = (timeEnd - timeStart).TotalMilliseconds;

        var cpuUsageTotal =
            cpuUsedMs / (Environment.ProcessorCount * totalMsPassed) * 100;

        return Math.Round(Math.Clamp(cpuUsageTotal, 0, 100), 2);
    }

    public double GetMemoryUsage()
    {
        // only .net accessible memory info. not physical memory
        // but i dont want to impl cross-platform physical memory check now
        var info = GC.GetGCMemoryInfo();

        var totalMemory = info.TotalAvailableMemoryBytes / 1024d / 1024d;
        var usedMemory = GC.GetTotalMemory(false) / 1024d / 1024d;

        var usagePercent = totalMemory == 0
            ? 0
            : usedMemory / totalMemory * 100;

        return Math.Round(usagePercent, 2);
    }

    public double GetDiskUsage()
    {
        var disks = GetDisks();
        return Math.Round(disks.Average(disk => disk.UsagePercent), 2);
    }

    private static List<DiskStatusDto> GetDisks()
    {
        var disks = new List<DiskStatusDto>();

        foreach (var drive in DriveInfo.GetDrives())
        {
            if (!drive.IsReady)
                continue;

            var totalGb = drive.TotalSize / 1024d / 1024d / 1024d;
            var freeGb = drive.AvailableFreeSpace / 1024d / 1024d / 1024d;
            var usedGb = totalGb - freeGb;

            disks.Add(new DiskStatusDto
            {
                Name = drive.Name,
                TotalGB = Math.Round(totalGb, 2),
                UsedGB = Math.Round(usedGb, 2),
                UsagePercent = totalGb == 0 ? 0 : usedGb / totalGb * 100
            });
        }

        return disks;
    }

    private record DiskStatusDto
    {
        public string Name { get; set; } = string.Empty;
        public double TotalGB { get; set; }
        public double UsedGB { get; set; }
        public double UsagePercent { get; init; }
    }
}
