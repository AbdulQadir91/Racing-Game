using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GamePlay_Script_Handler : MonoBehaviour
{
	[Header("-------- LOADING IMAGE --------")]
	public GameObject loadingScreen;

	[Header("-------- GAMEPLAY BUTTONS --------")]
	public GameObject gp1;
	public GameObject gp2,gp3;

	[Header("-------- PAUSE GAME DAILOUG --------")]
	public GameObject pauseGame;

	[Header("-------- LEVEL FAIL DAILOUG --------")]
	public GameObject levelFail_Dialoug;

	[Header("-------- LEVEL COMPLETE DAILOUG --------")]
	public GameObject levelComplete_Dialoug;

	[Header("-------- LEVEL OBJECTIVES DAILOUG --------")]
	public GameObject objectiveDialoug;
	public Text levelObjectives;

	[Header("-------- OTHER GAMEPLAY ELEMENTS --------")]
	public GameObject levelsParentObject;
	[Header("No need to assign array populated on runtime")]
	[SerializeField]
	private GameObject[] total_Levels;
	public GameObject skipButton;
	public bool level_Complete_fail_event,displayAds_Event,levelComplete_test,levelFail_test,showingPrimaryObjectives;
	public int selectedLevel_Prefs,levelUnlocked_Prefs;
	//---------------------------- Other Stuff END ------------------------//
	public static GamePlay_Script_Handler gsh;

	// Use this for initialization
	void Awake()
	{
		if (gsh == null) {
			gsh = this;
		}
		if (SoundManager_Handler.soundManagerInstance != null) {
			SoundManager_Handler.soundManagerInstance.playMusic_Gameplay ();
			SoundManager_Handler.soundManagerInstance.changeMusic_Volume (0.5f);
		}
		StartCoroutine (delayed_LoadingScreenEnabler_Disabler());
		Time.timeScale = 1f;

		#if !UNITY_EDITOR
			AdsManagerHandler.adsManager.destroyBannerAd ();
			selectedLevel_Prefs = PlayerPrefs.GetInt ("SelectedLevel", 1);
			levelUnlocked_Prefs = PlayerPrefs.GetInt ("TotalLevel_Unlocked", 1);
			AdsManagerHandler.adsManager.recordLogs ("User started playing Level Number: "+selectedLevel_Prefs);
		#endif

		total_Levels = new GameObject[levelsParentObject.transform.childCount];
		for (int i = 0; i < total_Levels.Length; i++) {
			total_Levels [i] = levelsParentObject.transform.GetChild (i).gameObject;
		}
		if (selectedLevel_Prefs <= total_Levels.Length) 
		{
			totalLevels_Unlocker (selectedLevel_Prefs);
		}
		PlayerPrefs.SetInt ("showAds_backMM",1);
	}

	void totalLevels_Unlocker(int levelNumber)
	{
		total_Levels [levelNumber].SetActive (true);
	}

	public LevelMode_Handler returnLevelModelHandler()
	{
		return total_Levels [selectedLevel_Prefs].GetComponent<LevelMode_Handler> ();
	}
		
	IEnumerator delayed_LoadingScreenEnabler_Disabler()
	{
		loadingScreen.SetActive (true);
		yield return new WaitForSeconds (2f);
		loadingScreen.SetActive (false);
		if (returnLevelModelHandler().hasCutScene) {
			StartCoroutine (returnLevelModelHandler().cutSceneInit());
			StartCoroutine (showSkipButton());
		} 
		else {
			initLevelObjectives ();
		}
	}

	IEnumerator showSkipButton()
	{
		yield return new WaitForSeconds ((returnLevelModelHandler ().cutSceneTime / 2));
		skipButton.SetActive (true);
	}

	public void initLevelObjectives()
	{
		objectiveDialoug.SetActive (true);
		levelObjectives.text = returnLevelModelHandler ().levelPrimaryObjectives;
	}

	public void initSecondaryObjectives(int temp)
	{
		objectiveDialoug.SetActive (true);
		levelObjectives.text = returnLevelModelHandler ().levelSecondaryObjectives[temp];
	}

	public void skipCutScene()
	{
		returnLevelModelHandler ().skipCutScene = true;
		returnLevelModelHandler ().skipCutScene_Event ();
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			pauseButton ();
		}
		#if UNITY_EDITOR
		if (levelComplete_test) 
		{
			levelComplete_test = false;
			levelComplete_EventCall ();
		}
		if (levelFail_test) 
		{
			levelFail_test = false;
			levelFail_EventCall ();
		}
		#endif
	}

	public void callingDelayLevelComplete()
	{
		StartCoroutine (delayLevelComplete_Fail(true));
	}
	public void callingDelayLevelFail()
	{
		StartCoroutine (delayLevelComplete_Fail(false));
	}

	IEnumerator delayLevelComplete_Fail(bool temp)
	{
		hideAll_gpButtons ();
		yield return new WaitForSeconds (4f);
		if (temp) {
			levelComplete_EventCall ();
		}
		else if (!temp) {
			levelFail_EventCall ();
		}
	}

	public void buttonClickSound()
	{
		if (SoundManager_Handler.soundManagerInstance != null)
			SoundManager_Handler.soundManagerInstance.playButtonClickSound ();
	}

	public void okButton()
	{
		if (!showingPrimaryObjectives) {
			showingPrimaryObjectives = true;
		}
		objectiveDialoug.SetActive (false);
	}

	public void hideAll_gpButtons()
	{
		gp1.SetActive (false);
		gp2.SetActive (false);
		gp3.SetActive (false);
	}

	public void showAll_gpButtons()
	{
		gp1.SetActive (true);
		gp2.SetActive (true);
		gp3.SetActive (true);
	}

	public void pauseButton()
	{
		hideAll_gpButtons ();
		pauseGame.SetActive (true);
		if (SoundManager_Handler.soundManagerInstance != null)
			SoundManager_Handler.soundManagerInstance.stopMusic_Gameplay ();
		Time.timeScale = 0.0000001f;
		StartCoroutine (delayedAdCall_Pause());
	}

	IEnumerator delayedAdCall_Pause()
	{
		yield return new WaitForSeconds (0.01f);
		#if !UNITY_EDITOR
			AdsManagerHandler.adsManager.showAd (3);
		#endif
	}

	public void resumeButton()
	{
		showAll_gpButtons ();
		Time.timeScale = 1f;
		if (SoundManager_Handler.soundManagerInstance != null)
			SoundManager_Handler.soundManagerInstance.resumeMusic_Gameplay ();
		pauseGame.SetActive (false);
	}

	public void restartButton()
	{
		loadingScreen.SetActive (true);
		#if !UNITY_EDITOR
			AdsManagerHandler.adsManager.recordLogs ("User Restarted Level Number: "+selectedLevel_Prefs);
		#endif
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
	}

	public void homeButton()
	{
		loadingScreen.SetActive (true);
		#if !UNITY_EDITOR
			AdsManagerHandler.adsManager.recordLogs ("User went to Main Menu from Level Number: "+selectedLevel_Prefs);
		#endif
		SceneManager.LoadScene (0);
	}

	public void levelFail_EventCall()
	{
		if (!level_Complete_fail_event)
		{
			level_Complete_fail_event = true;
			levelFail_Dialoug.SetActive (true);
			if (SoundManager_Handler.soundManagerInstance != null) {
				SoundManager_Handler.soundManagerInstance.stopMusic_Gameplay ();
				SoundManager_Handler.soundManagerInstance.playlevelFailSound ();
			}
			#if !UNITY_EDITOR
				AdsManagerHandler.adsManager.recordLogs ("User Failed Level Number: "+selectedLevel_Prefs);
			#endif
			Time.timeScale = 0.0000001f;
			showAds (false);
		}
	}

	public void levelComplete_EventCall()
	{
		if (!level_Complete_fail_event)
		{
			level_Complete_fail_event = true;
			levelComplete_Dialoug.SetActive (true);
			if (SoundManager_Handler.soundManagerInstance != null) {
				SoundManager_Handler.soundManagerInstance.stopMusic_Gameplay ();
				SoundManager_Handler.soundManagerInstance.playlevelCompleteSound ();
			}
			#if !UNITY_EDITOR
				AdsManagerHandler.adsManager.recordLogs ("User Completed Level Number: "+selectedLevel_Prefs);
			#endif
			Time.timeScale = 0.0000001f;
			if (PlayerPrefs.GetInt ("SelectedLevel", 1) == PlayerPrefs.GetInt ("TotalLevel_Unlocked", 1)) 
			{
				PlayerPrefs.SetInt ("TotalLevel_Unlocked", PlayerPrefs.GetInt ("TotalLevel_Unlocked", 1) + 1);
			}
			showAds (true);
		}
	}

	public void nextButton()
	{
		Time.timeScale = 1f;
		loadingScreen.SetActive (true);
		if (selectedLevel_Prefs < (total_Levels.Length - 1)) 
		{
			PlayerPrefs.SetInt ("SelectedLevel", PlayerPrefs.GetInt ("SelectedLevel", 1) + 1);
			restartButton ();
		}
		else
		{
			#if !UNITY_EDITOR
				AdsManagerHandler.adsManager.recordLogs ("User Completed the full Game going to Main Menu");
			#endif
			SceneManager.LoadScene (0);
		}

	}

	void showAds(bool temp)
	{
		if (!displayAds_Event)
		{
			displayAds_Event = true;
			if (temp) {
				#if !UNITY_EDITOR
					StartCoroutine (delayedAdCall_Complete());
				#endif
			}
			else if (!temp) {
				#if !UNITY_EDITOR
					StartCoroutine (delayedAdCall_Fail());
				#endif
			}
		}
	}

	IEnumerator delayedAdCall_Fail()
	{
		yield return new WaitForSeconds (0.01f);
		#if !UNITY_EDITOR
			AdsManagerHandler.adsManager.showAd (2);
		#endif
	}

	IEnumerator delayedAdCall_Complete()
	{
		yield return new WaitForSeconds (0.01f);
		#if !UNITY_EDITOR
		AdsManagerHandler.adsManager.showAd (1);
		#endif
	}

}
