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
using UnityEngine.Events;
using System;

namespace PlayerScripts {
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable {
        private const float EPSILON = 0.00001f;
        private const float SLOWDOWN_TIME = 1f;
        private const float SLOWDOWN_COEF = 0.5f;
        public CircleCollider2D collider;
        public Weapon weapon;
        public Weapon AK47;
        public Weapon AWP;
        public Weapon ShotGun;
        public Weapon P2000;
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
        public ParticleSystem blood;
        private InGameManager gameManager;
        private PhotonView photonView;
        private float moveAnimSpeed;
        private InGameCanvasController canvasController;
        private IEnumerator moveSoundsEnumerator;
        [SerializeField] private float health;

        private float slowdownTimer = 0;
        private float moveCoef;

        public List<Weapon> arsenal;
        public event Action<int> IAmswitchWeapon;
        public int currIdWeapon;


        void Start() {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InGameManager>();
            canvasController = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<InGameCanvasController>();
            photonView = GetComponent<PhotonView>();
            NicknameText.SetText(photonView.Owner.NickName);
            moveSoundsEnumerator = moveSounds.GetEnumerator();
            AddIntanceOfWeaponToArsenal();
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
            SelecteWeapon();
        }

        private void SynchronizeNetworkVariables() {
            if (photonView.IsMine) {
                health = PlayerSoldier.localPlayer.health;
            }
            else {
                PlayerSoldier.FindPSByPhotonView(photonView).health = health;
            }
        }

        private void AddIntanceOfWeaponToArsenal() {
            arsenal.Add(Instantiate(P2000));
            arsenal.Add(Instantiate(ShotGun));
            arsenal.Add(Instantiate(AK47));
            arsenal.Add(Instantiate(AWP));
        }

        public Weapon GetElementFromArsenalById(int ID) {
            var i = 0;
            foreach (var weapon in arsenal) {
                if (weapon.weaponID == ID) {
                    return arsenal[i];
                }
                i++;    
            }
            return arsenal[0];
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
            moveCoef = CalculateMoveCoef();
            transform.position += posChange * (moveSpeed * Time.deltaTime) * moveCoef;
            slowdownTimer -= Time.deltaTime;
           
        }

        private float CalculateMoveCoef()
        {
            return slowdownTimer >= 0 ? SLOWDOWN_COEF : 1f;
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
                PhotonTeamExtensions.GetPhotonTeam(photonView.Owner), arsenal[0], soldier, gameObject, this);
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
        
        // InGameCanvasController end  

        public void isDamaged() {
            slowdownTimer = SLOWDOWN_TIME;
        }

        private void SelecteWeapon() {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currIdWeapon = 0;
                IAmswitchWeapon?.Invoke(currIdWeapon);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                currIdWeapon = 1;
                IAmswitchWeapon?.Invoke(currIdWeapon);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                currIdWeapon = 2;
                IAmswitchWeapon?.Invoke(currIdWeapon);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                currIdWeapon = 3;
                IAmswitchWeapon?.Invoke(currIdWeapon);;
            }


        }
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