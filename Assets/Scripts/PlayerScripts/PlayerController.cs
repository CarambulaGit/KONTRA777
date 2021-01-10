using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Resources.Classes;
using ServerScripts;
using TMPro;
using Unity.Collections;
using UnityEngine;

namespace PlayerScripts {
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable {
        private const float EPSILON = 0.00001f;
        public BoxCollider2D collider;
        public float moveSpeed;
        public Animator animator;
        public TextMeshPro NicknameText;
        public bool isDead { get; private set; }
        private InGameManager gameManager;
        private PhotonView photonView;
        private float moveAnimSpeed;
        private bool init;
        [ReadOnly] private float health;
        [ReadOnly] private float takenDamageThisTick;
        [ReadOnly] private float damage;

        void Start() {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InGameManager>();
            photonView = GetComponent<PhotonView>();
            health = PlayerSoldier.defaultHealth;
            NicknameText.SetText(PhotonNetwork.NickName);
        }

        void FixedUpdate() {
            if (!init) {
                initPlayerSoldier();
                init = true;
            }

            if (!photonView.IsMine) return;
           
           // Debug.Log(PlayerSoldier.players.ToArray().ToStringFull());
            SynchronizeNetworkVariables();
            if (isDead) return;
            isDead = PlayerSoldier.localPlayer.IsDead();
            if (isDead) {
                Kill();
                return;
            }

            Tick();
            Move(out moveAnimSpeed);
            Animate();
            health = PlayerSoldier.localPlayer.health;
            // PhotonView.Find()
        }

        private void SynchronizeNetworkVariables() {
            PlayerSoldier.localPlayer.health = health;
            PlayerSoldier.localPlayer.takenDamageThisTick = takenDamageThisTick;
            PlayerSoldier.localPlayer.damage = damage;
        }


        private void Tick() {
            PlayerSoldier.localPlayer.TakeDamage(PlayerSoldier.localPlayer.takenDamageThisTick);
        }

        private void Kill() {
            collider.enabled = false;
            health = PlayerSoldier.localPlayer.health;
            moveAnimSpeed = 0;
            Animate();
        }

        private void Move(out float animSpeed) {
            var posChange = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            if (posChange.magnitude > Vector3.right.magnitude) {
                posChange = posChange.normalized;
            }

            animSpeed = posChange.magnitude;
            transform.position += posChange * (moveSpeed * Time.deltaTime);
        }


        private void Animate() {
            AnimateMove();
            AnimateDeath();
        }

        private void AnimateMove() {
            animator.SetFloat("MoveSpeed", moveAnimSpeed);
        }

        private void AnimateDeath() {
            animator.SetBool("IsDead", isDead);
        }

        private PlayerSoldier initPlayerSoldier() {
            var player = new PlayerSoldier(photonView.Owner, photonView.Owner.NickName,
                PhotonTeamExtensions.GetPhotonTeam(photonView.Owner), 10, gameObject);
            if (photonView.IsMine) {
                PlayerSoldier.localPlayer = player;
            }

            return player;
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting) {
                stream.SendNext(health);
                stream.SendNext(takenDamageThisTick);
                stream.SendNext(damage);
            }
            else {
                this.health = (float) stream.ReceiveNext();
                this.takenDamageThisTick = (float) stream.ReceiveNext();
                this.damage = (float) stream.ReceiveNext();
            }
        }

        void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.tag.Equals("Box")) {
                PlayerSoldier.localPlayer.Kill();
                Kill();
            }
        }
    }
}