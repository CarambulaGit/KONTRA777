using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class LookAtMouse : MonoBehaviour {
	private PhotonView photonView;
	private Camera playerCamera;

	void Start() {
		photonView = GetComponent<PhotonView>();
		playerCamera = GameObject.FindGameObjectWithTag(CameraController.playerCameraTag).GetComponent<Camera>();
	}

	void Update() {
		if (!photonView.IsMine) return;
		Vector3 aimDir = (playerCamera.WorldToScreenPoint(transform.position) - Input.mousePosition).normalized;
		float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
		transform.eulerAngles = new Vector3(0, 0, angle + 90);
	}
}