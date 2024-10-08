using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmtrominoTexture : MonoBehaviour
{
    private Textures textures;
    private SpriteRenderer[] spriteRenderers;

    private void OnEnable()
    {
        textures = FindObjectOfType<Textures>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        string amtrominoName = this.name.Split(' ')[1].Replace("(Clone)", "") + "-shape";
        Sprite sprite = null;

        foreach (var item in textures.textures)
        {

            if (item.Key == amtrominoName)
            {
                Texture2D spriteTexture = item.Value;
                sprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), 
                                       new Vector2(0.5f, 0.5f), 1024f);
                break;
            }
        }

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
            }
        }
    }
}
