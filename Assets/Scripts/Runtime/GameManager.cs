using System;
using System.Collections.Generic;
using Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _gameManager;

    [SerializeField] private List<String> _scenes;

    private int _curLevel = 0;

    public Agent possessedAgent;


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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
