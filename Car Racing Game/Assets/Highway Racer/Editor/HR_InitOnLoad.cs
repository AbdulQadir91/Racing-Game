using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class HR_InitOnLoad {
	
	static HR_InitOnLoad(){
		
		if(!EditorPrefs.HasKey("HR_Installed")){
			
			EditorPrefs.SetInt("HR_Installed", 1);
			EditorUtility.DisplayDialog("Regards from BoneCracker Games", "Thank you for purchasing Highway Racer Complete Project. Please read the documentation before use. Also check out the online documentation for updated info. Have fun :)", "Let's get started");
			Selection.activeObject = HR_HighwayRacerProperties.Instance;

		}

	}

}