using System;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class TexturePackLoader : MonoBehaviour
{
    public static string GameFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Amtris");
    public static string ResourcePackPath = Path.Combine(GameFilePath, "resourcepacks");
    public static string ExtractionPath = Path.Combine(ResourcePackPath, ".extract");
    public static string UsagePath = Path.Combine(ExtractionPath, ".use");
    public static string AssetsPath = Path.Combine(GameFilePath, "assets");

    public List<TexturePack> texturePacks = new List<TexturePack>();

    [SerializeField] private Transform texturePackContainer;
    [SerializeField] private GameObject texturePrefab;

    public TileStorage tileStorage;

    private void Start()
    {
        tileStorage = FindObjectOfType<TileStorage>();
    }

    public void LoadTexturePacks()
    {
        texturePacks.Clear();

        ClearExtractionPath();
        CreateDirectories();

        string[] zipFiles = Directory.GetFiles(ResourcePackPath, "*.zip");
        foreach (string zipFilePath in zipFiles) 
        {
            if (IsValidTexturePack(zipFilePath))
            {
                UnpackTexturePack(zipFilePath);
            }
        }
    }

    private void CreateDirectories()
    {
        Directory.CreateDirectory(GameFilePath);
        Directory.CreateDirectory(ResourcePackPath);
        Directory.CreateDirectory(ExtractionPath);
        Directory.CreateDirectory(UsagePath);
        Directory.CreateDirectory(AssetsPath);
    }

    private bool IsValidTexturePack(string zipFilePath)
    {
        using (ZipArchive archive = ZipFile.OpenRead(zipFilePath)) 
        {
            bool hasConfigJson = false;
            bool hasIconPng = false;
            bool hasTexturesDirectory = false;

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.FullName == "config.json")
                {
                    hasConfigJson = true;
                }
                else if (entry.FullName == "icon.png")
                {
                    hasIconPng = true;
                }
                else if (entry.FullName.StartsWith("textures", StringComparison.OrdinalIgnoreCase))
                {
                    hasTexturesDirectory = true;
                }
            }

            return hasConfigJson && hasIconPng && hasTexturesDirectory;
        }
    }

    private void UnpackTexturePack(string zipFilePath) 
    {
        string unpackPath = Path.Combine(ExtractionPath, Path.GetFileNameWithoutExtension(zipFilePath));
        ZipFile.ExtractToDirectory(zipFilePath, unpackPath);

        TexturePack texturePack = new TexturePack();

        string configPath = Path.Combine(unpackPath, "config.json");
        string configJson = File.ReadAllText(configPath);
        TexturePackConfig packConfig = JsonConvert.DeserializeObject<TexturePackConfig>(configJson);
        texturePack.name = packConfig.name;
        texturePack.description = packConfig.description;

        string iconPath = Path.Combine(unpackPath, "icon.png");
        byte[] iconBytes = File.ReadAllBytes(iconPath);
        Texture2D iconTexture = new Texture2D(1, 1);
        iconTexture.LoadImage(iconBytes);
        texturePack.icon = iconTexture;

        string[] textureFiles = Directory.GetFiles(Path.Combine(unpackPath, "textures"));
        foreach (string textureFile in textureFiles)
        {
            string textureName = Path.GetFileNameWithoutExtension(textureFile);
            Texture2D texture = LoadTextureFromFile(textureFile);
            texturePack.textures.Add(textureName, texture);
        }

        GameObject newTexturePackObj = Instantiate(texturePrefab, texturePackContainer);
        TexturePackUIElement texPackUI = newTexturePackObj.GetComponent<TexturePackUIElement>();
        texPackUI.texturePack = texturePack;

        TMP_Text title = newTexturePackObj.transform.Find("Title").GetComponent<TMP_Text>();
        title.text = texturePack.name;
        TMP_Text description = newTexturePackObj.transform.Find("Description").GetComponent<TMP_Text>();
        description.text = texturePack.description;
        RawImage icon = newTexturePackObj.transform.Find("Icon").GetComponent<RawImage>();
        icon.texture = texturePack.icon;
        Button applyButton = newTexturePackObj.transform.Find("ApplyButton").GetComponent<Button>();
        applyButton.onClick.AddListener(() => CreateTilesFromTextures(texturePack));

        texturePacks.Add(texturePack);
    }

    private Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(fileData);
        return texture;
    }

    public void CreateTilesFromTextures(TexturePack texturePack)
    {
        foreach (KeyValuePair<string, Texture2D> textureEntry in texturePack.textures)
        {
            string textureName = textureEntry.Key;
            Texture2D texture = textureEntry.Value;

           SaveTextureToFile(texture, Path.Combine(UsagePath, textureName + ".png"));

            for (int i = 1; i < 4; i++)
            {
                Texture2D rotatedTexture = RotateTexture(texture, i * 90);
                SaveTextureToFile(rotatedTexture, Path.Combine(UsagePath, textureName + $"-{i * 90f}.png"));
                CreateAndSaveTile(rotatedTexture, textureName);
            }

            CreateAndSaveTile(texture, textureName);
        }
    }

    public Texture2D RotateTexture(Texture2D originalTexture, int degrees)
    {
        Texture2D rotatedTexture = new Texture2D(originalTexture.width, originalTexture.height);

        Vector2 pivotPoint = new Vector2(0.5f, 0.5f);

        for (int x = 0; x < rotatedTexture.width; x++)
        {
            for (int y = 0; y < rotatedTexture.height; y++) 
            {
                float newX = Mathf.Cos(Mathf.Deg2Rad * degrees) * (x - pivotPoint.x) - Mathf.Sin(Mathf.Deg2Rad * degrees) * (y - pivotPoint.y) + pivotPoint.x;
                float newY = Mathf.Sin(Mathf.Deg2Rad * degrees) * (x - pivotPoint.x) + Mathf.Cos(Mathf.Deg2Rad * degrees) * (y - pivotPoint.y) + pivotPoint.y;

                Color colour = originalTexture.GetPixelBilinear(newX / rotatedTexture.width, newY / rotatedTexture.height);

                rotatedTexture.SetPixel(x, y, colour);
            }
        }

        rotatedTexture.Apply();
        return rotatedTexture;
    }

    public void SaveTextureToFile(Texture2D texture, string filePath) 
    {
        Texture2D tempTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        tempTexture.SetPixels32(texture.GetPixels32());
        tempTexture.Apply();

        byte[] bytes = tempTexture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
    }

    public void CreateAndSaveTile(Texture2D texture, string tileName)
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 1024);

        tileStorage.Add(tileName, tile);
    }

    private void ClearExtractionPath()
    {
        if (Directory.Exists(ExtractionPath))
        {
            Directory.Delete(ExtractionPath, true);
        }
    }
}

[System.Serializable]
public class TexturePack
{
    public string name;
    public string description;
    public Texture2D icon;
    public Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
}

public class TexturePackConfig
{
    public string name;
    public string description;
}