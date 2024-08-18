using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ApplyTexturePacks : MonoBehaviour
{
    public static ApplyTexturePacks instance;

    public List<TexturePack_Element> elements = new List<TexturePack_Element>();

    [SerializeField]
    public GameObject defaultTexturePackObject;
    [SerializeField]
    public TMP_Text defaultTexturePackIndicator;
    [SerializeField]
    public RectTransform defaultTexturePackProgressbarRect;

    private Textures textures;

    public static string[] letterStates =
    {
        "C", "B"
    };

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            textures = FindObjectOfType<Textures>();
        }
    }

    public void ApplyOnClickListener()
    {
        foreach (var texElement in elements)
        {
            texElement.GetComponent<Button>().onClick.RemoveAllListeners();
            texElement.GetComponent<Button>().onClick.AddListener(() => ApplyTexturePack(texElement));
        }
    }

    public void LoadResourcePack()
    {
        string saved_path = PlayerPrefs.GetString("resourcepack", "default");
        if (saved_path != null)
        {
            if (saved_path == "default")
            {
                ApplyDefaultTexturePack();
            } 
            else
            {
                InitialiseLoadedResourcePack(saved_path);
            }
        }
    }

    public void FixLoadedUi()
    {
        string saved_path = PlayerPrefs.GetString("resourcepack", "default");
        if (saved_path != null)
        {
            if (saved_path != "default")
            {
                string[] parts = Path.GetFullPath(saved_path).Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                string pack_name = parts[parts.Length - 2];
                foreach (var textureElement in elements)
                {
                    if (textureElement.pack_name == pack_name)
                    {
                        textureElement.indicator.text = letterStates[1];
                        break;
                    }
                }
            }
        }
    }

    private void InitialiseLoadedResourcePack(string path)
    {
        string[] textureFiles = Directory.GetFiles(path, "*.png");

        foreach (string file in textureFiles)
        {
            string textureName = Path.GetFileNameWithoutExtension(file);
            Texture2D newTexture = LoadTextureFromFile(file);

            textures.ReplaceTexture(textureName, newTexture);
        }
    }

    public void ApplyTexturePack(TexturePack_Element element)
    {
        UnapplyAllPacks();
        UnapplyDefaultTexturePack();

        element.applied = true;

        string pack_path = Path.Combine(element.pack_path, "textures/");
        InitialiseLoadedResourcePack(pack_path);
        element.AnimateProgressbar();

        PlayerPrefs.SetString("resourcepack", pack_path);
        PlayerPrefs.Save();

        element.indicator.text = letterStates[1];
    }

    public void ApplyDefaultTexturePack()
    {
        UnapplyAllPacks();

        foreach (var defaultTexKey in textures.defaultTextures.Keys)
        {
            foreach (var texKey in textures.textures.Keys)
            {
                if (defaultTexKey == texKey)
                {
                    textures.textures[texKey] = textures.defaultTextures[texKey];
                    break;
                }
            }
        }

        float defaultMaxWidth = defaultTexturePackObject.GetComponent<RectTransform>().rect.width;
        ProgressbarAnimator.instance.AnimateProgressbar(defaultTexturePackProgressbarRect, defaultMaxWidth);

        PlayerPrefs.SetString("resourcepack", "default");
        PlayerPrefs.Save();

        defaultTexturePackIndicator.text = letterStates[1];
    }

    private void UnapplyDefaultTexturePack()
    {
        defaultTexturePackIndicator.text = letterStates[0];
    }

    private void UnapplyAllPacks()
    {
        foreach (var texElement in elements)
        {
            texElement.ApplyPack(false);
        }
    }

    private Texture2D LoadTextureFromFile(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }
        else
        {
            Debug.LogError("Failed to load texture from file: " + filePath);
        }

        return tex;
    }
}
