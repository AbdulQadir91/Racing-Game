using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.Analytics;
using UnityEngine.Advertisements;
using System;
using AudienceNetwork;

public class AdsManagerHandler : MonoBehaviour {
	[Header("** Game Essentials **")]
	[SerializeField]
	string productName;
	[SerializeField]
	string productId,productVersion;
	[Header("** Android AD IDs **")]
	[SerializeField]
	string admobBannerIDAndroid;
	[SerializeField]
	string admobInterIDAndroid,admobRewardedAndroid,unityAdIDAndroid,facebookInterstitialAdAndroid,facebookRewardedAdAndroid;
	[Header("** iOS AD IDs **")]
	[SerializeField]
	string admobBannerIDIOS;
	[SerializeField]
	string admobInterIDIOS,admobRewardedIOS,unityAdIDIOS,facebookInterstitialAdIOS,facebookRewardedAdIOS;
	//--------------------------------------------------------------------------------//
	string admobBannerID,admobInterID,admobRewardedId,unityAd_ID,facebookInterstitialAd,facebookRewardedAd;
	int adNumber_Showen,analyticsNum;
	[Header("## ------- Rate & More Apps Link ---------- ##")]
	[SerializeField]
	string moreAppsLink;
	[SerializeField]
	string rateAppsLink;
	[SerializeField]
	string privacyPolicyLink;
	private bool removeAds;
	[Header("** Ad Display Related Start **")]
	[Header("-- AD Showing Properties on  Back to Main Menu --")]
	public List<AdsHandler_Complete_Fail> mainMenuInterstitial;
	[Header("-- AD Showing Properties on Level Complete --")]
	public List<AdsHandler_Complete_Fail> interstitialShow_Success;
	[Header("-- AD Showing Properties on Level Failed --")]
	public List<AdsHandler_Complete_Fail> interstitialShow_Fail;
	[Header("-- AD Showing Properties on Game Paused --")]
	public List<AdsHandler_Complete_Fail> gpPausedInterstitial;
	[Header("** Rewarded Ad Priority **")]
	public RewardedVideoPriority rewardedPriority;
	[Header("** Enable this mode when the game is in testing mode **")]
	[Header("P.S. Uncheck when in build is finalized")]
	public bool enableTestingMode;
	[Header("** Ad Display Related End **")]

	GoogleMobileAds.Api.InterstitialAd interstitial;
	GoogleMobileAds.Api.BannerView bannerView;
	GoogleMobileAds.Api.AdRequest request;
	GoogleMobileAds.Api.AdRequest requestRewarded;
	private GoogleMobileAds.Api.RewardBasedVideoAd rewardBasedVideo;

	AudienceNetwork.InterstitialAd interstitialAd;
	AudienceNetwork.RewardedVideoAd rewardedVideoAd;
	private bool isLoaded,rewardedAdColonyShown,isFBRewardedLoaded;

	public delegate void rewardedAdShown ();
	public static event rewardedAdShown onRewardedAdShown;

	public static int loadAdNumber_Gameplay_Sucess = 0;
	public static int loadAdNumber_Gameplay_Fail = 0;
	public static int loadAdNumber_MM = 0;
	public static int loadAdNumber_GP_Paused = 0;
	public static int rewardedAdShownNumber = 0;

	public static AdsManagerHandler adsManager;

