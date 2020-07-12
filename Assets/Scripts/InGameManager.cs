using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviourPunCallbacks {

	public GameObject canvas;
	private bool canvasActive = true;

	void Start() {
		canvasActive = !canvasActive;
		canvas.SetActive(canvasActive);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			canvasActive = !canvasActive;
			canvas.SetActive(canvasActive);
		}
	}

	public void Leave() { PhotonNetwork.LeaveRoom(); }

	public override void OnLeftRoom() { SceneManager.LoadScene(0); }

	public override void OnPlayerEnteredRoom(Player newPlayer) { Debug.Log($"{newPlayer.NickName} entered room"); }

	public override void OnPlayerLeftRoom(Player otherPlayer) { Debug.Log($"{otherPlayer.NickName} left room"); }
}