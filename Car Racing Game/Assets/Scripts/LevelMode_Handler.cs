using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMode_Handler : MonoBehaviour {

	[Header("Primary Objective")]
	public string levelPrimaryObjectives;

	[Header("Secondary Objectives")]
	public string[] levelSecondaryObjectives;

	[Header("Contain CutScene?")]
	public bool hasCutScene;
	[Header("CutScene Object")]
	public GameObject cutSceneObject;
	public float cutSceneTime;
	public bool skipCutScene;


	public IEnumerator cutSceneInit()
	{
		cutSceneObject.SetActive (true);
		yield return new WaitForSeconds (cutSceneTime);
		if (!skipCutScene) {
			GamePlay_Script_Handler.gsh.initLevelObjectives ();
		}
	}

	public void skipCutScene_Event()
	{
		StopCoroutine (cutSceneInit());
		GamePlay_Script_Handler.gsh.initLevelObjectives ();
	}
}
