using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks {
	public Text logText;
	public string roomName = "default";
	[Range(1, 8)] public int numOfPlayers = 4;

	void Start() {
		PhotonNetwork.NickName = "Player " + PhotonNetwork.CountOfPlayers;
		Log(PhotonNetwork.NickName);
		PhotonNetwork.GameVersion = "1";
		PhotonNetwork.AutomaticallySyncScene = true;
		PhotonNetwork.ConnectUsingSettings();
	}

	private void Log(string message) { logText.text += "\n" + message; }

	public override void OnConnectedToMaster() { Log("Connected to Master"); }

	public void CreateRoom() {
		PhotonNetwork.CreateRoom(roomName, new Photon.Realtime.RoomOptions {MaxPlayers = (byte) numOfPlayers});
	}

	public void JoinRoom() { PhotonNetwork.JoinRoom(roomName); }

	public override void OnJoinedRoom() {
		Log("Joined room");
		PhotonNetwork.LoadLevel("Game");
	}

	void Update() { }
}