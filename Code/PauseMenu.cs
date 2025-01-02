using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public AudioSource InGameMusic;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        InGameMusic.Pause();
        Time.timeScale = 0;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        InGameMusic.Play();
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
}
