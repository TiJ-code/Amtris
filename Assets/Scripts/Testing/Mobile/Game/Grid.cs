using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform[,] grid;
    public int width, height;

    public Scoreboard scoreboard;

    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(3f);

        grid = new Transform[width, height];
        scoreboard = GetComponent<Scoreboard>();
    }

    public bool WouldBeGameOver(Transform amtromino)
    {
        Transform amtrominoTransform = amtromino.transform;
        amtrominoTransform.position = new Vector3(4f, 19f, 0f);
        foreach (Transform child in amtromino)
        {
            Vector2 position = Round(child.position);
            if (!IsInsideBorder(position))
            {
                return true;
            }

            if (GetTransformAtGridPosition(position) != null && GetTransformAtGridPosition(position).parent != amtromino)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsGameOver(Transform amtromino)
    {
        foreach (Transform child in amtromino)
        {
            Vector2 position = Round(child.position);
            if (!IsInsideBorder(position))
            {
                return true;
            }

            if (GetTransformAtGridPosition(position) != null && GetTransformAtGridPosition(position).parent != amtromino)
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateGrid(Transform amtromino)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x  = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == amtromino)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform child in amtromino)
        {
            Vector2 position = Round(child.position);
            if (position.y < height)
            {
                grid[(int)position.x, (int)position.y] = child;
            }
        }
    }

    public bool IsValidPosition(Transform amtromino)
    {
        foreach (Transform child in amtromino)
        {
            Vector2 position = Round(child.position);
            if (!IsInsideBorder(position))
            {
                return false;
            }

            if (GetTransformAtGridPosition(position) != null && GetTransformAtGridPosition(position).parent != amtromino)
            {
                return false;
            }
        }
        return true;
    }

    public Transform GetTransformAtGridPosition(Vector2 position)
    {
        if (position.y > height - 1)
        {
            return null;
        }
        return grid[(int)position.x, (int)position.y];
    }

    private bool IsInsideBorder(Vector2 position)
    {
        return (int) position.x >= 0 && (int)position.x < width &&
               (int) position.y >= 0 && (int)position.y <= height;
    }

    private static Vector2 Round(Vector2 v2)
    {
        return new Vector2(Mathf.Round(v2.x), Mathf.Round(v2.y));
    }

    public void CheckForLines()
    {
        for (int y = 0; y < height; y++)
        {
            if (LineIsFull(y))
            {
                DeleteLine(y);
                DecreaseRowsAbove(y + 1);
                scoreboard.AddLineCleared();
                y--;
            }
        }
    }

    private bool LineIsFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }

    private void DeleteLine(int y)
    {
        for (int x = 0; x < width; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    private void DecreaseRowsAbove(int startRow)
    {
        for (int y = startRow; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;
                    grid[x, y - 1].position += Vector3.down;
                }
            }
        }
    }
}
