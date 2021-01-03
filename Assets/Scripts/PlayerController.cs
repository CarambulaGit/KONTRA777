using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	private const float EPSILON = 0.00001f;
	public float moveSpeed;
	public Animator animator;
	private PhotonView photonView;
	private float moveAnimSpeed;

	void Start() { photonView = GetComponent<PhotonView>(); }

	void FixedUpdate() {
		if (!photonView.IsMine) return;
		Move(out moveAnimSpeed);
		Animate();
	}

	private void Move(out float animSpeed) {
		var posChange = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
		if (posChange.magnitude > Vector3.right.magnitude) { posChange = posChange.normalized; }
		animSpeed = posChange.magnitude;
		transform.position += posChange * (moveSpeed * Time.deltaTime);
	}

	private void Animate() { AnimateMove(); }

	private void AnimateMove() { animator.SetFloat("MoveSpeed", moveAnimSpeed); }
}