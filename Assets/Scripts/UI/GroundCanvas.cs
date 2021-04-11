using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundCanvas : MonoBehaviour {

    public Image img;

    public static GroundCanvas instance;

    void Awake() { instance = this; }

	// Use this for initialization
	void Start () {
        Hide();
        //iTween.ScaleBy(gameObject, iTween.Hash("amount", new Vector3(3, 3, 3), "time", 1.5f, "loopType", "loop"));
	}

    bool animating = false;

    IEnumerator animate()
    {
//
        animating = true;
        Vector3 scale = new Vector3(0.003f, 0.003f, 0.003f);
        iTween.ScaleBy(gameObject, iTween.Hash("amount", new Vector3(2, 2, 2), "time", 0.7f));
        yield return new WaitForSeconds(0.8f);
        transform.localScale = scale;
        iTween.ScaleBy(gameObject, iTween.Hash("amount", new Vector3(2, 2, 2), "time", 0.7f));        
        yield return new WaitForSeconds(0.8f);
        transform.localScale = scale;
        animating = false;
        Hide();
    }

    public void Hide() {
        img.enabled = false;
    }
    public void Show(Vector3 pos) {
        if (animating)
            return;
        img.enabled = true;      
        transform.position = pos;
        StartCoroutine(animate());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
