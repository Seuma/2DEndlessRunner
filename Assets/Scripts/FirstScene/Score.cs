using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private Text _text;

    private GameObject _player;

    private ScoreData _saver;

    private Vector3 _playerStartPos;
    
    void Awake()
    {
        _player = transform.parent.gameObject;
        _saver = GameObject.Find("/ScoreSaver").GetComponent<ScoreData>();
        _playerStartPos = _player.transform.position;
        _text = transform.GetChild(0).GetComponent<Text>();
        //_score = 0;
        _saver.Load();
        UpdateScoreText();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
    }

    private void UpdateScore()
    {
        int _newScore = (int) (_player.transform.position.x - _playerStartPos.x);

        if (_newScore > _saver.GetScore())
        {
            _saver.SetScore(_newScore);
            UpdateScoreText();
        }
    }

    private void UpdateScoreText()
    {
        _text.text = "Score: " + _saver.GetScore() + "\nCoins: " + _saver.GetCoins();
        
    }
}
