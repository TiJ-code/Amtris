using System;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class MenuManager : MonoBehaviour
{
    private TexturePackLoader texturePackLoader;
    private TileStorage tileStorage;

    [SerializeField]
    private GameObject contentPane;
    private Image panel;

    void Start()
    {
        texturePackLoader = FindObjectOfType<TexturePackLoader>();
        tileStorage = FindObjectOfType<TileStorage>();
        panel = GetComponent<Image>();

        Vector3 currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, -Screen.height / 2f, currentPosition.z);
    }

    public void StartGame()
    {
        contentPane.SetActive(false);
        panel.color = Color.white;

        if (tileStorage.tilesPerType.Count == 0 || tileStorage.tilesPerType == null) {
            foreach (Texture2D defaultTexture in tileStorage.defaultTextures)
            {
                string textureName = defaultTexture.name;

                texturePackLoader.SaveTextureToFile(defaultTexture, Path.Combine(TexturePackLoader.UsagePath, textureName + ".png"));

                for (int i = 1; i < 4; i++)
                {
                    Texture2D rotatedTexture = texturePackLoader.RotateTexture(defaultTexture, i * 90);
                    texturePackLoader.SaveTextureToFile(rotatedTexture, Path.Combine(TexturePackLoader.UsagePath, textureName + $"-{i * 90}.png"));
                    texturePackLoader.CreateAndSaveTile(rotatedTexture, textureName);
                }
                texturePackLoader.CreateAndSaveTile(defaultTexture, textureName);
            }
        }
        
        transform.LeanMoveY(Screen.height / 2f, 1.2f).setEaseInOutExpo().setOnComplete(SwitchScene);
    }

    private void SwitchScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenSettings()
    {
        contentPane.SetActive(true);
        panel.color = Color.white;
        texturePackLoader.LoadTexturePacks();
        transform.LeanMoveY(Screen.height / 2f, 1.2f).setEaseInOutExpo();
    }

    public void CloseSettings()
    {
        transform.LeanMoveY(-Screen.height / 2f, 1.2f).setEaseInOutExpo();
    }

    public void QuitGame()
    {
        contentPane.SetActive(false);
        panel.color = Color.black;
        transform.LeanMoveY(Screen.height / 2f, 1.2f).setEaseInOutExpo().setOnComplete(EndGame);
    }

    private void EndGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
