using System.Collections;
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
    public bool isPaused = false;
    private bool running = false;

    private Vector2 startTouchPosition;
    private Vector2 currentPosition;
    private Vector2 endTouchPosition;
    private bool moving = false;
    private bool movingDown = false;
    private float touchDelay = 0.2f;
    private float touchTime;
    private static float leftRightSensitivity = 65f;
    private static float hardDropSensitivity = 150f;
    private float tapRange = 65f;

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
        if (running && !isPaused)
        {
            if (isGameOver) return;

            lockTime += Time.deltaTime;

            MobileControls();
            KeyboardControls();

            if (Time.time > stepTime)
            {
                Step();
            }

            stepDelay -= stepDecreaseRate * Time.deltaTime;
        }
    }

    private void KeyboardControls()
    {
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
    }

    private void MobileControls()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                startTouchPosition = Input.GetTouch(0).position;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Moved || moving)
            {
                currentPosition = Input.GetTouch(0).position;
                Vector2 distance = currentPosition - startTouchPosition;

                // Swipe left
                if (distance.x < -leftRightSensitivity && Mathf.Abs(distance.y) < Mathf.Abs(distance.x))
                {
                    float tempDistance = distance.x;
                    while (tempDistance < -leftRightSensitivity)
                    {
                        MoveAmtromino(Vector2.left);
                        startTouchPosition.x -= leftRightSensitivity;
                        moving = true;
                        tempDistance = currentPosition.x - startTouchPosition.x;
                    }
                }
                // Swipe right
                else if (distance.x > leftRightSensitivity && Mathf.Abs(distance.y) < Mathf.Abs(distance.x))
                {
                    float tempDistance = distance.x;
                    while (tempDistance > leftRightSensitivity)
                    {
                        MoveAmtromino(Vector2.right);
                        startTouchPosition.x += leftRightSensitivity;
                        moving = true;
                        tempDistance = currentPosition.x - startTouchPosition.x;
                    }
                }

                // Swipe down
                if (movingDown || (distance.y < -hardDropSensitivity && Mathf.Abs(distance.y) > Mathf.Abs(distance.x)))
                {
                    if (Time.time > moveTime) 
                    {
                        MoveAmtromino(Vector2.down);
                        if (!movingDown)
                        {
                            touchTime = Time.time + touchDelay;
                        }
                        else if (distance.y < -hardDropSensitivity && Time.time >= touchTime)
                        {
                            startTouchPosition.y = currentPosition.y;
                        }
                        moving = true;
                        movingDown = true;
                    }
                }
                // Swipe down
                /*else if (!moving && distance.y > hardDropSensitivity && 120 > Mathf.Abs(distance.x))
                {}*/
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                endTouchPosition = Input.GetTouch(0).position;
                Vector2 distance = endTouchPosition - startTouchPosition;
                if (Mathf.Abs(distance.x) < tapRange && Mathf.Abs(distance.y) < tapRange && endTouchPosition.y < Screen.height * 0.86 && !moving)
                {
                    currentAmtromino.transform.Rotate(0f, 0f, 90f);
                    if (!IsValidPosition())
                    {
                        currentAmtromino.transform.Rotate(0f, 0f, -90f);
                    }
                }
                else if (distance.y < -hardDropSensitivity && Mathf.Abs(distance.y) > Mathf.Abs(distance.x) && Time.time < touchTime)
                {
                    HardDrop();
                }
                moving = false;
                movingDown = false;
            }
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
        MenuHandler[] menuHandlers = FindObjectsOfType<MenuHandler>();
        foreach (MenuHandler menuHandler in menuHandlers)
        {
            if (menuHandler.type == MenuHandler.MenuType.GameOver)
            {
                menuHandler.OpenMenu();

                Scoreboard scoreboard = FindObjectOfType<Scoreboard>();
                GameOverMenu.instance.scoreValue.text = scoreboard.score.ToString();
                print("test");
                if (scoreboard.score > 0)
                {
                    print("test2");
                    LeaderboardManager.instance.AddScore(scoreboard.score);
                }

                break;
            }
        }
    }
}
