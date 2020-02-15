using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene("Sandbox", LoadSceneMode.Single);
        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
