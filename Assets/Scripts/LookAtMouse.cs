using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour {
	void Start() { }

	void Update() {
		Vector3 aimDir = (Camera.main.WorldToScreenPoint(transform.position) - Input.mousePosition).normalized;
		float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
		transform.eulerAngles = new Vector3(0,0, angle + 90);
	Debug.Log(angle);
	}
}