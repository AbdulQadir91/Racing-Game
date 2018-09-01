using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleFillAmount_Handler : MonoBehaviour {

	public Image parkingImage;
	bool disableFillAmount;
	[Header("** Parking Complete Event **")]
	public UnityEngine.Events.UnityEvent parkingComplete;

	public void simpleParkingAdder()
	{
		parkingImage.fillAmount += 0.01f;
	}

	public void disableFillAmountVar(bool temp)
	{
		disableFillAmount = temp;
	}

	public void callingFillAmount()
	{
		StartCoroutine (fillAmountFullyDone());
	}

	public IEnumerator fillAmountFullyDone()
	{
		if (parkingImage.fillAmount > 0.95f && disableFillAmount) {
			disableFillAmount = false;
			parkingImage.gameObject.SetActive (false);
			parkingComplete.Invoke ();
			gameObject.SetActive (false);
		}
		if (parkingImage.fillAmount < 0.95f && disableFillAmount) {
			yield return new WaitForSeconds (0.1f);
			StartCoroutine (fillAmountFullyDone());
		}
	}
}
