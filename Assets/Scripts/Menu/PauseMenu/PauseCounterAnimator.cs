using System.Collections;
using TMPro;
using UnityEngine;

public class PauseCounterAnimator : MonoBehaviour
{
    public static PauseCounterAnimator instance;

    private TMP_Text counterText;
    [SerializeField]
    private PauseMenu pauseMenu;

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            counterText = GetComponent<TMP_Text>();
            counterText.text = string.Empty;
        }
    }

    public void StartCountdown()
    {
        pauseMenu.Continue(); // Minimise the menu
        counterText.text = "3";
        StartCoroutine(AnimateCountdown());
    }

    private IEnumerator AnimateCountdown()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 3; i > 0; i--)
        {
            counterText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        counterText.text = "Go";
        yield return new WaitForSeconds(0.5f);

        counterText.text = string.Empty;
        pauseMenu.ChangeState(false); // Unpause the game
    }
}