using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    private GameObject _Spawn;
    private WorldSpawn _WorldSpawn;

    private bool _isInCutscene = false;
    
    private static CutsceneManager _instance;

    private short _deaths = 0;

    private bool _tutorialWasUsed = false;
    
    public static CutsceneManager instance
    {
        get => _instance;
        set => _instance = value;
    }

    // Update is called once per frame
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }
    
    public IEnumerator StartTutorial()
    {
        if (!_tutorialWasUsed)
        {
            _tutorialWasUsed = true;
            GameObject player = GameObject.FindWithTag("Player");
            SetDialog("[TUTORIAL]");
            player.transform.Find("Canvas").transform.Find("Dialog").gameObject.SetActive(true);
            player.transform.Find("Canvas").transform.Find("Dialog").GetComponent<Text>().fontSize = 20;
            SetDialog("[TUTORIAL]\nPlease use the Arrow keys to control the character.\n" +
                      "Left Arrow: Moves the Character to the left.\n" +
                      "Right Arrow: Moves the Character to the right\n" +
                      "Up Arrow: Let's the Character jump");

            yield return new WaitForSeconds(5);
            player.transform.Find("Canvas").transform.Find("Dialog").gameObject.SetActive(false);
        }
    }

    public void AddDeath()
    {
        _deaths += 1;
        if (_deaths == 5)
        {
            _isInCutscene = true;
            StartCoroutine(StartCutscene());
        }
        else
        {
            SceneManager.LoadScene("Scenes/MainGame");
        }
    }

    private IEnumerator StartCutscene()
    {
        SceneManager.LoadScene("Scenes/MainGame");
        yield return new WaitForSeconds(1);
        GameObject player = GameObject.FindWithTag("Player");
        SetDialog("Stickman: Why... Why am I here?");
        player.transform.Find("Canvas").transform.Find("Dialog").gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        SetDialog("Stickman: H..How did I got..here?");
        yield return  new WaitForSeconds(5);
        SetDialog("Stickman: What Happened to me?\nWhy am I in this game?");
        yield return new WaitForSeconds(5);
        SetDialog("Stickman: I don't know what happened..\nI need to find some answers...\nHow can I even escape this?");
        yield return new WaitForSeconds(5);
        _deaths = 0;
        _isInCutscene = false;
        player.transform.Find("Canvas").transform.Find("Dialog").gameObject.SetActive(false);
        SceneManager.LoadScene("Scenes/UnderDevelopment");
    }

    private void SetDialog(String value)
    {
        GameObject.FindWithTag("Player").transform.Find("Canvas").transform.Find("Dialog").GetComponent<Text>().text = value;
    }
    
    public bool isInCutscene => _isInCutscene;
}
