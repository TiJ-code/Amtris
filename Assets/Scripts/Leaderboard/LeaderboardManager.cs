using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager instance;

    private const string leaderboardId = "GlobalID";

    private async void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;

            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            DontDestroyOnLoad(this);
        }
    }

    public async void AddScore(int score)
    {
        var metadata = new Dictionary<string, string>() 
        {
            { "username", PlayerPrefs.GetString("username", string.Empty) } 
        };
        var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(
            leaderboardId,
            score,
            new AddPlayerScoreOptions { Metadata = metadata }
        );
    }

    public async Task<LeaderboardScore> GetPlayerscore()
    {
        try
        {
            var scoreResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync(
                leaderboardId,
                new GetPlayerScoreOptions { IncludeMetadata = true }
            );

            LeaderboardScore playerScore = new LeaderboardScore();
            playerScore.playerId = scoreResponse.PlayerId;
            playerScore.playerName = scoreResponse.PlayerName;
            playerScore.score = scoreResponse.Score;
            playerScore.rank = scoreResponse.Rank;
            playerScore.metadata = JsonConvert.DeserializeObject<Metadata>(scoreResponse.Metadata);

            return playerScore;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error getting player score: " + ex.Message);
            return null;
        }
    }

    public async Task<LeaderboardScoreboard> GetScores()
    {
        try
        {
            var scoreResponse = await LeaderboardsService.Instance.GetScoresAsync(
                leaderboardId,
                new GetScoresOptions { Limit = 5, IncludeMetadata = true }
            );

            LeaderboardScoreboard scoreboard = new LeaderboardScoreboard();
            scoreboard.limit = scoreResponse.Limit;
            scoreboard.total = scoreResponse.Total;
            scoreboard.results = new LeaderboardScore[scoreResponse.Results.Count];

            for (int i = 0; i < scoreResponse.Results.Count; i++)
            {
                var entry = scoreResponse.Results[i];
                scoreboard.results[i] = new LeaderboardScore();
                scoreboard.results[i].playerId = entry.PlayerId;
                scoreboard.results[i].playerName = entry.PlayerName;
                scoreboard.results[i].rank = entry.Rank;
                scoreboard.results[i].score = entry.Score;

                if (entry.Metadata != null)
                {
                    scoreboard.results[i].metadata = new Metadata();
                    scoreboard.results[i].metadata = JsonConvert.DeserializeObject<Metadata>(entry.Metadata);
                }
            }

            return scoreboard;
        }
        catch (Exception ex)
        {
            // Handle the exception
            Debug.LogError("Error getting scores: " + ex.Message);
            return null; // or throw the exception again
        }
    }

}
