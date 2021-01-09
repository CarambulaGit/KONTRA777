using System.Collections.Generic;
using Photon.Pun;
using Resources.Classes;
using ServerScripts;
using Unity.Collections;
using UnityEngine;

namespace PlayerScripts {
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable {
        private const float EPSILON = 0.00001f;
        public BoxCollider2D collider;

        public float moveSpeed;
        public Animator animator;
        private PhotonView photonView;
        private float moveAnimSpeed;
        public bool isDead { get; private set;}
        [ReadOnly] private float health;
        [ReadOnly] private float takenDamage;
        [ReadOnly] private float damage;

        void Start() {
            photonView = GetComponent<PhotonView>();
            health = PlayerSoldier.defaultHealth;
        }

        void FixedUpdate() {
            if (!photonView.IsMine) return;

            //InGameManager.localPlayer.health = health;
            //InGameManager.localPlayer.TakeDamage(InGameManager.localPlayer.takenDamageThisTick);
            
            if (isDead) return;
            isDead = InGameManager.localPlayer.IsDead();

            if (isDead) {
                collider.enabled = false;
                health = InGameManager.localPlayer.health;
                moveAnimSpeed = 0;
                Animate();
                return;
            }


            Move(out moveAnimSpeed);
            Animate();
            health = InGameManager.localPlayer.health;
            // PhotonView.Find()
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

        private List<PlayerSoldier> initPlayerSoldiers() {
            List<PlayerSoldier> playerSoldiers = new List<PlayerSoldier>();
            
            return playerSoldiers;
        } 


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting) {
                stream.SendNext(health);
                stream.SendNext(takenDamage);
                stream.SendNext(damage);
            }
            else {
                this.health = (float) stream.ReceiveNext();
                this.takenDamage = (float) stream.ReceiveNext();
                this.damage = (float) stream.ReceiveNext();
            }
        }

        void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.tag.Equals("Box")) {
                InGameManager.localPlayer.Kill();
            }
        }
    }
}