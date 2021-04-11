using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Examples.LocationProvider;
using Mapbox.Examples;

public class CameraController : MonoBehaviour {

	public bool LookPlayer = true;
	public PositionWithLocationProvider player;

	public float distance = 60.0f;

	public Vector3 offSet = new Vector3(0, 1, -1);
	public float damping = 1.0f;

	private float xAngle, yAngle, 
	xAngTemp, yAngTemp;

	public Vector3 firstPoint, secondPoint;

	#if UNITY_EDITOR
	public bool mouseDown = false;
	#endif
	public static CameraController instance;

	// Use this for initialization
	void Awake() {  instance = this; }
	void Start () {
		transform.position = player.transform.position + offSet;
	}

	void Update()
	{
		if (MainUIController.instance.Active || !LookPlayer)
			return;
		#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0))
		{
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 1000))
            {
                Vector3 pos = hit.point;
                pos.y = 1;
                GroundCanvas.instance.Show(pos);
            }
            firstPoint = Input.mousePosition;
			mouseDown = true;
		}
		if (Input.GetMouseButton(0))
		{
			secondPoint = Input.mousePosition;
			float deltaMoveY = firstPoint.y - secondPoint.y;
			float deltaMove = firstPoint.x - secondPoint.x;
			if (Mathf.Abs(deltaMoveY) > Mathf.Abs(deltaMove))
			{	
				Camera.main.fieldOfView += deltaMoveY * Time.deltaTime * 0.4f;
				if (Camera.main.fieldOfView < 10)
					Camera.main.fieldOfView = 10;
				if (Camera.main.fieldOfView > 100)
					Camera.main.fieldOfView = 100;
			}
			else
			{
				transform.RotateAround(player.transform.position, Vector3.up, deltaMove * Time.deltaTime * 0.4f);
				offSet = transform.position-player.transform.position;
			}
			mouseDown = false;
		}



#else
		if (Input.touchCount > 0 && Input.GetTouch(0).tapCount == 2) {
			if (Input.GetTouch (0).phase == TouchPhase.Began) {
				firstPoint = Input.GetTouch (0).position;				
			} else if (Input.GetTouch (0).phase == TouchPhase.Moved) {
				secondPoint = Input.GetTouch (0).position;				
				float deltaMove = firstPoint.x - secondPoint.x;
				float deltaMoveY = firstPoint.y - secondPoint.y;
				if (Mathf.Abs(deltaMoveY) > Mathf.Abs(deltaMove))
				{	
					Camera.main.fieldOfView += deltaMoveY * Time.deltaTime * 0.4f;
					if (Camera.main.fieldOfView < 10)
						Camera.main.fieldOfView = 10;
					if (Camera.main.fieldOfView > 100)
						Camera.main.fieldOfView = 100;
				}
				else
				{
					transform.RotateAround(player.transform.position, Vector3.up, deltaMove * Time.deltaTime * 0.4f);
					offSet = transform.position-player.transform.position;
				}
			}
		} else if (Input.touchCount > 0 && Input.GetTouch(0).tapCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Ray r = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(r, out hit, 1000))
                {
                    Vector3 pos = hit.point;
                    pos.y = 1;
                    GroundCanvas.instance.Show(pos);
                }
            }
        }
#endif
    }

    // Update is called once per frame
    void LateUpdate () {
		if (!LookPlayer)
			return;
		Vector3 _targetPos = player.transform.position + offSet;
		if (Vector3.Distance (transform.position, _targetPos) > 50) {
			transform.position = player.transform.position + offSet;
			return;
		}
		//GetComponent<Camera> ().fieldOfView = distance;
		_targetPos.y = transform.position.y;
		transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * damping);
		transform.LookAt (player.transform);
	}
}
