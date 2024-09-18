using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public static LoadGame instance { get; private set; }

    public CanvasGroup alphaChannel;

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

    public void LoadGameScene()
    {
        LeanTween.value(0f, 1f, 1f)
            .setOnUpdate((float value) =>
            {
                alphaChannel.alpha = value;
            })
            .setEaseInOutQuad()
            .setOnComplete(() => LoadScene());
           // .setOnComplete(() => print("load"));
    }

    private void LoadScene()
    {
        SceneManager.LoadScene("Game");
    }
}
