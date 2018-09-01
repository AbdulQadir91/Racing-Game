using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuScript_Handler : MonoBehaviour
{
	[Header("-------- LOADING SCREEEN OBJECT --------")]
	public GameObject loadingScreen;
	[Header("-------- MAIN MENU OBJECT --------")]
	public GameObject mainMenu;
	public GameObject removeAdsButton,unlockAllButton,unlockNextLevelRewardedButton,inAppSheet,restoreButton;
	[Header("-------- LEVEL SELECTION OBJECT --------")]
	public GameObject levelSelection;
	[Header("-------- LEVELS PARENT --------")]
	public GameObject levels_Parent;
	[Header("No need to assign array populated on runtime")]
	[SerializeField]
	private GameObject[] totalLevels; 
	public int totalLevelsUnlocked;
	public bool inLevelSelection;
	[Header("Clear Data - ENABLED ONLY IN EDITOR")]
	public bool clearData;
	public static MenuScript_Handler msh;

	void Start ()
	{
		if (msh == null) {
			msh = this;
		}
		Time.timeScale = 1f;
		#if UNITY_EDITOR
			if (clearData) {
				PlayerPrefs.DeleteAll();	
			}
		#endif

		StartCoroutine (delayed_LoadingScreenEnabler_Disabler());
		totalLevels = new GameObject[levels_Parent.transform.childCount];
		for (int i = 0; i < levels_Parent.transform.childCount; i++) 
		{
			totalLevels [i] = levels_Parent.transform.GetChild (i).gameObject;
		}
		#if UNITY_ANDROID
			restoreButton.SetActive(false);
		#elif !UNITY_ANDROID
			restoreButton.SetActive(true);
		#endif
		AdsManagerHandler.adsManager.displayBannerAd ();
		#if !UNITY_EDITOR
			AdsManagerHandler.adsManager.recordLogs ("User Started Playing the Game");
		#endif
		if (AdsManagerHandler.adsManager.removeAdsCheck()) {
			removeAdsButton.SetActive (false);
		}
		else {
			removeAdsButton.SetActive (true);
		}
		SoundManager_Handler.soundManagerInstance.playMusic_MainMenu ();
	}

	void OnEnable()
	{
		AdsManagerHandler.onRewardedAdShown += AdsManagerHandler_onRewardedAdShown;
		InAppManager.onPurchaseWasSucessFull += InAppManager_onPurchaseWasSucessFull;
		InAppManager.onPurchaseWasUnSucessFull += InAppManager_onPurchaseWasUnSucessFull;
	}

	void OnDisable()
	{
		AdsManagerHandler.onRewardedAdShown -= AdsManagerHandler_onRewardedAdShown;
		InAppManager.onPurchaseWasSucessFull -= InAppManager_onPurchaseWasSucessFull;
		InAppManager.onPurchaseWasUnSucessFull -= InAppManager_onPurchaseWasUnSucessFull;
	}

	IEnumerator delayed_LoadingScreenEnabler_Disabler()
	{
		loadingScreen.SetActive (true);
		yield return new WaitForSeconds (2f);
		loadingScreen.SetActive (false);
		mainMenu.SetActive (true);
		#if !UNITY_EDITOR
			if(PlayerPrefs.GetInt ("showAds_backMM") == 1)
				AdsManagerHandler.adsManager.showAd (0);
		#endif
	}

	public void displayExitDialoug()
	{
		AdsManagerHandler.adsManager.showExitMenu ();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !inLevelSelection) {
			AdsManagerHandler.adsManager.showExitMenu();
		}
		if (Input.GetKeyDown(KeyCode.Escape) && inLevelSelection) {
			backTO_MainMenu ();
		}
	}
		
	public void playButton()
	{  
		mainMenu.SetActive (false);
		intialize_levelSelection_Levels ();
		levelSelection.SetActive (true);
	}

	public void privacyPolicy()
	{
		Application.OpenURL (AdsManagerHandler.adsManager.privacyPolicy());
	}

	public void buttonClickSound()
	{
		SoundManager_Handler.soundManagerInstance.playButtonClickSound ();
	}

	public void moreButton()
	{
		AdsManagerHandler.adsManager.openMoreApps ();
		#if !UNITY_EDITOR
			AdsManagerHandler.adsManager.recordLogs ("User clicked on More Apps Button on Main Menu");
		#endif
	}

	void intialize_levelSelection_Levels()
	{
		inLevelSelection = true;
		totalLevelsUnlocked = PlayerPrefs.GetInt ("TotalLevel_Unlocked",1);
		for (int i = 0; i < totalLevelsUnlocked; i++) 
		{
			totalLevels [i].GetComponent<Button> ().interactable = true;
		}
		if (totalLevelsUnlocked == totalLevels.Length) {
			unlockAllButton.SetActive (false);
			unlockNextLevelRewardedButton.SetActive (false);
		}
	}

	public void levelNumber(int levelNum)
	{
		loadingScreen.SetActive (true);
		PlayerPrefs.SetInt ("SelectedLevel",levelNum);
		#if !UNITY_EDITOR
			AdsManagerHandler.adsManager.recordLogs ("User Started Clicked on Level Number: "+levelNum);
		#endif
		SoundManager_Handler.soundManagerInstance.playButtonClickSound ();
		SceneManager.LoadScene("Level1");
	}

	public void backTO_MainMenu()
	{
		mainMenu.SetActive (true);
		levelSelection.SetActive (false);
	}

	public void rateUsButton()
	{
		AdsManagerHandler.adsManager.showRateUsDialoug ();
		#if !UNITY_EDITOR
			AdsManagerHandler.adsManager.recordLogs ("User clicked on Rate Us Button on Main Menu");
		#endif
	}

	public void rewarededVideo()
	{
		AdsManagerHandler.adsManager.showRewardedVideoAd ();
	}

	public void removeAds()
	{
		#if !UNITY_ANDROID
		inAppSheet.SetActive (true);
		#endif
		InAppManager.inAppManager.buyInAppAgainstID (InAppManager.inAppManager.inApps[0].id);
	}

	public void unlockAll_Button()
	{
		#if !UNITY_ANDROID
		inAppSheet.SetActive (true);
		#endif
		InAppManager.inAppManager.buyInAppAgainstID (InAppManager.inAppManager.inApps[1].id);
	}

	public void restore()
	{
		InAppManager.inAppManager.RestorePurchases ();
	}

	void InAppManager_onPurchaseWasUnSucessFull ()
	{
		Debug.Log ("INAPP failed Please Try Again - LOG");
		AdsManagerHandler.adsManager.recordLogs ("Could not buy inapps");
		#if !UNITY_ANDROID
			inAppSheet.SetActive (false);
		#endif
	}

	void InAppManager_onPurchaseWasSucessFull (string temp)
	{
		if (temp.Equals(InAppManager.inAppManager.inApps[0].id)) {
			Debug.Log ("User bought Remove Ads - INAPP - LOG");
			AdsManagerHandler.adsManager.recordLogs ("User bought Remove Ads - INAPP");
			AdsManagerHandler.adsManager.removeAdsEvent ();
			removeAdsButton.SetActive (false);
		}
		else if (temp.Equals(InAppManager.inAppManager.inApps[1].id)) {
			Debug.Log ("User bought Remove Ads - INAPP - LOG");
			AdsManagerHandler.adsManager.recordLogs ("User bought Unlock All levels - INAPP");
			PlayerPrefs.SetInt ("TotalLevel_Unlocked",totalLevels.Length);
			unlockNextLevelRewardedButton.SetActive (false);
			unlockAllButton.SetActive (false);
		}
		#if !UNITY_ANDROID
			inAppSheet.SetActive (false);
		#endif
	}

	void AdsManagerHandler_onRewardedAdShown ()
	{
		Debug.Log ("User unlocked a level from viewing a rewarded video ad");
		AdsManagerHandler.adsManager.recordLogs ("User unlocked a level from viewing a rewarded video ad");
		PlayerPrefs.SetInt ("TotalLevel_Unlocked",PlayerPrefs.GetInt ("TotalLevel_Unlocked",1) + 1);
		intialize_levelSelection_Levels ();
		if (PlayerPrefs.GetInt ("TotalLevel_Unlocked",1) == totalLevels.Length) {
			unlockNextLevelRewardedButton.SetActive (false);
			unlockAllButton.SetActive (false);
		}
	}

}
