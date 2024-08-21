using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public static GameOverMenu instance;

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    [SerializeField]
    private CanvasGroup alphaPanel;
    [SerializeField]
    public TMP_Text scoreValue;

    public void PlayAgain()
    {
        alphaPanel.LeanAlpha(1f, 1f)
            .setEaseInOutQuad()
            .setOnComplete(() => SceneManager.LoadScene(1));
    }

    public void SubmitScore()
    {

    }

    public void BackToMenu()
    {
        alphaPanel.LeanAlpha(1f, 1f)
            .setEaseInOutQuad()
            .setOnComplete(() => SceneManager.LoadScene(0));
    }

    public void OpenLeaderboard()
    {

    }
}
