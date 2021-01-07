using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Resources.Classes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ServerScripts {
    public class InGameManager : MonoBehaviourPunCallbacks {
        public static PlayerSoldier localPlayer { get; private set; }
        public GameObject canvas;
        public GameObject playerPrefab;
        public PhotonTeamsManager ptm;
        private bool init;
        private bool canvasActive;


        void Start() {
            canvas.SetActive(canvasActive);
        }

        void Update() {
            if (!init) {
                localPlayer = AddNewPlayer();
                init = true;
            }

            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            canvasActive = !canvasActive;
            canvas.SetActive(canvasActive);
        }


        private PlayerSoldier AddNewPlayer() {
            var player = PhotonNetwork.Instantiate(playerPrefab.name,
                new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0), Quaternion.identity);
            var teamsList = ptm.GetAvailableTeams().ToList();
            var minTeamMembers = teamsList.Min(team => ptm.GetTeamMembersCount(team));
            var myTeam = teamsList.First(team => ptm.GetTeamMembersCount(team) == minTeamMembers);
            PhotonNetwork.LocalPlayer.JoinTeam(myTeam);
            return new PlayerSoldier(PhotonNetwork.LocalPlayer, PhotonNetwork.NickName, myTeam, 10, player); // TODO replace damage with ScriptableObject Weapon
        }

        public void Leave() {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom() {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) {
            Debug.Log($"{newPlayer.NickName} entered room");
        }

        public override void OnJoinedRoom() {
            Debug.Log($"Here we are");

        }

        public override void OnPlayerLeftRoom(Player otherPlayer) {
            Debug.Log($"{otherPlayer.NickName} left room");
        }
    }
}