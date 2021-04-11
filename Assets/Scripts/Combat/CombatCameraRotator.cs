using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCameraRotator : MonoBehaviour {
	bool canUseGyro = false;
	Gyroscope gyro;
	Quaternion originalRot;
	// Use this for initialization
	void Start () {
		originalRot = transform.rotation;
		canUseGyro = SystemInfo.supportsGyroscope;

		if (canUseGyro) {
			gyro = Input.gyro;
			gyro.enabled = true;
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (canUseGyro && CombatUI.instance.ARIsOn) {
			transform.Rotate (-gyro.rotationRateUnbiased.x, -gyro.rotationRateUnbiased.y, 0);
		} else
			transform.rotation = originalRot;
	}
}