	// Use this for initialization
	void Awake () {
		if (adsManager == null) {
			adsManager = this;
			DontDestroyOnLoad (this.gameObject);
		} 
		else {
			Destroy (this.gameObject);
		}

		#if UNITY_ANDROID
			rateAppsLink = "https://play.google.com/store/apps/details?id="+productId;
		#endif

		if (!removeAdsCheck ()) {
			selectIDs_Platform ();
			adInit ();
		}
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	void Start()
	{
		if (PlayerPrefs.GetInt("consetGDPR",0) == 0) {
			showConsetDialoug();
		}
	}

	public bool removeAdsCheck()
	{
		if (PlayerPrefs.GetInt ("removeAds") == 1) {
			removeAds = true;
			return true;
		} 
		return false;
	}

	public void removeAdsEvent()
	{
		PlayerPrefs.SetInt ("removeAds", 1);
		removeAds = true;
		destroyBannerAd ();
	}

	void selectIDs_Platform()
	{
		#if UNITY_ANDROID
			admobBannerID = admobBannerIDAndroid;
			admobInterID = admobInterIDAndroid;
			unityAd_ID = unityAdIDAndroid;
			facebookInterstitialAd = facebookInterstitialAdAndroid;
			facebookRewardedAd = facebookRewardedAdAndroid;
		#elif UNITY_IPHONE
			admobBannerID = admobBannerIDIOS;
			admobInterID = admobInterIDIOS;
			unityAd_ID = unityAdIDIOS;
			facebookInterstitialAd = facebookInterstitialAdIOS;
			facebookRewardedAd = facebookRewardedAdIOS;
		#endif
		if (enableTestingMode) {
			admobBannerID = "ca-app-pub-3940256099942544/6300978111";
			admobInterID = "ca-app-pub-3940256099942544/1033173712";
		}
	}

	void adInit()
	{
		RequestBanner ();
		RequestInterstitial ();
		Advertisement.Initialize(unityAd_ID, enableTestingMode);
		rewardBasedVideo = RewardBasedVideoAd.Instance;
		RequestRewardedVideo();
		rewardedVideoCallBacksRegister ();
//		#if !UNITY_EDITOR
			LoadFacebookRewardedVideo ();
			LoadFacebookInterstitial ();
//		#endif
	}


	private void RequestRewardedVideo()
	{
		requestRewarded = new AdRequest.Builder().Build();
		this.rewardBasedVideo.LoadAd(requestRewarded, admobRewardedId);
	}

	void rewardedVideoCallBacksRegister()
	{
		this.rewardBasedVideo = RewardBasedVideoAd.Instance;
		rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
		rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
		rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
		rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
		rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
		rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
		rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;
		this.RequestRewardedVideo();
	}


	public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
	}

