using System;
using System.IO;
using System.Net.NetworkInformation;

public static class Infrastructure
{
    public static string GameFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Amtris_test");
    public static string ResourcePackPath = Path.Combine(GameFilePath, "resourcepacks_test");
    public static string WorkPath = Path.Combine(GameFilePath, "game_test");
    public static string ExtractionPath = Path.Combine(WorkPath, "extract_test");
    public static string UsagePath = Path.Combine(WorkPath, "usage_test");
    public static string OnlinePath = Path.Combine(WorkPath, "online_test");

    public const string repositoryOwner = "TiJ_code";
    public const string repositoryName = "Amtris-Textures";

    public static void CreateFileInfrastructure()
    {
        Directory.CreateDirectory(GameFilePath);
        Directory.CreateDirectory(ResourcePackPath);
        Directory.CreateDirectory(WorkPath);
        Directory.CreateDirectory(ExtractionPath);
        Directory.CreateDirectory(UsagePath);
        Directory.CreateDirectory(OnlinePath);
    }

    public static bool CheckNetworkConnection()
    {
        try
        {
            Ping ping = new Ping();
            string host = "github.com";
            byte[] buffer = new byte[32];
            int timeout = 1000;
            PingOptions pingOptions = new PingOptions();
            PingReply reply = ping.Send(host, timeout, buffer, pingOptions);
            return (reply.Status == IPStatus.Success);
        } 
        catch (Exception e)
        {
            return false;
        }
    }
}
