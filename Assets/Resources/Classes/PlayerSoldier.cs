using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PlayerScripts;
using UnityEngine;

namespace Resources.Classes {
    public class PlayerSoldier {
        public static List<PlayerSoldier> players = new List<PlayerSoldier>();
        public static PlayerSoldier localPlayer;
        public float health;
        public Soldier soldier;
        public Weapon weapon;
        public string nickname { get; set; }
        private Player photonPlayer;
        private PhotonTeam team;
        public GameObject gOPlayer;
        public PhotonView photonView;
        public PlayerController playerController;

        public static PlayerSoldier FindPSByPhotonPlayer(Player player) {
            return players.First(somePlayer => somePlayer.photonPlayer.Equals(player));
        }

        public static PlayerSoldier FindPSByPlayerGO(GameObject gameObject) {
            return players.First(somePlayer => somePlayer.gOPlayer.Equals(gameObject));
        }

        public static PlayerSoldier FindPSByNickname(string nickname) {
            return players.First(somePlayer => somePlayer.nickname.Equals(nickname));
        }

        public static PlayerSoldier FindPSByPhotonView(PhotonView pv) {
            return players.First(somePlayer => somePlayer.photonView.ViewID == pv.ViewID);
        }

        public static List<PlayerSoldier> GetAllPSByTeam(PhotonTeam team) {
            return players.FindAll(somePlayer => somePlayer.team.Equals(team));
        }

        public static bool PlayersIsDead(List<PlayerSoldier> players) {
           return players.All(somePlayer => somePlayer.IsDead());
        }

        public static bool TeamIsDead(PhotonTeam team) {
            return PlayersIsDead(GetAllPSByTeam(team));
        }

        public PlayerSoldier(Player photonPlayer, string nickname, PhotonTeam team, Weapon weapon, Soldier soldier, GameObject gOPlayer, PlayerController playerController) {
            this.photonPlayer = photonPlayer;
            this.nickname = nickname;
            this.team = team;
            this.weapon = weapon;
            this.gOPlayer = gOPlayer;
            this.soldier = soldier;
            this.health = soldier.health;
            this.photonView = this.gOPlayer.GetPhotonView();
            this.playerController = playerController;
            players.Add(this);
        }

        public bool IsDead() => this.health <= 0;
        public void Kill() => this.health = 0;

        public void TakeDamage(float damage) {
            this.health = Mathf.Clamp(health - damage, 0, soldier.health); 
        }

        public override string ToString() {
            return $"{photonPlayer}\n{nickname}\n{health}\n{team}\n";
        }
    }
}