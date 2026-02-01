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
                GameObject go = Instantiate(Resources.Load<GameObject>("GameManager"), null);
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
        if(_curLevel + 1 < _scenes.Count)
        {
            _curLevel++;
            SceneManager.LoadSceneAsync(_scenes[_curLevel]);
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
