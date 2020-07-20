using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class WorldSpawn : MonoBehaviour
{
    [SerializeField] private GameObject groundTexture;

    [SerializeField] private GameObject undergroundTexture;

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private float destroyTime = 1f;

    [SerializeField] private float createTime = 1f;

    private const int MaxHeight = 40;

    private const int MinHeightMovable = -20;

    private const int MinHeight = -40;

    private float _destroyTimer, _createTimer, _delayedDestroy;
    
    private List<GameObject> _grounds;

    private Vector3 _lastPos;

    private GameObject _player;

    private const float TransitionToHigherCreateTime = 15;

    private Dictionary<float, List<GameObject>> _undergrounds;
    
    private short _gapCounter = 0;

    private GameObject _saver;

    private CutsceneManager _CutsceneManager;

    // Start is called before the first frame update
    void Start()
    {
        _CutsceneManager = GameObject.Find("/CutsceneManager").GetComponent<CutsceneManager>();
        _saver = GameObject.Find("ScoreSaver");
        _destroyTimer = destroyTime;
        _createTimer = createTime;
        _undergrounds = new Dictionary<float, List<GameObject>>();
        
        _lastPos = new Vector3(0, 0, 0.1f);
        
        _grounds = new List<GameObject>();
        for (int startCounter = 0; startCounter < 10; startCounter++)
        {
            
            if (startCounter == 5)
            {
                SpawnGround(true);
                _player = Instantiate(playerPrefab, new Vector3(_lastPos.x, _lastPos.y + 5), Quaternion.identity);
            }
            else
            {
                SpawnGround();    
            }
        }
    }

    private void SpawnGround(bool isWithPlayer = false)
    {
        Vector3 newPos = _lastPos;
        newPos.x += 1;

        int random = Random.Range(1, 4);
        if ((random == 1) && (newPos.y < MaxHeight))
        {
            newPos.y += 1;
        } else if ((random == 2) && (newPos.y > MinHeightMovable))
        {
            newPos.y -= 1;
        }

        if ((random == 3 && _gapCounter <= 2) && !isWithPlayer)
        {
            _gapCounter++;
        }
        else
        {
            _gapCounter = 0;
            SpawnUnderground(newPos);
            _grounds.Add(Instantiate(groundTexture, newPos, Quaternion.identity));
        }
        _lastPos = newPos;
    }

    private void SpawnUnderground(Vector3 newPos)
    {
        List<GameObject> ugrounds = new List<GameObject>();
            
        for (int undergroundCounter = (int)newPos.y - 1; undergroundCounter > MinHeight; undergroundCounter--)
        {
            Vector3 ugroundPos = new Vector3(newPos.x, undergroundCounter);
            ugrounds.Add(Instantiate(undergroundTexture, ugroundPos, Quaternion.identity));
        }
            
        _undergrounds.Add(newPos.x, ugrounds);
    }
    
    private void Update()
    {
        if (_player.transform.position.y <= MinHeightMovable)
        {
            if (_saver != null)
            {
                _saver.GetComponent<ScoreData>().Save();
            }

            _CutsceneManager.AddDeath();
            //SceneManager.LoadScene("Scenes/MainGame");
        }
        if ((_grounds[_grounds.Count-1].transform.position.x - _player.transform.position.x) > TransitionToHigherCreateTime )
        {
            createTime = destroyTime;
        }
        else
        {
            createTime = 0.1f;
        }

        if (((_grounds[_grounds.Count - 1].transform.position.x - _player.transform.position.x) <=
             (TransitionToHigherCreateTime * 2)) && (_player.GetComponent<CharacerController2D>().GameStarted))
        {
            _createTimer -= Time.deltaTime;
            if (_createTimer <= 0f)
            {
                _createTimer = createTime;
                SpawnGround();
            }
        }

        if (_player.GetComponent<CharacerController2D>().GameStarted)
        {
            _delayedDestroy += Time.deltaTime;
            if (_delayedDestroy >= 2f)
            {
                _delayedDestroy = 2f;
                _destroyTimer -= Time.deltaTime;
                if (_destroyTimer <= 0f)
                {
                    _destroyTimer = destroyTime;
                    DestroyGround();
                }
            }
        }
    }

    private void DestroyGround()
    {
        GameObject groundTodestroy = _grounds[0];
        _grounds.RemoveAt(0);
        List<GameObject> ugrounds;
        if (_undergrounds.TryGetValue(groundTodestroy.transform.position.x, out ugrounds))
        {
            foreach (var variable in ugrounds)
            {
                Destroy(variable);
            }
            
            _undergrounds.Remove(groundTodestroy.transform.position.x);
        }
        Destroy(groundTodestroy);
    }
}
