using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreData : MonoBehaviour
{
    // Start is called before the first frame update
    private static ScoreData _instance;

    public static ScoreData Instance
    {
        get => _instance;
        set => _instance = value;
    }

    private int _score;
    private int _coins;
    
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        } else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetScore(int newScore)
    {
        _score = newScore;
    }

    public int GetScore()
    {
        return _score;
    }

    public void SetCoins(int newCoins)
    {
        _coins = newCoins;
    }

    public int GetCoins()
    {
        return _coins;
    }

    public void Save()
    {
        SaveSystem.SavePlayer(this);
    }

    public void Load()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data != null)
        {
            _score = data.score;
            _coins = data.coins;
        }
    }
}
