using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class Infrastructure : MonoBehaviour
{
    public static string applicationPath;
    public static string downloadPath;
    public static string usagePath;

    public static ErrorNode errorNode = ErrorNode.NONE;

    public static string[] textureNames =
    {
        "I-shape", "O-shape", "T-shape", "S-shape", "Z-shape", "L-shape", "J-shape"
    };

    private void Awake()
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

    public static async Task<bool> IsConnected()
    {
        try
        {
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) })
            {
                var response = await client.GetAsync("https://github.com");
                return response.IsSuccessStatusCode;
            }
        }
        catch
        {
            return false;
        }
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
