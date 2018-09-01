using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTriggerHandler : MonoBehaviour {

	[Header("- For Trigger Detection? -")]
	public bool isInRoot;
	public bool withTag,withMonoBehaviour;
	[Header("- If you want to check with a script -")]
	public MonoBehaviour collisionWith;
	[Header("- Tag name-")]
	public string collisionTag;
	[Header("** Trigger Events **")]
	public UnityEngine.Events.UnityEvent onTriggerEnter;
	public UnityEngine.Events.UnityEvent onTriggerStay,onTriggerExit;

	void OnTriggerEnter(Collider col)
	{
		if (isInRoot && withTag && col.transform.root.CompareTag(collisionTag)) {
			onTriggerEnter.Invoke ();
		}
		else if (!isInRoot && withTag && col.CompareTag(collisionTag)) {
			onTriggerEnter.Invoke ();
		}
		if (isInRoot && (withMonoBehaviour && (col.transform.root.GetComponent<MonoBehaviour>() == collisionWith))) {
			onTriggerEnter.Invoke ();
		}
		else if (!isInRoot && (withMonoBehaviour && (col.transform.GetComponent<MonoBehaviour>() == collisionWith))) {
			onTriggerEnter.Invoke ();
		}
	}

	void OnTriggerStay(Collider col)
	{
		if (isInRoot && withTag && col.transform.root.CompareTag(collisionTag)) {
			onTriggerStay.Invoke ();
		}
		else if (!isInRoot && withTag && col.CompareTag(collisionTag)) {
			onTriggerStay.Invoke ();
		}
		if (isInRoot && (withMonoBehaviour && (col.transform.root.GetComponent<MonoBehaviour>() == collisionWith))) {
			onTriggerStay.Invoke ();
		}
		else if (!isInRoot && (withMonoBehaviour && (col.transform.GetComponent<MonoBehaviour>() == collisionWith))) {
			onTriggerStay.Invoke ();
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (isInRoot && withTag && col.transform.root.CompareTag(collisionTag)) {
			onTriggerExit.Invoke ();
		}
		else if (!isInRoot && withTag && col.CompareTag(collisionTag)) {
			onTriggerExit.Invoke ();
		}
		if (isInRoot && (withMonoBehaviour && (col.transform.root.GetComponent<MonoBehaviour>() == collisionWith))) {
			onTriggerExit.Invoke ();
		}
		else if (!isInRoot && (withMonoBehaviour && (col.transform.GetComponent<MonoBehaviour>() == collisionWith))) {
			onTriggerExit.Invoke ();
		}
	}
}
