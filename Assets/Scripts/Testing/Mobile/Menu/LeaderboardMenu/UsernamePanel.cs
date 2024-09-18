using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UsernamePanel : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField usernameInputText;
    [SerializeField]
    private Button applyButton;

    private CanvasGroup entryMenuAlpha;

    private bool first_launch = true;

    [SerializeField]
    private Color applyColour;
    [SerializeField]
    private Color notApplyColour;

    public void Apply()
    {
        PlayerPrefs.SetString("username", usernameInputText.text);
        PlayerPrefs.Save();

        entryMenuAlpha.LeanAlpha(0f, 1f)
            .setEaseInOutQuad()
            .setOnComplete(() => {
                entryMenuAlpha.interactable = false;
                entryMenuAlpha.blocksRaycasts = false;
            });
    }

    private void Awake()
    {
        entryMenuAlpha = GetComponent<CanvasGroup>();

        int first_launch_int = PlayerPrefs.GetInt("first_launch", 1);
        first_launch = (first_launch_int == 1);

        if (first_launch)
        {
            entryMenuAlpha.alpha = 1f;
            entryMenuAlpha.interactable = true;
            entryMenuAlpha.blocksRaycasts = true;
            UpdateButton();

            first_launch = false;
            PlayerPrefs.SetInt("first_launch", 0);
            PlayerPrefs.Save();
        }
        else
        {
            entryMenuAlpha.alpha = 0f;
            entryMenuAlpha.interactable = false;
            entryMenuAlpha.blocksRaycasts = false;
            first_launch = false;
        }
    }

    public void UpdateButton()
    {
        if (usernameInputText.text.Length >= 3)
        {
            applyButton.interactable = true;
            applyButton.GetComponent<Image>().color = applyColour;
        }
        else
        {
            applyButton.interactable = false;
            applyButton.GetComponent<Image>().color = notApplyColour;
        }
    }
}
