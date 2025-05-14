using System.Collections.Generic;
using UnityEngine;

public static class ScoreManager
{
    public static Dictionary<string, int> previousScores = new Dictionary<string, int>
    {
        { "Demo", 0 },
        { "Level1", 0 },
        { "Level2", 0 },
        { "Level3", 0 }
    };
}
