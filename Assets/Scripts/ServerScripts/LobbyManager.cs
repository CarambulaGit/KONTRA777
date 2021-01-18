using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace ServerScripts
{
	public class LobbyManager : MonoBehaviourPunCallbacks {
		public InputField nicknameInputField;
		public Text logText;
		public string roomName = "default";
		private string path;
		private string nicknameFileName = "Nickname.txt";

		[Range(1, 8)] public int numOfPlayers = 4;

		public override void OnEnable() {
			base.OnEnable();
			path = Path.Combine(Application.dataPath, nicknameFileName);
			nicknameInputField.text = LoadNickname();
		}

		void Start() {
			PhotonNetwork.NickName = nicknameInputField.text;
			PhotonNetwork.GameVersion = "1";
			PhotonNetwork.AutomaticallySyncScene = true;
			PhotonNetwork.ConnectUsingSettings();
		}

		private void Log(string message) { logText.text += "\n" + message; }

		public override void OnConnectedToMaster() { Log("Connected to Master"); }

		public void CreateRoom() {
			SaveNickname(nicknameInputField.text);
			PhotonNetwork.NickName = LoadNickname();
			PhotonNetwork.CreateRoom(roomName, new Photon.Realtime.RoomOptions {MaxPlayers = (byte) numOfPlayers});
		}

		public void JoinRoom() {
			SaveNickname(nicknameInputField.text);
			PhotonNetwork.NickName = LoadNickname();
			PhotonNetwork.JoinRoom(roomName);
		}

		public override void OnJoinedRoom() {
			Log("Joined room");
			PhotonNetwork.LoadLevel("Game");
		}

		void Update() { }

		void SaveNickname(string nickname) {
			using (StreamWriter sw = File.CreateText(path)) {
				sw.WriteLine(nickname);
			}
		}

		string LoadNickname() {
			if (!File.Exists(path)) {
				SaveNickname("");
			}
			using (StreamReader sr = File.OpenText(path)) {
				var text = sr.ReadToEnd();	
				return text;
			}
		}
	}
}