using System;
using System.Collections.Generic;
using Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{ 
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = Resources.Load<GameObject>("GameManager");
                _instance = go.GetComponent<GameManager>();
                DontDestroyOnLoad(go);
            }
            
            return _instance;
        }
    }

    [SerializeField] private List<String> _scenes;

    private int _curLevel = 0;

    public Agent possessedAgent;

    private int enemyCount;

    public void NotifyEnemyKilled()
    {
        enemyCount--;

            Debug.Log("enemy killed. left: " + enemyCount);
        if(enemyCount == 0)
        {
            LoadNextLevel();
        }
    }

    public void RegisterEnemySpawn()
    {
        enemyCount++;
    }

    public void LoadNextLevel()
    {
            Debug.Log("trying to load new scene ");
        if(_curLevel + 1 < _scenes.Count)
        {
            _curLevel++;
            SceneManager.LoadSceneAsync(_scenes[_curLevel]);
            Debug.Log("Loading new level: " + _scenes[_curLevel]);
        }
    }

    public void RestartLevel()
    {
        // TODO: "you died" screen with delay
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void ReturnToMainMenu()
    {
        _curLevel = 0;
        SceneManager.LoadSceneAsync(_scenes[_curLevel]);
    }

    void Awake()
    {
        enemyCount = 0;
    }
}
