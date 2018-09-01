using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager_Handler : MonoBehaviour {

	[Header("Assign AudioClips from the projects here")]
	[SerializeField]
	private AudioClip mainMenuSound;
	[SerializeField]
	private AudioClip gamePlaySound,buttonClicksSound,levelCompleteSound,levelFailedSound;

	[Header("Assign AudioSources here ** Do not temper with these variables ** ")]
	[SerializeField]
	private AudioSource musicAudioSource;
	[SerializeField]
	private AudioSource buttonClickAudioSource,levelComplete_FailAudioSource;

	public static SoundManager_Handler soundManagerInstance;

	void Awake()
	{
		if (soundManagerInstance == null) {
			soundManagerInstance = this;
			DontDestroyOnLoad (this.gameObject);
		} 
		else {
			Destroy (this.gameObject);	
		}
	}

	public void playMusic_MainMenu()
	{
		musicAudioSource.clip = mainMenuSound;
		musicAudioSource.Play ();
	}

	public void playMusic_Gameplay()
	{
		musicAudioSource.clip = gamePlaySound;
		musicAudioSource.Play ();
	}

	public void stopMusic_Gameplay()
	{
		musicAudioSource.Stop ();
	}

	public void resumeMusic_Gameplay()
	{
		musicAudioSource.Play ();
	}

	public void changeMusic_Volume(float temp)
	{
		musicAudioSource.volume = temp;
	}

	public void playButtonClickSound()
	{
		if (buttonClickAudioSource.clip == null)
			buttonClickAudioSource.clip = buttonClicksSound;

		buttonClickAudioSource.Play ();
	}

	public void playlevelCompleteSound()
	{
		levelComplete_FailAudioSource.clip = levelCompleteSound;
		levelComplete_FailAudioSource.Play ();
	}

	public void playlevelFailSound()
	{
		levelComplete_FailAudioSource.clip = levelFailedSound;
		levelComplete_FailAudioSource.Play ();
	}

}
