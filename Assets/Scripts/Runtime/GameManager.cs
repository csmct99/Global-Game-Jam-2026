using System;
using System.Collections.Generic;
using Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tymski;

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

	[SerializeField] private List<SceneReference> _sceneLoadOrder;

	private int _curLevel = 0;

	public Agent possessedAgent;

	private int enemyCount = 0;

	public void NotifyEnemyKilled()
	{
		enemyCount--;

		if(enemyCount <= 0)
		{
			Invoke(nameof(LoadNextLevel), 1.5f);
		}
	}

	public void RegisterEnemySpawn()
	{
		enemyCount++;
	}

	public void LoadNextLevel()
	{
		if(_curLevel + 1 < _sceneLoadOrder.Count)
		{
			_curLevel++;
			enemyCount = 0;
			SceneManager.LoadSceneAsync(_sceneLoadOrder[_curLevel].ScenePath);
		}
	}

	public void RestartLevel()
	{
		// TODO: "you died" screen with delay
		enemyCount = 0;
		string currentSceneName = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(currentSceneName);
	}

	public void ReturnToMainMenu()
	{
		_curLevel = 0;
		SceneManager.LoadSceneAsync(_sceneLoadOrder[_curLevel].ScenePath);
	}

	void Awake()
	{
		enemyCount = 0;

		AdjustToCorrectScene();
	}

	private void AdjustToCorrectScene()
	{
		Scene currentScene = SceneManager.GetActiveScene();
		
		bool wrongLevel = _curLevel >= _sceneLoadOrder.Count || currentScene.path != _sceneLoadOrder[_curLevel].ScenePath;
		if (wrongLevel)
		{
			_curLevel = _sceneLoadOrder.FindLastIndex(s => s.ScenePath == currentScene.path);
			if (_curLevel == -1)
			{
				Debug.Log("Failed to find the correct level, we will start from the beginning of the playlist");
				_curLevel = 0; // Failed to find the correct level
			}
			else
			{
				Debug.Log($"Found the correct level, its index is {_curLevel}. Continuing from there.");
			}
		}
	}
}
