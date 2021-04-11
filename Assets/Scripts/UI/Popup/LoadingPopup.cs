using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPopup : BasePopup {
	public GameObject Mask;
	public bool UseMask = true;

	public System.Func<bool> LoadingCondition;

	public void StartLoading(System.Func<bool> loadingCondition)
	{
		LoadingCondition = loadingCondition;
		if (!UseMask)
			Mask.SetActive (false);
		StopAllCoroutines ();
		StartCoroutine (WaitForCondition ());
	}

	IEnumerator WaitForCondition()
	{
		while (!LoadingCondition ()) {
			yield return new WaitForEndOfFrame ();
		}

		Close ();
	}
}
