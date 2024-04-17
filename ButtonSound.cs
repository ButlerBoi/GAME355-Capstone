using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSOund : MonoBehaviour
{
    public AudioClip buttonClickSound;

    private Button button;
    private AudioSource audioSource;

    void Start()
    {
        button = GetComponent<Button>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = buttonClickSound;

        button.onClick.AddListener(PlayButtonClickSound);
    }

    private void PlayButtonClickSound()
    {
        if (buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}
