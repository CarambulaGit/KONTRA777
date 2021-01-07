using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

namespace Resources.Classes {
    public class PlayerSoldier {
        public static float defaultHealth = 100;
        public static List<PlayerSoldier> players = new List<PlayerSoldier>();
        public float health;
        private Player photonPlayer;
        private string nickname;
        private PhotonTeam team;
        private float damage;
        private GameObject gOPlayer;
        public float takenDamageThisTick;

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

        public void TakeDamage(float damage) {
            this.health -= damage;
        }
    }
}