//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class HR_ImageEffects : MonoBehaviour {

	private BloomOptimized bloom;
	private FlareLayer flareLayer;
	
	void Awake () {

		if(!PlayerPrefs.HasKey("LensFlare"))
			PlayerPrefs.SetInt("LensFlare", (HR_HighwayRacerProperties.Instance._defaultFlares == true ? 1 : 0));

		if(!PlayerPrefs.HasKey("Bloom"))
			PlayerPrefs.SetInt("Bloom", (HR_HighwayRacerProperties.Instance._defaultBloom == true ? 1 : 0));

		if(!PlayerPrefs.HasKey("HQLights"))
			PlayerPrefs.SetInt("HQLights", (HR_HighwayRacerProperties.Instance._defaultHQLights == true ? 1 : 0));

		bloom = GetComponent<BloomOptimized>();
		flareLayer = GameObject.FindObjectOfType<FlareLayer>();

		Check ();
	
	}
	
	public void Check () {
	
		if(PlayerPrefs.GetInt("Bloom") == 0){
			if(bloom.enabled)
				bloom.enabled = false;
		}
		if(PlayerPrefs.GetInt("Bloom") == 1){
			if(!bloom.enabled)
				bloom.enabled = true;
		}

		if(PlayerPrefs.GetInt("LensFlare") == 0){
				flareLayer.enabled = false;
		}
		if(PlayerPrefs.GetInt("LensFlare") == 1){
				flareLayer.enabled = true;
		}

		if(PlayerPrefs.GetInt("HQLights") == 0){
			QualitySettings.pixelLightCount = 0;
		}
		if(PlayerPrefs.GetInt("HQLights") == 1){
			QualitySettings.pixelLightCount = 10;
		}

	}

}
