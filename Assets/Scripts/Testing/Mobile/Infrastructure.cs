using System.IO;
using UnityEngine;

public class Infrastructure : MonoBehaviour
{
    public static string applicationPath;
    public static string downloadPath;
    public static string usagePath;

    public static bool IsStartUpComplete = false;

    public static ErrorNode errorNode = ErrorNode.NONE;

    public static string[] textureNames =
    {
        "I-shape", "O-shape", "T-shape", "S-shape", "Z-shape", "L-shape", "J-shape"
    };

    private async void Awake()
    {
        applicationPath = Application.persistentDataPath;
        downloadPath = applicationPath + "/.download/";
        usagePath = applicationPath + "/.texturepacks/";


        if (!Directory.Exists(downloadPath))
        {
            Directory.CreateDirectory(downloadPath);
        }
        if (!Directory.Exists(usagePath))
        {
            Directory.CreateDirectory(usagePath);
        }
    }

    public static bool IsConnected()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    public static long GetAvailableDiskSpace(string path)
    {
        var driveInfo = new DriveInfo(Path.GetPathRoot(path));
        return driveInfo.AvailableFreeSpace;
    }
}

public enum ErrorNode
{ 
    NONE,
    NO_CONNECTION,
    DISK_SPACE
} 
