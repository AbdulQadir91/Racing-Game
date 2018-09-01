using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySecondaryObjectiveDialoug : MonoBehaviour {

	public int secondaryObjectiveToDisplay;

	public void displaySecondaryObjectives()
	{
		GamePlay_Script_Handler.gsh.initSecondaryObjectives (secondaryObjectiveToDisplay);
	}
}
