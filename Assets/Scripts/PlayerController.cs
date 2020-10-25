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
		var posChange = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
		if (posChange.magnitude > Vector3.right.magnitude) { posChange = posChange.normalized; }
		transform.position += posChange * moveSpeed * Time.deltaTime;
	}
}