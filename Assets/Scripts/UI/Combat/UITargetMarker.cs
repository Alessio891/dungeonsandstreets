using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITargetMarker : MonoBehaviour
{

    public Image marker;
    public Transform target;
    public static UITargetMarker instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(animate());
    }

    IEnumerator animate()
    {
        while (true)
        {
            while (marker.color.a > 0)
            {
                Color c = marker.color;
                c.a -= 1.5f * Time.deltaTime;
                marker.color = c;
                yield return null;
            }
            while (marker.color.a < 1)
            {
                Color c = marker.color;
                c.a += 1.5f * Time.deltaTime;
                marker.color = c;
                yield return null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if (!marker.enabled)
                marker.enabled = true;
            transform.position = Camera.main.WorldToScreenPoint(target.position + (Vector3.up));
        } else
        {
            if (marker.enabled)
                marker.enabled = false;
        }
    }
}
