using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class TileMapTiles : MonoBehaviour
{
    public Tilemap tilemap;

    public void AddTilesToBoard()
    {
        Dictionary<string, List<Tile>> tilesDict = FindObjectOfType<TileStorage>().tilesPerType;

        Board board = FindObjectOfType<Board>();
        
        foreach (AmtrominoData itAmtData in board.amtrominos) 
        {
            AmtrominoData amtData = itAmtData;
            foreach (var kvp in tilesDict)
            {
                List<Tile> tileList = kvp.Value;
                string key = kvp.Key;
                if (amtData.amtromino.ToString() == key.Substring(0, 1)) 
                {
                    amtData.tiles = tileList.ToArray();
                    board.amtrominos[Array.IndexOf(board.amtrominos, itAmtData)] = amtData;
                    break;
                }
            }
        }
    }
}
