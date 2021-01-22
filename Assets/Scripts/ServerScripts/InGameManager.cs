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
        public GameObject playerPrefab;
        public PhotonTeamsManager ptm;
        public Transform[] SpawnPoints;
        public RoundManager roundManager;
        private bool init;
        private GameObject localPlayer;

        void Start() { }

        void Update() {
            if (!init) {
                localPlayer = AddNewPlayer();
                init = true;
            }
        }

        private GameObject AddNewPlayer() {
            var teamsList = ptm.GetAvailableTeams().ToList();
            var minTeamMembers = teamsList.Min(team => ptm.GetTeamMembersCount(team));
            var myTeam = teamsList.First(team => ptm.GetTeamMembersCount(team) == minTeamMembers) ?? teamsList[0];
            PhotonNetwork.LocalPlayer.JoinTeam(myTeam);
            return PhotonNetwork.Instantiate(playerPrefab.name,
                SpawnPoints[myTeam.Code - 1].position, Quaternion.identity);
        }

        public override void OnLeftRoom() {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) {
            Debug.Log($"{newPlayer.NickName} entered room");
        }

        public override void OnJoinedRoom() { }

        public override void OnPlayerLeftRoom(Player otherPlayer) {
            PlayerSoldier.players.Remove(PlayerSoldier.FindPSByPhotonPlayer(otherPlayer));
            Debug.Log($"{otherPlayer.NickName} left room");
        }
    }
}