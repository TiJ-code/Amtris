public class LeaderboardScore
{
    public string playerId { get; set; }
    public string playerName { get; set; }
    public int rank { get; set; }
    public double score { get; set; }
    public Metadata metadata { get; set; }
}

public class LeaderboardScoreboard
{
    public int limit { get; set; }
    public int total { get; set; }
    public LeaderboardScore[] results { get; set; }
}

public class Metadata
{
    public string username { get; set; }
}
