using System.Diagnostics;
using System.Text.Json;

namespace ItemTime.ProcessUsage;
public class ProcessUsageInfo
{
    readonly Process? process;
    PerformanceCounter? cpuCtr;
    PerformanceCounter? ramCtr;
    PerformanceCounter? privatedRamCtr;
    public ProcessUsageInfo(Process process) 
    {
        this.process = process;
        Init();
    }
    private void Init()
    {
        cpuCtr = new PerformanceCounter("Process", "% Processor Time", Name, true);
        ramCtr = new PerformanceCounter("Process", "Working Set", Name, true);
        privatedRamCtr = new PerformanceCounter("Process", "Private Bytes", Name, true);
    }
    private float CalculateCpu()
    {
        float? cpu = 0;
        for (int i = 0; i < 5; i++) 
        {
            if ((cpu = cpuCtr?.NextValue()) > 0)
                break;
        }
        return (float)cpu!;
    }
    public string Name => (string)process?.ProcessName!;
    public float Cpu => CalculateCpu();
    public float Ram => (float)ramCtr?.NextValue()!;
    public float PrivatedRam => (float)privatedRamCtr?.NextValue()!;
    public int HandleCount => (int)process?.HandleCount!;
    public string ToJson() => JsonSerializer.Serialize<ProcessUsageInfo>(this, new JsonSerializerOptions { WriteIndented = true, AllowTrailingCommas = true });
    public override string ToString()
    {
        string info = $"cpu: {Cpu:f2}\n";
        info += $"ram: {Ram} B\n";
        info += $"privated ram: {PrivatedRam} B\n";
        info += $"handle opened: {HandleCount}\n";
        return info;
    }
}
