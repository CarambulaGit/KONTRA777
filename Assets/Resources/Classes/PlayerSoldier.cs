using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

namespace Resources.Classes {
    public class PlayerSoldier {
        public static float defaultHealth = 100;
        public static List<PlayerSoldier> players = new List<PlayerSoldier>();
        public static PlayerSoldier localPlayer;
        public float health;
        public float damage;
        public float takenDamageThisTick;
        public string nickname { get; set; }
        private Player photonPlayer;
        private PhotonTeam team;
        private GameObject gOPlayer;

        public static PlayerSoldier FindPSByPhotonPlayer(Player player) {
            return players.First(somePlayer => somePlayer.photonPlayer.Equals(player));
        }

        public static PlayerSoldier FindPSByPlayerGO(GameObject gameObject) {
            return players.First(somePlayer => somePlayer.gOPlayer.Equals(gameObject));
        }

        public static PlayerSoldier FindPSByNickname(string nickname) {
            return players.First(somePlayer => somePlayer.nickname.Equals(nickname));
        }

        public PlayerSoldier(Player photonPlayer, string nickname, PhotonTeam team, float damage, GameObject gOPlayer,
            float health, float takenDamageThisTick = 0) {
            this.photonPlayer = photonPlayer;
            this.nickname = nickname;
            this.team = team;
            this.damage = damage;
            this.gOPlayer = gOPlayer;
            this.health = health;
            this.takenDamageThisTick = takenDamageThisTick;
            players.Add(this);
        }

        public PlayerSoldier(Player photonPlayer, string nickname, PhotonTeam team, float damage, GameObject gOPlayer) :
            this(photonPlayer, nickname, team, damage, gOPlayer, defaultHealth, 0) {
        }

        public bool IsDead() => this.health <= 0;
        public void Kill() => this.health = 0;

        public void TakeDamage(float damage) {
            this.health -= damage;
        }

        public override string ToString() {
            return $"{photonPlayer}\n{nickname}\n{health}\n{team}\n";
        }
    }
}