	public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		MonoBehaviour.print(
			"HandleRewardBasedVideoFailedToLoad event received with message: "
			+ args.Message);
	}

	public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
	}

	public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
	}

	public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleRewardBasedVideoClosed event received");
		this.RequestRewardedVideo();
	}

	public void HandleRewardBasedVideoRewarded(object sender, Reward args)
	{
		onRewardedAdShown ();
	}

	public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
	}



    private void RequestBanner()
	{
		bannerView = new BannerView(admobBannerID, GoogleMobileAds.Api.AdSize.SmartBanner,GoogleMobileAds.Api.AdPosition.Top);
		AdRequest request = new AdRequest.Builder().Build();
		bannerView.LoadAd(request);
	}

	private void RequestInterstitial()
	{
		interstitial = new GoogleMobileAds.Api.InterstitialAd(admobInterID);
		request = new GoogleMobileAds.Api.AdRequest.Builder().Build();
		interstitial.LoadAd(request);
	}

	public void destroyBannerAd()
	{
		bannerView.Hide ();
		bannerView.Destroy ();
	}

	public void displayBannerAd()
	{
		if (!removeAdsCheck()) {
			bannerView.Show ();
		}
	}

	public void showAd(int temp)
	{
		if (temp == 0) {
			if (!removeAdsCheck ()) {
				displayAd (mainMenuInterstitial,loadAdNumber_MM);
				if (loadAdNumber_MM < mainMenuInterstitial.Count - 1)
					loadAdNumber_MM++;
				else
					loadAdNumber_MM = 0;
			}
		}
		else if (temp == 1) {
			if (!removeAdsCheck ()) {
				displayAd (interstitialShow_Success, loadAdNumber_Gameplay_Sucess);
				if (loadAdNumber_Gameplay_Sucess < interstitialShow_Success.Count - 1)
					loadAdNumber_Gameplay_Sucess++;
				else
					loadAdNumber_Gameplay_Sucess = 0;
			}
		}
		else if (temp == 2) {
			if (!removeAdsCheck ()) {
				displayAd (interstitialShow_Fail, loadAdNumber_Gameplay_Fail);
				if (loadAdNumber_Gameplay_Fail < interstitialShow_Fail.Count - 1)
					loadAdNumber_Gameplay_Fail++;
				else
					loadAdNumber_Gameplay_Fail = 0;
			}
		}
		else if (temp == 3) {
			if (!removeAdsCheck ()) {
				displayAd (gpPausedInterstitial,loadAdNumber_GP_Paused);
				if (loadAdNumber_GP_Paused < gpPausedInterstitial.Count - 1)
					loadAdNumber_GP_Paused++;
				else
					loadAdNumber_GP_Paused = 0;
			}
		}
	}

	public void recordLogs(string temp)
	{
		Analytics.CustomEvent(temp);
	}

	void displayAd(List<AdsHandler_Complete_Fail> adHCF,int adNum)
	{
		if (PlayerPrefs.GetInt ("removeAds") != 1) {
			if (showPriorityAd (adHCF,adNum)) {
				Debug.Log ("SDK AD Stuff : Priority Ad on Element " + adNum + " was sucessfull");
			} 
			else if (showSecondaryAd (adHCF,adNum)) {
				Debug.Log ("SDK AD Stuff : 2nd Priority - Fail Ad on Element " + adNum + " was sucessfull");
			}
			else if (showThirdAd (adHCF,adNum)) {
				Debug.Log ("SDK AD Stuff : 3rd Priority - Fail Ad on Element " + adNum + " was sucessfull");
			}
		}
	}

	bool showPriorityAd(List<AdsHandler_Complete_Fail> adHCF,int temp)
	{
		if (adHCF [temp].firstPriority == AdsHandler_Complete_Fail.AdPriority.Admob) {
			if (displayAdMobInterstitial ()) {
				return true;
			}
		}
		else if (adHCF [temp].firstPriority == AdsHandler_Complete_Fail.AdPriority.Unity) {
			if (showVideoAds ()) {
				return true;
			}
		}
		else if (adHCF [temp].firstPriority == AdsHandler_Complete_Fail.AdPriority.Facebook) {
			if (ShowFacebookInterstitial ()) {
				return true;
			}
		}
		return false;
	}

	bool showSecondaryAd(List<AdsHandler_Complete_Fail> adHCF,int temp)
	{
		if (adHCF [temp].secondPriority == AdsHandler_Complete_Fail.AdPriority.Admob) {
			if (displayAdMobInterstitial ()) {
				return true;
			}
		}
		else if (adHCF [temp].secondPriority == AdsHandler_Complete_Fail.AdPriority.Unity) {
			if (showVideoAds ()) {
				return true;
			}
		}
		else if (adHCF [temp].secondPriority == AdsHandler_Complete_Fail.AdPriority.Facebook) {
			if (ShowFacebookInterstitial ()) {
				return true;
			}
		}
		return false;
	}

	bool showThirdAd(List<AdsHandler_Complete_Fail> adHCF,int temp)
	{
		if (adHCF [temp].thirdPriority == AdsHandler_Complete_Fail.AdPriority.Admob) {
			if (displayAdMobInterstitial ()) {
				return true;
			}
		}
		else if (adHCF [temp].thirdPriority == AdsHandler_Complete_Fail.AdPriority.Unity) {
			if (showVideoAds ()) {
				return true;
			}
		}
		else if (adHCF [temp].thirdPriority == AdsHandler_Complete_Fail.AdPriority.Facebook) {
			if (ShowFacebookInterstitial ()) {
				return true;
			}
		}
		return false;
	}


	public void LoadFacebookInterstitial()
	{
		interstitialAd = new AudienceNetwork.InterstitialAd(facebookInterstitialAd);
		this.interstitialAd = interstitialAd;
		this.interstitialAd.Register(this.gameObject);
		this.interstitialAd.InterstitialAdDidLoad = (delegate() {
			Debug.Log("Interstitial ad loaded.");
			this.isLoaded = true;
		});
		interstitialAd.InterstitialAdDidFailWithError = (delegate(string error) {
			Debug.Log("Interstitial ad failed to load with error: " + error);
		});
		interstitialAd.InterstitialAdWillLogImpression = (delegate() {
			Debug.Log("Interstitial ad logged impression.");
		});
		interstitialAd.InterstitialAdDidClick = (delegate() {
			Debug.Log("Interstitial ad clicked.");
		});
		this.interstitialAd.LoadAd();
	}

	bool ShowFacebookInterstitial()
	{
		if (this.isLoaded) {
			this.interstitialAd.Show();
			this.isLoaded = false;
			LoadFacebookInterstitial ();
			return true;
		} 
		LoadFacebookInterstitial ();
		return false;
	}


	bool displayAdMobInterstitial()
	{
		if (!removeAdsCheck()) {
			if(interstitial.IsLoaded())
			{
				interstitial.Show ();
				return true;
			} 
			else
			{
				Debug.Log ("Show Ads = Admob Interstitial not loaded");
			}
			RequestInterstitial ();
		}
		return false;
	}

	bool displayChartboost_Interstatial()
	{
		if (!removeAdsCheck()) {
			
		}
		return false;
	}

	bool showVideoAds()
	{
		if (!removeAdsCheck()) {
			if(Advertisement.IsReady())
			{
				Advertisement.Show ();
				return true;
			} 
			else
			{
				Debug.Log ("Show Ads = Unity Ad not loaded");
			}
		}
		return false;
	}

	bool showRewardedUnityAd()
	{
		if (Advertisement.IsReady("rewardedVideo"))
		{
			var options = new ShowOptions { resultCallback = handleShowResult };
			Advertisement.Show("rewardedVideo", options);
			return true;
		}
		return false;
	}

	bool showAdmobRewardedAd()
	{
		if (rewardBasedVideo.IsLoaded()) {
			rewardBasedVideo.Show();
			RequestRewardedVideo();
			return true;
		}
		RequestRewardedVideo();
		return false;
	}


	void LoadFacebookRewardedVideo()
	{
		rewardedVideoAd = new RewardedVideoAd(facebookRewardedAd);
		this.rewardedVideoAd = rewardedVideoAd;
		this.rewardedVideoAd.Register(this.gameObject);
		this.rewardedVideoAd.RewardedVideoAdDidLoad = (delegate() {
			this.isFBRewardedLoaded = true;
		});
		rewardedVideoAd.RewardedVideoAdDidFailWithError = (delegate(string error) {
			
		});
		rewardedVideoAd.RewardedVideoAdWillLogImpression = (delegate() {
			Debug.Log("RewardedVideo ad logged impression.");
		});
		rewardedVideoAd.RewardedVideoAdDidClick = (delegate() {
			Debug.Log("RewardedVideo ad clicked.");
		});
		this.rewardedVideoAd.LoadAd();
	}

	bool ShowFacebookRewardedVideo()
	{
		if (this.isLoaded) {
			this.rewardedVideoAd.Show();
			onRewardedAdShown ();
			this.isFBRewardedLoaded = false;
			LoadFacebookRewardedVideo ();
			return true;
		}
		LoadFacebookRewardedVideo ();
		return false;
	}

	bool rewardedFirstPriority()
	{
		if (rewardedPriority.firstPriority == RewardedVideoPriority.RewardedVideoAd.Unity) {
			if (showRewardedUnityAd ()) {
				return true;
			}
		}
		else if (rewardedPriority.firstPriority == RewardedVideoPriority.RewardedVideoAd.Admob) {
			if (showAdmobRewardedAd()) {
				return true;
			}
		}
		else if (rewardedPriority.firstPriority == RewardedVideoPriority.RewardedVideoAd.Facebook) {
			if (ShowFacebookRewardedVideo()) {
				return true;
			}
		}
		return false;
	}

	bool rewardedSecondPriority()
	{
		if (rewardedPriority.secondPriority == RewardedVideoPriority.RewardedVideoAd.Unity) {
			if (showRewardedUnityAd ()) {
				return true;
			}
		}
		else if (rewardedPriority.secondPriority == RewardedVideoPriority.RewardedVideoAd.Admob) {
			if (showAdmobRewardedAd()) {
				return true;
			}
		}
		else if (rewardedPriority.secondPriority == RewardedVideoPriority.RewardedVideoAd.Facebook) {
			if (ShowFacebookRewardedVideo()) {
				return true;
			}
		}
		return false;
	}

	bool rewardedThirdPriority()
	{
		if (rewardedPriority.thirdPriority == RewardedVideoPriority.RewardedVideoAd.Unity) {
			if (showRewardedUnityAd ()) {
				return true;
			}
		}
		else if (rewardedPriority.thirdPriority == RewardedVideoPriority.RewardedVideoAd.Admob) {
			if (showAdmobRewardedAd()) {
				return true;
			}
		}
		else if (rewardedPriority.thirdPriority == RewardedVideoPriority.RewardedVideoAd.Facebook) {
			if (ShowFacebookRewardedVideo()) {
				return true;
			}
		}
		return false;
	}
	public void showRewardedVideoAd()
	{
		if (rewardedFirstPriority()) {
			Debug.Log ("First Priority Rewarded Ad Shown");
		}
		else if (rewardedSecondPriority()) {
			Debug.Log ("Second Priority Rewarded Ad Shown");
		}
		else if (rewardedThirdPriority()) {
			Debug.Log ("Third Priority Rewarded Ad Shown");
		}
		else {
			MNPopup popup = new MNPopup ("Ad not available", "Sorry rewarded ad not available, please try again later.");
			popup.AddAction ("Ok", () => {
				Debug.Log ("Show Ad = Rewarded Ad not available try again");
			});
			popup.AddDismissListener (() => {
				Debug.Log("dismiss listener");
			});
			popup.Show ();
		}

	}

	private void handleShowResult(ShowResult result)
	{
		switch (result)
		{
			case ShowResult.Finished:
				Debug.Log ("The ad was successfully shown.");
				onRewardedAdShown ();
			break;
			case ShowResult.Skipped:
				Debug.Log("The ad was skipped before reaching the end.");
			break;
			case ShowResult.Failed:
				Debug.LogError("The ad failed to be shown.");
			break;
		}
	}

	public void showRateUsDialoug()
	{
		MNRateUsPopup rateUs = new MNRateUsPopup ("Are you having fun?", "Show us how much you are love and give us Five stars", "Rate Us", "No, Thanks", "Later");
		rateUs.AddDeclineListener (() => { 
			Debug.Log("rate us declined"); 
		});
		rateUs.AddRemindListener (() => { 
			Debug.Log("remind me later"); 
		});
		rateUs.AddRateUsListener (() => { 
			openRateUs();
		});
		rateUs.AddDismissListener (() => { 
			Debug.Log("rate us dialog dismissed :("); 
		});
		rateUs.Show ();
	}

	public void showConsetDialoug()
	{
		MNPopup popup = new MNPopup ("Terms and Conditions", productName+"is a free to play game. This product is subject to our terms of services and privacy policy. Press ok to accept the terms and conditions and priavcy policy");
		popup.AddAction ("Ok", () => {
			PlayerPrefs.SetInt("consetGDPR",1);
		});
		popup.AddAction ("Terms and Conditions", () => {
			Application.OpenURL(privacyPolicy());
		});
		popup.AddDismissListener (() => {Debug.Log("dismiss listener");});
		popup.Show ();
	}


	public string privacyPolicy()
	{
		return privacyPolicyLink;
	}

	public void showExitMenu()
	{
		MNPopup popup = new MNPopup ("Exit Game", "Do you really want to exit this game?");
		popup.AddAction ("Yes", () => {
			Application.Quit();
		});
		popup.AddAction ("No", () => {
			
		});
		popup.AddDismissListener (() => {Debug.Log("dismiss listener");});
		popup.Show ();
	}

	void OnApplicationQuit()
	{
		
	}

	void openRateUs()
	{
		Application.OpenURL (rateAppsLink);
	}

	public void openMoreApps()
	{
		Application.OpenURL (moreAppsLink);
	}

	[System.Serializable]
	public class AdsHandler_Complete_Fail
	{
		public enum AdPriority 
		{
			Admob,
			Unity,
			Facebook
		};
		public AdPriority firstPriority,secondPriority,thirdPriority;
	}
	[System.Serializable]
	public class RewardedVideoPriority
	{
		public enum RewardedVideoAd {
			Unity,
			Admob,
			Facebook
		};
		public RewardedVideoAd firstPriority,secondPriority,thirdPriority;
	}
}
