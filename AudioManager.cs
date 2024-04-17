using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource soundEffectSource;

    public AudioClip[] soundEffects;

    public const int PLAYERHIT_SOUND = 0;
    public const int ENEMYHIT_SOUND = 1;
    public const int ATTACK_SOUND = 2;
    public const int COIN_SOUND = 3;
    public const int HEALTH_SOUND = 4;
    public const int SPEED_SOUND = 5;
    public const int ENEMYDEATH_SOUND = 6;
    public const int CHEST_SOUND = 7;
    public const int BUTTON_SOUND = 8;

    void Awake()
    {
        // Ensure only one instance of AudioManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySoundEffect(int index)
    {
        if (index >= 0 && index < soundEffects.Length)
        {
            soundEffectSource.PlayOneShot(soundEffects[index]);
        }
        else
        {
            Debug.LogWarning("Invalid sound effect index.");
        }
    }

    public void StopSoundEffect()
    {
        soundEffectSource.Stop();
    }

    public void SetSoundEffectVolume(float volume)
    {
        soundEffectSource.volume = volume;
    }
}
