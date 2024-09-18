using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    private MenuHandler menuHandler;
    private GameManager gameManager;
    private Scoreboard scoreboard;

    [SerializeField]
    public TMP_Text scoreValue;
    [SerializeField]
    public PauseCounterAnimator pauseCounterAnimator;

    [SerializeField]
    public TMP_Text pauseButtonText;

    private string[] pauseLetters = { "J", "K" };

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            menuHandler = GetComponent<MenuHandler>();
            gameManager = FindObjectOfType<GameManager>();
            scoreboard = gameManager.GetComponent<Scoreboard>();
        }
    }

    [SerializeField]
    private CanvasGroup alphaPanel;

    public void Continue()
    {
        menuHandler.CloseMenu();
    }

    public void ChangeState(bool state)
    {
        gameManager.isPaused = state;
    }

    public void Restart()
    {
        alphaPanel.LeanAlpha(1f, 1f)
            .setEaseInOutQuad()
            .setOnComplete(() => SceneManager.LoadScene(1));
    }

    public void BackToMenu()
    {
        gameManager.GameOver();
    }

    public void PauseUnpauseButton()
    {
        if (gameManager.isPaused)
        {
            pauseButtonText.text = pauseLetters[0];
            pauseCounterAnimator.StartCountdown();
        }
        else
        {
            gameManager.isPaused = true;
            scoreValue.text = scoreboard.score.ToString();
            menuHandler.OpenMenu();
            pauseButtonText.text = pauseLetters[1];
        }
    }
}
