using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LeaderboardDisplay : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> rankEntries = new List<GameObject>();
    [SerializeField]
    private GameObject playerRank;

    private LeaderboardScoreboard lScoreboard;
    private LeaderboardScore lPlayer;

    private async void Awake()
    {
        lScoreboard = await LeaderboardManager.instance.GetScores();

        for (int i = 0; i < lScoreboard.results.Length; i++)
        {
            var rankEntry = rankEntries[i];
            TMP_Text[] components = rankEntry.GetComponentsInChildren<TMP_Text>();
            TMP_Text _playername = components.FirstOrDefault(text => text.name.ContainsInsensitive("playername"));
            TMP_Text _score = components.FirstOrDefault(text => text.name.ContainsInsensitive("score"));
            _playername.text = lScoreboard.results[i].metadata.username;
            _score.text = lScoreboard.results[i].score.ToString();
        }

        lPlayer = await LeaderboardManager.instance.GetPlayerscore();
        TMP_Text[] playerComponents = playerRank.GetComponentsInChildren<TMP_Text>();
        TMP_Text playername = playerComponents.FirstOrDefault(text => text.name.ContainsInsensitive("playername"));
        TMP_Text score = playerComponents.FirstOrDefault(text => text.name.ContainsInsensitive("score"));
        TMP_Text rank = playerComponents.FirstOrDefault(text => text.name.ContainsInsensitive("rank"));
        playername.text = lPlayer.metadata.username;
        score.text = lPlayer.score.ToString();
        rank.text = (lPlayer.rank + 1).ToString();
    }
}
