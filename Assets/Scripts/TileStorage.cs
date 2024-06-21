using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileStorage : MonoBehaviour
{
    public Dictionary<string, List<Tile>> tilesPerType = new Dictionary<string, List<Tile>>();
    public List<Texture2D> defaultTextures = new List<Texture2D>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void Add(string key, Tile value)
    {
        if (!tilesPerType.TryGetValue(key, out List<Tile> tiles)) 
        {
            tiles = new List<Tile>();
            tilesPerType.Add(key, tiles);
        }
        tiles.Add(value);
    }
}
