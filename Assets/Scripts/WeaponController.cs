using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class WeaponController : MonoBehaviour {
	public Transform firePoint;
	public LineRenderer lineRenderer;
	private PhotonView photonView;

	void Start() { photonView = GetComponent<PhotonView>(); }

	void Update() {
		if (!photonView.IsMine) return;
		if (Input.GetButtonDown("Fire1")) StartCoroutine(Shoot());
	}

	IEnumerator Shoot() {
		var hitInfo = Physics2D.Raycast(firePoint.position, firePoint.up);
		if (hitInfo) {
			lineRenderer.SetPosition(0, firePoint.position);
			lineRenderer.SetPosition(1, hitInfo.transform.position);
			Debug.Log($"Hit {hitInfo.transform.name}");
		} else {
			lineRenderer.SetPosition(0, firePoint.position);
			lineRenderer.SetPosition(1, firePoint.up * 100);
		}
		lineRenderer.enabled = true;
		yield return new WaitForSeconds(0.02f);
		lineRenderer.enabled = false;
	}
}