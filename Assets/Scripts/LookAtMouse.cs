using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class LookAtMouse : MonoBehaviour {
	private PhotonView photonView;
	void Start() { photonView = GetComponent<PhotonView>(); }

	void Update() {
		if (!photonView.IsMine) return;
		Vector3 aimDir = (Camera.main.WorldToScreenPoint(transform.position) - Input.mousePosition).normalized;
		float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
		transform.eulerAngles = new Vector3(0, 0, angle + 90);
	}
}