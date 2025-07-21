
using Godot;

public class KoreGodotMemoryStats
{
    // RenderingServer values
    public float VideoMemoryUsedMB   { get; set; }
    public float TextureMemoryUsedMB { get; set; }
    public float BufferMemUsedMB     { get; set; } 
    public ulong TotalDrawCalls      { get; set; }
    
    // OS.GetMemoryInfo values
    public float PhysicalMemoryMB    { get; set; }
    public float AvailableMemoryMB   { get; set; }
    public float FreeMemoryMB        { get; set; }
    public float StackMemoryMB       { get; set; }

    public KoreGodotMemoryStats()
    {
        VideoMemoryUsedMB   = 0;
        TextureMemoryUsedMB = 0;
        BufferMemUsedMB     = 0;
        TotalDrawCalls      = 0;
        
        PhysicalMemoryMB    = 0;
        AvailableMemoryMB   = 0;
        FreeMemoryMB        = 0;
        StackMemoryMB       = 0;
    }
        
    public void UpdateStats()
    {
        // Get the amount of texture memory used
        ulong vidMem    = RenderingServer.GetRenderingInfo(RenderingServer.RenderingInfo.VideoMemUsed);
        ulong texMem    = RenderingServer.GetRenderingInfo(RenderingServer.RenderingInfo.TextureMemUsed);
        ulong bufMem    = RenderingServer.GetRenderingInfo(RenderingServer.RenderingInfo.BufferMemUsed);
        ulong drawCalls = RenderingServer.GetRenderingInfo(RenderingServer.RenderingInfo.TotalDrawCallsInFrame);
        
        // Convert this into a decimal MBytes value
        VideoMemoryUsedMB   = (float)vidMem / 1024 / 1024;
        TextureMemoryUsedMB = (float)texMem / 1024 / 1024;
        BufferMemUsedMB     = (float)bufMem / 1024 / 1024;
        TotalDrawCalls      = drawCalls;

        Godot.Collections.Dictionary memDict = OS.GetMemoryInfo();
        PhysicalMemoryMB  = (float)memDict["physical"]  / 1024 / 1024;
        AvailableMemoryMB = (float)memDict["available"] / 1024 / 1024;
        FreeMemoryMB      = (float)memDict["free"]      / 1024 / 1024;
        StackMemoryMB     = (float)memDict["stack"]     / 1024 / 1024;
    }

    public void GDPrintStats()
    {
        GD.Print($"Video Memory Used: {VideoMemoryUsedMB:N0} MB // Texture Memory Used: {TextureMemoryUsedMB:N0} MB // Buffer Memory Used: {BufferMemUsedMB:N0} MB // Draw Calls: {TotalDrawCalls:N0}");
        GD.Print($"Physical Memory: {PhysicalMemoryMB:N0} MB // Available Memory: {AvailableMemoryMB:N0} MB // Free Memory: {FreeMemoryMB:N0} MB // Stack Memory: {StackMemoryMB:N0} MB");
    }

}
