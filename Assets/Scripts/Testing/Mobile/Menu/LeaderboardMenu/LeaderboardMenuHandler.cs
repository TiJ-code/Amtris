using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardMenuHandler : MonoBehaviour
{
    public CanvasGroup alphaChannel;

    public void OpenLeaderboard()
    {
        print("open");
        alphaChannel.blocksRaycasts = true;
        alphaChannel.interactable = true;
        alphaChannel.LeanAlpha(1f, 1.5f)
            .setEaseInOutQuad()
            .setOnComplete(() => SceneManager.LoadScene(2));
    }

    public void CloseLeaderboard()
    {
        print("close");
        alphaChannel.blocksRaycasts = true;
        alphaChannel.interactable = true;
        alphaChannel.LeanAlpha(1f, 1.5f)
            .setEaseInOutQuad()
            .setOnComplete(() => SceneManager.LoadScene(0));
    }
}
