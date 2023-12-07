using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        GameManager.Instance.NextLevel();
        //SceneManager.LoadScene("Level1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.name);
    }

    public void BTM()
    {
        SceneManager.LoadScene("Level1");

    }

}
