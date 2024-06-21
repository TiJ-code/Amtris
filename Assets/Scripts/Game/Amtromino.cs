using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections.Generic;

public enum Amtromino
{
    I, O, T, L, J, S, Z
}

[System.Serializable]
public struct AmtrominoData
{
    public Tile[] tiles;
    public Amtromino amtromino;

    public Vector2Int[] cells { get; private set; }
    public Vector2Int[,] wallKicks { get; private set; }

    public void Initialise()
    {
        cells = Data.Cells[amtromino];
        wallKicks = Data.WallKicks[amtromino];
    }
}