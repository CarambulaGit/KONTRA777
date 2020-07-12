using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float moveSpeed;
	private PhotonView photonView;

	void Start() { photonView = GetComponent<PhotonView>(); }

	void FixedUpdate() {
		if (!photonView.IsMine) return;
		var horizontalChange = Vector3.right * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
		var verticalChange = Vector3.up * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
		transform.position += horizontalChange + verticalChange;
	}
}