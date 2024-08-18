using TMPro;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    public int score { get; private set; } = 0;
    public int highscore { get; private set; } = 0;

    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private TMP_Text highscoreText;

    private void OnEnable()
    {
        highscore = PlayerPrefs.GetInt("highscore", 0);
        UpdateTextFields();
    }

    public void AddLineCleared()
    {
        score += 100;
        SaveScoreIfNecessary();
        UpdateTextFields();
    }

    private void SaveScoreIfNecessary()
    {
        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
        }
    }

    private void UpdateTextFields()
    {
        scoreText.text = score.ToString();
        highscoreText.text = highscore.ToString();
    }
}
