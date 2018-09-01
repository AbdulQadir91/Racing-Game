//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HR_ImageEffects_LensFlare : MonoBehaviour {

	private HR_ImageEffects imageEffects;
	private Button sprite;
	private Color defCol;
	
	void Start () {

		imageEffects = GameObject.FindObjectOfType<HR_ImageEffects>();
		sprite = GetComponent<Button>();
		defCol = sprite.image.color;

		if(!PlayerPrefs.HasKey("LensFlare"))
			PlayerPrefs.SetInt("LensFlare", 1);

		Check();
		
	}

	
	public void OnClick () {
		
		if(PlayerPrefs.GetInt("LensFlare") == 0)
			PlayerPrefs.SetInt("LensFlare", 1);
		else if(PlayerPrefs.GetInt("LensFlare") == 1)
			PlayerPrefs.SetInt("LensFlare", 0);

		Check();
		
	}
	
	void Check(){
		
		if(PlayerPrefs.GetInt("LensFlare") == 1){
			sprite.image.color = new Color(.667f, 1f, 0f);
		}
		if(PlayerPrefs.GetInt("LensFlare") == 0){
			sprite.image.color = defCol;
		}

		imageEffects.Check();
		
	}

}
