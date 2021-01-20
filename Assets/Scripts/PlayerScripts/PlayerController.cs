using System.Collections;
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
using System.IO;
using UnityEditor;

namespace PlayerScripts {
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable {
        private const float EPSILON = 0.00001f;
        public CircleCollider2D collider;
        public Weapon weapon;
        public Soldier soldier;
        public float moveSpeed;
        public Animator animator;
        public TextMeshPro NicknameText;
        public SpriteRenderer sprite;
        public AudioClip deathSound;
        public AudioSource audio;
        public AudioClip[] moveSounds = new AudioClip[2];
        public bool init;
        public bool isDead { get; private set; }
        private InGameManager gameManager;
        private PhotonView photonView;
        private float moveAnimSpeed;
        private InGameCanvasController canvasController;
        private IEnumerator moveSoundsEnumerator;
        [SerializeField] private float health;
        
        private int slowdownTime = 60;
        private float slowdownCoef = 0.5f;
        private int counter = 0;


        void Start() {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InGameManager>();
            canvasController = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<InGameCanvasController>();
            photonView = GetComponent<PhotonView>();
            NicknameText.SetText(photonView.Owner.NickName);
            moveSoundsEnumerator = moveSounds.GetEnumerator();
        }

        void FixedUpdate() {
            if (!init) {
                initPlayerSoldier();
                init = true;
            }

            // if (!canvasController.isReady) return;
            SynchronizeNetworkVariables();

            if (!photonView.IsMine) return;

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
            if (photonView.IsMine) {
                health = PlayerSoldier.localPlayer.health;
            }
            else {
                PlayerSoldier.FindPSByPhotonView(photonView).health = health;
            }
        }

        private void Kill() {
            photonView.RPC(nameof(KillRPC), RpcTarget.AllBuffered, photonView.ViewID);
            health = PlayerSoldier.localPlayer.health;
            moveAnimSpeed = 0;
            Animate();
        }

        private void Move(out float animSpeed) {
            var posChange = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            if (posChange.magnitude > Vector3.right.magnitude) {
                posChange = posChange.normalized;
            }

            if (posChange.magnitude > 0.1f) {
                photonView.RPC(nameof(MoveRPC), RpcTarget.All, photonView.ViewID);
            }
            animSpeed = posChange.magnitude;
            if (counter > 0)
            {
                transform.position += posChange * (moveSpeed * Time.deltaTime) * slowdownCoef;
                counter--;
            }
            else
            {
                transform.position += posChange * (moveSpeed * Time.deltaTime);

            }
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
                PhotonTeamExtensions.GetPhotonTeam(photonView.Owner), Instantiate(weapon), soldier, gameObject, this);
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

        void OnCollisionEnter2D(Collision2D other) { }


        [PunRPC]
        void KillRPC(int viewId) {
            if (photonView.ViewID == viewId) {
                collider.enabled = false;
                sprite.sortingOrder = 1;
                audio.clip = deathSound;
                audio.Play();
            }
        }

        [PunRPC]
        private void MoveRPC(int viewIdWhoMove) {
            if (photonView.ViewID == viewIdWhoMove) {
                if (!audio.isPlaying) {
                    moveSoundsEnumerator.MoveNextCycled();
                    audio.clip = moveSoundsEnumerator.Current as AudioClip;
                    audio.Play();
                }
            }
        }

        // InGameCanvasController start

        [PunRPC]
        private void StartGameRPC() {
            canvasController.isReady = true;
            canvasController.canvasStatus =
                canvasController.canvasStatus == InGameCanvasController.CanvasStatus.StartGameMenu
                    ? 0
                    : canvasController.canvasStatus;
            canvasController.OnChangedCanvasStatus();
        }

        public void OnStartGame() {
            photonView.RPC(nameof(StartGameRPC), RpcTarget.AllBuffered, null);
        }

        public void isDamaged()
        {
            counter += slowdownTime;
        }

        // InGameCanvasController end  
    }
}

public static class IEnumeratorExtension {
    public static void MoveNextCycled(this IEnumerator enumerator) {
        if (!enumerator.MoveNext()) {
            enumerator.Reset();
            enumerator.MoveNext();
        }
    }
}