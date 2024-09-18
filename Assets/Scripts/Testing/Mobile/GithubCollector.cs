using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class GithubCollector : MonoBehaviour
{
    [Serializable]
    public class FileEntry
    {
        public string name;
        public string download_url;
    }

    [SerializeField]
    public class DownloadEntry
    {
        public string name;
        public string url;

        public DownloadEntry(string name, string url)
        {
            this.name = name;
            this.url = url;
        }
    }

    private const string texturesUrl = "https://api.github.com/repos/TiJ-code/Amtris-Textures/contents";

    private List<DownloadEntry> downloadEntries = new List<DownloadEntry>();
    private List<DownloadEntry> missingEntries = new List<DownloadEntry>();

    private async void Start()
    {
        await CollectZipFilesFromGithub();
        await CheckAndDownloadMissingTexturePacks();
        await UnpackAndValidateTexturePacks();
        DisplayTexturePacks.instance.Display();
    }

    async Task CollectZipFilesFromGithub()
    {
        using (var client = new WebClient())
        {
            client.Headers.Add("User-Agent", "Amtris/Android");
            string json = await client.DownloadStringTaskAsync(texturesUrl);
            FileEntry[] files = JsonConvert.DeserializeObject<FileEntry[]>(json);

            downloadEntries = files.Where(file => file.name.EndsWith(".zip"))
                .Select(zipFile => new DownloadEntry(zipFile.name, zipFile.download_url))
                .ToList();
        }
    }

    async Task CheckAndDownloadMissingTexturePacks()
    {
        foreach (DownloadEntry entry in downloadEntries)
        {
            string unpackedDirectory = Path.Combine(Infrastructure.usagePath, Path.GetFileNameWithoutExtension(entry.name));

            if (!Directory.Exists(unpackedDirectory))
            {
                missingEntries.Add(entry);
            }
        }

        foreach (DownloadEntry entry in missingEntries)
        {
            string zipFilePath = Path.Combine(Infrastructure.downloadPath, entry.name);

            if (!File.Exists(zipFilePath))
            {
                await DownloadFileAsync(entry.url, zipFilePath);
            }
        }
    }

    async Task UnpackAndValidateTexturePacks()
    {
        foreach (DownloadEntry entry in missingEntries)
        {
            string zipFilePath = Path.Combine(Infrastructure.downloadPath, entry.name);
            string unpackedDirectory = Path.Combine(Infrastructure.usagePath, Path.GetFileNameWithoutExtension(zipFilePath));

            if (File.Exists(zipFilePath))
            {
                bool isValid = await UnpackAndValidateTexturePackAsync(zipFilePath, unpackedDirectory);
                if (!isValid)
                {
                    Directory.Delete(unpackedDirectory, true);
                    File.Delete(zipFilePath);
                }
                else
                {
                    File.Delete(zipFilePath);
                }
            }
        }
    }

    async Task DownloadFileAsync(string url, string filePath)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        using (var fileStream = File.Create(filePath))
                        {
                            await stream.CopyToAsync(fileStream);
                        }
                    }

                    Debug.Log($"File downloaded and saved to {filePath}");
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Debug.LogError($"Error downloading file: {ex.Message} - - {ex.StackTrace}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error downloading file: {ex.Message} - {ex.StackTrace}");
        }
    }

    async Task<bool> UnpackAndValidateTexturePackAsync(string zipFilePath, string unpackedDirectory)
    {
        try
        {
            using (var zipFile = ZipFile.OpenRead(zipFilePath))
            {
                zipFile.ExtractToDirectory(unpackedDirectory);
            }

            bool hasConfigJson = File.Exists(Path.Combine(unpackedDirectory, "config.json"));
            bool hasIconPng = File.Exists(Path.Combine(unpackedDirectory, "icon.png"));
            bool hasTexturesDir = Directory.Exists(Path.Combine(unpackedDirectory, "textures"));

            if (hasConfigJson && hasIconPng && hasTexturesDir)
            {
                Debug.Log($"{Path.GetFileName(zipFilePath)} is valid");
                return true;
            }
            else
            {
                Debug.LogWarning($"{Path.GetFileName(zipFilePath)} is invalid");
                return false;
            }
        }
        catch (IOException ex)
        {
            Debug.LogError($"Error extracting or accessing ZIP file: {ex.Message}");
            return false;
        }
    }
}