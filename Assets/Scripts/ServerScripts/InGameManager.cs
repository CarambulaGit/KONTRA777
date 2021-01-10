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
        public GameObject canvas;
        public GameObject playerPrefab;
        public PhotonTeamsManager ptm;
        public Transform[] SpawnPoints;
        private bool init;
        private bool canvasActive;
        private GameObject localPlayer;


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


        private GameObject AddNewPlayer() {
            var teamsList = ptm.GetAvailableTeams().ToList();
            var minTeamMembers = teamsList.Min(team => ptm.GetTeamMembersCount(team));
            var myTeam = teamsList.First(team => ptm.GetTeamMembersCount(team) == minTeamMembers);
            PhotonNetwork.LocalPlayer.JoinTeam(myTeam);
            return PhotonNetwork.Instantiate(playerPrefab.name,
                SpawnPoints[myTeam.Code - 1].position, Quaternion.identity);
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
            // todo update PlayerSoldier.players
            PlayerSoldier.players.Remove(PlayerSoldier.FindPSByPhotonPlayer(otherPlayer));
            Debug.Log($"{otherPlayer.NickName} left room");
        }
    }
}