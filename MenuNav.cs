using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private AudioManager audioManager;
    private void Start()
    {
        audioManager = AudioManager.instance;
    }
    public void OnPlayButtonClicked()
    {
        audioManager.PlaySoundEffect(8);
        SceneManager.LoadScene("LevelTest");
    }

    public void OnMainMenuButtonClicked()
    {
        audioManager.PlaySoundEffect(8);
        SceneManager.LoadScene("MainMenu");
    }

    public void OnQuitButtonClicked()
    {
        audioManager.PlaySoundEffect(8);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
