using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public AudioSource canvasAudioSource;
    public Button audioButton;
    private bool isMuted = false;

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Application has quit.");
    }

    public void ToggleSound()
    {
        isMuted = !isMuted;
        canvasAudioSource.mute = isMuted;
    }
}