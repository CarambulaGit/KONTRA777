using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviourPunCallbacks {

	public GameObject canvas;
	public GameObject playerPrefab;
	private bool canvasActive = true;


	void Start() {
		canvasActive = !canvasActive;
		canvas.SetActive(canvasActive);
		PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0), Quaternion.identity);
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