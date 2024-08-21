using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup alphaPanel;

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
