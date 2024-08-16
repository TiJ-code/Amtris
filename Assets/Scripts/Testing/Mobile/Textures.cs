using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Textures : MonoBehaviour
{
    public static Textures instance { get; private set; }

    [SerializeField]
    public TextureDictionary textureDictionary = new TextureDictionary();
    [SerializeField]
    public TextureDictionary defaultTextureDictionary = new TextureDictionary();

    public Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
    public Dictionary<string, Texture2D> defaultTextures = new Dictionary<string, Texture2D>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            textures = textureDictionary.ToDictionary();
            DontDestroyOnLoad(this);
        }
    }

    public void ReplaceTexture(string name, Texture2D texture)
    {
        if (textures.ContainsKey(name))
        {
            textures[name] = texture;
        }
        else
        {
            Debug.LogError($"Texture with name '{name}' does not exist.");
        }
    }

    public Texture2D GetTexture(string name)
    {
        if (textures.ContainsKey(name)) return textures[name];
        else
        {
            Debug.LogError($"Texture with name '{name}' does not exist.");
            return null;
        }
    }
}


[Serializable]
public class TextureDictionary
{
    [SerializeField]
    TextureDictionaryItem[] items;

    public Dictionary<string, Texture2D> ToDictionary()
    {
        Dictionary<string, Texture2D> newDict = new Dictionary<string, Texture2D>();

        foreach (var item in items)
        {
            newDict.Add(item.name, item.texture);
        }

        return newDict;
    }

    public void ReplaceTexture(string name, Texture2D texture)
    {
        foreach (var item in items)
        {
            if (item.name == name)
            {
                item.texture = texture;
                break;
            }
        }
    }
}

[Serializable]
public class TextureDictionaryItem
{
    [SerializeField]
    public string name;
    [SerializeField]
    public Texture2D texture;
}