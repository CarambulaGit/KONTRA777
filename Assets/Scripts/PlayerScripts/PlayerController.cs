using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Resources;
using Resources.Classes;
using ServerScripts;
using TMPro;
using Unity.Collections;
using UnityEngine;

namespace PlayerScripts {
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable {
        private const float EPSILON = 0.00001f;
        public BoxCollider2D collider;
        public Weapon weapon;
        public Soldier soldier;
        public float moveSpeed;
        public Animator animator;
        public TextMeshPro NicknameText;
        public bool isDead { get; private set; }
        private InGameManager gameManager;
        private PhotonView photonView;
        private float moveAnimSpeed;
        private bool init;
        [SerializeField] private float health;

        void Start() {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InGameManager>();
            photonView = GetComponent<PhotonView>();
            NicknameText.SetText(photonView.Owner.NickName);
        }

        void FixedUpdate() {
            if (!init) {
                initPlayerSoldier();
                init = true;
            }

            if (!photonView.IsMine) return;

            SynchronizeNetworkVariables();
            if (isDead) return;
            isDead = PlayerSoldier.localPlayer.IsDead();
            if (isDead) {
                Kill();
                return;
            }

            Move(out moveAnimSpeed);
            Animate();
        }

        private void SynchronizeNetworkVariables() {
            health = PlayerSoldier.localPlayer.health;
        }

        private void Kill() {
            photonView.RPC(nameof(DisableColliderRPC),RpcTarget.AllBuffered, photonView.ViewID);
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
                PhotonTeamExtensions.GetPhotonTeam(photonView.Owner), weapon, soldier, gameObject);
            if (photonView.IsMine) {
                PlayerSoldier.localPlayer = player;
            }

            return player;
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting) {
                stream.SendNext(health);
            }
            else {
                this.health = (float) stream.ReceiveNext();
            }
        }

        void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.tag.Equals("Box")) {
                PlayerSoldier.localPlayer.Kill();
                Kill();
            }
        }

        [PunRPC]
        void DisableColliderRPC(int viewId) {
            if (photonView.ViewID == viewId) {
                collider.enabled = false;
            }
        }
    }
}