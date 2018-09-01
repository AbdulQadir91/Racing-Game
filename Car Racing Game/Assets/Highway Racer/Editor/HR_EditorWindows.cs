using UnityEngine;
using UnityEditor;
using System.Collections;

public class HR_EditorWindows : Editor {

	[MenuItem("Highway Racer/General Settings", false, -100)]
	public static void OpenGeneralSettings(){
		Selection.activeObject =HR_HighwayRacerProperties.Instance;
	}

	[MenuItem("Highway Racer/Configure Player Cars", false, 1)]
	public static void OpenCarSettings(){
		Selection.activeObject =HR_PlayerCars.Instance;
	}

	[MenuItem("Highway Racer/Configure Upgradable Wheels", false, 1)]
	public static void OpenWheelsSettings(){
		Selection.activeObject =HR_Wheels.Instance;
	}

	[MenuItem("Highway Racer/PDF Documentation", false, 2)]
	public static void OpenDocs(){
		string url = "https://dl.dropboxusercontent.com/u/248930654/_Documentations/Highway%20Racer%20Complete%20Project.pdf";
		Application.OpenURL(url);
	}

	[MenuItem("Highway Racer/Highlight Traffic Cars Folder", false, 102)]
	public static void OpenTrafficCarsFolder(){
		Selection.activeObject = AssetDatabase.LoadMainAssetAtPath ("Assets/Highway Racer/Resources/TrafficCars");
	}

	[MenuItem("Highway Racer/Help", false, 1000)]
	static void Help(){

		EditorUtility.DisplayDialog("Contact", "Please include your invoice number while sending a contact form.", "Ok");

		string url = "http://www.bonecrackergames.com/contact/";
		Application.OpenURL (url);

	}

}
