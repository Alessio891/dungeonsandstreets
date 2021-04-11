using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIBridgeLoading : MonoBehaviour {
	public Image loader;
    public Text requestCount;
	bool showing = false;
	float timer = 0.0f;
	// Use this for initialization
	void Start () {
		loader.enabled = false;
        requestCount.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Bridge.routinesRunning > 3 && !showing) {
			timer += Time.deltaTime;
			if (timer > 0.5f) {				
				loader.enabled = true;
                requestCount.enabled = true;
				showing = true;
				timer = 0;
			}
		} else
			timer = 0;

		if (Bridge.routinesRunning <= 3 && showing) {
			loader.enabled = false;
            requestCount.enabled = false;
            showing = false;
		}

        if (showing)
        {
            requestCount.text = "Requests: " + Bridge.routinesRunning.ToString();
        }
	}
}
