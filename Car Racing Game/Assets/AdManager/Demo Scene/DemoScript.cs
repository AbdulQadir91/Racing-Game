using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoScript : MonoBehaviour{
	public Text demoReward;
	int temp;
	public GameObject removeAdButton_Editor, unlockAllButton_Editor,
	destroyBannerButton_Editor,displayBannerButton_Editor,showAdsButton_Editor;
	public static int recordedLogs;
	public Text text;

	void Start()
	{
		if (AdsManagerHandler.adsManager.removeAdsCheck()) {
			removeAdButton_Editor.SetActive (false);
			destroyBannerButton_Editor.SetActive (false);
			displayBannerButton_Editor.SetActive (false);
			showAdsButton_Editor.SetActive (false);
		}
		if (PlayerPrefs.GetInt ("unlockAll_Levels") == 11) {
			unlockAllButton_Editor.SetActive (false);
		}

		AdsManagerHandler.onRewardedAdShown += rewardUser;
		InAppManager.onPurchaseWasSucessFull += purchaseWasSucessfull_Handler;
		AdsManagerHandler.adsManager.recordLogs("User started Playing the Game");
		AdsManagerHandler.adsManager.recordLogs("User is ready to play the game");

	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Escape)) {
			AdsManagerHandler.adsManager.showExitMenu ();
		}
	}

	void OnDisable()
	{
		AdsManagerHandler.onRewardedAdShown -= rewardUser;
		InAppManager.onPurchaseWasSucessFull -= purchaseWasSucessfull_Handler;
	}

	public void recordLogs()
	{
		AdsManagerHandler.adsManager.recordLogs ("Recording Log Number = "+recordedLogs);
		recordedLogs++;
		text.text = "Recorded Logs: "+recordedLogs;
	}

	public void showAd()
	{
		AdsManagerHandler.adsManager.showAd (1);
	}

	public void showBannerAd()
	{
		AdsManagerHandler.adsManager.displayBannerAd ();
	}

	public void DestroyBannerAd()
	{
		AdsManagerHandler.adsManager.destroyBannerAd ();
	}

	public void showRewardedVideoAd()
	{
		AdsManagerHandler.adsManager.showRewardedVideoAd ();
	}

	public void rewardUser()
	{
		//after rewarded vid shown reward the user
		temp += 50;
		demoReward.text = "Rewarded = "+temp;
	}

	public void rateUs()
	{
		AdsManagerHandler.adsManager.showRateUsDialoug ();
	}

	public void moreApps()
	{
		AdsManagerHandler.adsManager.openMoreApps ();
	}

	public void removeAds_Button()
	{
		InAppManager.inAppManager.buyInAppAgainstID (InAppManager.inAppManager.inApps[0].id);
	}

	public void unlockAll_Button()
	{
		InAppManager.inAppManager.buyInAppAgainstID (InAppManager.inAppManager.inApps[1].id);
	}

	public void purchaseWasSucessfull_Handler(string temp)
	{
		if (temp.Equals(InAppManager.inAppManager.inApps[0].id)) {
			Debug.Log ("Remove all ads");
			AdsManagerHandler.adsManager.removeAdsEvent ();
			removeAdButton_Editor.SetActive (false);
		}
		else if (temp.Equals(InAppManager.inAppManager.inApps[1].id)) {
			Debug.Log ("Unlock All");
			PlayerPrefs.SetInt ("unlockAll_Levels",11); // unlock all levels here now
			unlockAllButton_Editor.SetActive (false);
		}
	}
}
