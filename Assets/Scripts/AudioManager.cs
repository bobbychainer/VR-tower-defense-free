using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("-------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;


    [Header("-------- Audio Clips ----------")]
    public AudioClip background;
    public AudioClip attackPhase;
    public AudioClip buttonClick;
    public AudioClip playerShootBullet;
    public AudioClip playerGettingHurt;
    public AudioClip enemyDeadSound;
    public AudioClip gameOverSound;
    public AudioClip gameOverVoice;
    public AudioClip enemyShootingBullet;
    public AudioClip coinsSound;

    private void Start(){

        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clipname){
        SFXSource.PlayOneShot(clipname);
    }

    public void PlayButtonSound(){
        SFXSource.PlayOneShot(buttonClick);
    }

    public void ChangeBGM(AudioClip music){
        musicSource.Stop();
        musicSource.clip = music;
        musicSource.Play();

    }
}
