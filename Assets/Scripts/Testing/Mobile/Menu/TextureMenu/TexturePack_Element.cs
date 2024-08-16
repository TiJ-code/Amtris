using Newtonsoft.Json;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class TexturePack_Element : MonoBehaviour
{
    [SerializeField]
    public string pack_name;
    [SerializeField]
    public string pack_path;

    [SerializeField]
    public bool applied = false;

    [SerializeField]
    public TMP_Text title;
    [SerializeField]
    public TMP_Text description;
    [SerializeField]
    public TMP_Text indicator;
    [SerializeField]
    public RawImage icon;

    [SerializeField]
    public GameObject progressbarPanel;
    [SerializeField]
    public RectTransform progressbarRectTransform;
    private float maxWidth;

    private Config config;

    public void Setup()
    {
        config = LoadConfig();

        if (config != null)
        {
            title.text = config.name;
            description.text = config.description;
        }
        else
        {
            title.text = string.Empty;
            description.text = string.Empty;
        }

        icon.texture = GetIcon();

        maxWidth = this.gameObject.GetComponent<RectTransform>().rect.width;
        progressbarRectTransform.sizeDelta = new Vector2(0, progressbarRectTransform.rect.height);
    }

    public void SetValues(string packName, string packPath)
    {
        this.pack_name = packName;
        this.pack_path = packPath;
    }

    public Texture2D GetIcon()
    {
        string iconPath = Path.Combine(pack_path, "icon.png");

        try
        {
            byte[] fileBytes = File.ReadAllBytes(iconPath);
            Texture2D iconTexture = new Texture2D(1, 1);
            iconTexture.LoadImage(fileBytes);
            return iconTexture;
        }
        catch (IOException ex)
        {
            Debug.LogException(ex);
        }

        return new Texture2D(100, 100);
    }

    private Config LoadConfig()
    {
        string configPath = Path.Combine(pack_path, "config.json");

        try
        {
            string json = File.ReadAllText(configPath);
            return JsonConvert.DeserializeObject<Config>(json);
        }
        catch (IOException ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }

    public void ApplyPack(bool apply)
    {
        applied = apply;
        indicator.text = applied ? ApplyTexturePacks.letterStates[1] : ApplyTexturePacks.letterStates[0];
    }

    public void AnimateProgressbar()
    {
        maxWidth = gameObject.GetComponent<RectTransform>().rect.width;
        ProgressbarAnimator.instance.AnimateProgressbar(progressbarRectTransform, maxWidth);
    }
}

[System.Serializable]
class Config
{
    public string name { get; set; }
    public string description { get; set; }
}