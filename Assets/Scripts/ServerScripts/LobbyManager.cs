using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace ServerScripts
{
	public class LobbyManager : MonoBehaviourPunCallbacks {
		public Text InputField;
		public Text logText;
		public string roomName = "default";
		[Range(1, 8)] public int numOfPlayers = 4;

		void Start() {
			PhotonNetwork.NickName = "Unnamed";
			PhotonNetwork.GameVersion = "1";
			PhotonNetwork.AutomaticallySyncScene = true;
			PhotonNetwork.ConnectUsingSettings();
		}

		private void Log(string message) { logText.text += "\n" + message; }

		public override void OnConnectedToMaster() { Log("Connected to Master"); }

		public void CreateRoom() {
			PhotonNetwork.NickName = InputField.text;
			PhotonNetwork.CreateRoom(roomName, new Photon.Realtime.RoomOptions {MaxPlayers = (byte) numOfPlayers});
		}

		public void JoinRoom() {
			PhotonNetwork.NickName = InputField.text;
			PhotonNetwork.JoinRoom(roomName);
		}

		public override void OnJoinedRoom() {
			Log("Joined room");
			PhotonNetwork.LoadLevel("Game");
		}

		void Update() { }
	}
}