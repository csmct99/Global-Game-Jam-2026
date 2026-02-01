using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        // Stops play mode in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quits the built game
        Application.Quit();
#endif
    }
}
