using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform parent;
    public GameObject[] Amtrominos;
    public float movementFrequency = 0.8f;
    private float passedTime = 0f;

    private float startDelay = 1f;
    private float stepDelay = 1f;
    private float stepDecreaseRate = 1f / 10e6f;
    private float moveDelay = 0.1f;
    private float lockDelay = 0.5f;

    private float stepTime;
    private float moveTime;
    private float lockTime;


    private GameObject currentAmtromino;

    private bool isGameOver = false;
    private bool running = false;

    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(3f);
        running = true;

        SpawnAmtromino();

        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;
        stepDelay = startDelay;
    }

    private void Update()
    {
        if (running)
        {
            if (isGameOver) return;

            lockTime += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentAmtromino.transform.Rotate(0f, 0f, 90f);
                if (!IsValidPosition())
                {
                    currentAmtromino.transform.Rotate(0f, 0f, -90f);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                HardDrop();
            }

            if (Time.time > moveTime)
            {
                UserInput();
            }

            if (Time.time > stepTime)
            {
                Step();
            }

            stepDelay -= stepDecreaseRate * Time.deltaTime;
        }
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;

        MoveAmtromino(Vector2.down);

        if (lockTime >= lockDelay)
        {
            Lock() ;
        }
    }

    private void HardDrop()
    {
        while (MoveAmtromino(Vector2.down))
        {
            continue;
        }
        Lock();
    }


    private void UserInput()
    {
        // touch input
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveAmtromino(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveAmtromino(Vector3.right);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (MoveAmtromino(Vector2.down))
            {
                stepTime = Time.time + stepDelay;
            }
        }
    }

    private void Lock()
    {
        GetComponent<Grid>().UpdateGrid(currentAmtromino.transform);
        CheckForLines();
        SpawnAmtromino();
    }

    private void SpawnAmtromino()
    {
        int index = Random.Range(0, Amtrominos.Length);
        GameObject amtromino = Amtrominos[index];
        if (GetComponent<Grid>().WouldBeGameOver(amtromino.transform))
        {
            GameOver();
            return;
        }
        currentAmtromino = Instantiate(amtromino, new Vector3(4, 19, 0), Quaternion.identity, parent);
        currentAmtromino.SetActive(true);
    }

    private bool MoveAmtromino(Vector3 direction)
    {
        currentAmtromino.transform.position += direction;
        if (!IsValidPosition())
        {
            currentAmtromino.transform.position -= direction;
            if (IsGameOver())
            {
                GameOver();
            }
            return false;
        }

        moveTime = Time.time + moveDelay;
        lockTime = 0f;

        return true;
    }

    private bool IsValidPosition()
    {
        return GetComponent<Grid>().IsValidPosition(currentAmtromino.transform);
    }

    private void CheckForLines()
    {
        GetComponent<Grid>().CheckForLines();
    }

    private bool IsGameOver()
    {
        return GetComponent<Grid>().IsGameOver(currentAmtromino.transform);
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over!");
    }
}
