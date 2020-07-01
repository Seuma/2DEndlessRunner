using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int score;
    public int coins;

    public PlayerData(ScoreData data)
    {
        score = data.GetScore();
        coins = data.GetCoins();
    }
}
