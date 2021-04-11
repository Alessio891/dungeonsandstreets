using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraViewer : MonoBehaviour {
	WebCamTexture wct;
	public RectTransform rawImageRT;
	public AspectRatioFitter rawImageARF;
	RawImage rawImage;
	// Use this for initialization
	void Start () {
		wct = new WebCamTexture ();


		rawImage = GetComponent<RawImage> ();
		rawImage.texture = wct;
		wct.Play ();
	}

	private void Update()
	{
		if ( wct.width < 100 )
		{
			Debug.Log("Still waiting another frame for correct info...");
			return;
		}

		// change as user rotates iPhone or Android:

		int cwNeeded = wct.videoRotationAngle;
		// Unity helpfully returns the _clockwise_ twist needed
		// guess nobody at Unity noticed their product works in counterclockwise:
		int ccwNeeded = -cwNeeded;

		// IF the image needs to be mirrored, it seems that it
		// ALSO needs to be spun. Strange: but true.
		if ( wct.videoVerticallyMirrored ) ccwNeeded += 180;

		// you'll be using a UI RawImage, so simply spin the RectTransform
		rawImageRT.localEulerAngles = new Vector3(0f,0f,ccwNeeded);

		float videoRatio = (float)wct.width/(float)wct.height;

		// you'll be using an AspectRatioFitter on the Image, so simply set it
		rawImageARF.aspectRatio = videoRatio;

		// alert, the ONLY way to mirror a RAW image, is, the uvRect.
		// changing the scale is completely broken.
		if ( wct.videoVerticallyMirrored )
			rawImage.uvRect = new Rect(0,1,1,-1);  // means flip on vertical axis
		else
			rawImage.uvRect = new Rect(0,0,1,1);  // means no flip

		// devText.text =
		//  videoRotationAngle+"/"+ratio+"/"+wct.videoVerticallyMirrored;
	}
}
