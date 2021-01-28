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
        public SpriteRenderer playerTeam;
        public bool init;
        public bool isDead { get; private set; }
        public ParticleSystem blood;
        public Color redTeam = new Color(255, 56, 83);
        public Color blueTeam = new Color(56, 89, 255);
        private InGameManager gameManager;
        private PhotonView photonView;
        private float moveAnimSpeed;
        private InGameCanvasController canvasController;
        private RoundManager roundManager;
        private IEnumerator moveSoundsEnumerator;
        [SerializeField] private float health;

        private float slowdownTimer = 0;
        private float moveCoef;

        public List<Weapon> arsenal;

        private KeyCode[] SwitchWeponKey = {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4
        };

        public event Action<int> IAmswitchWeapon;
        public int currIdWeapon;


        void Start() {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InGameManager>();
            canvasController = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<InGameCanvasController>();
            roundManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<RoundManager>();
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

            if (!canvasController.isReady) return;
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
            SelectWeapon();
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
            arsenal.Add(Instantiate(AK47));
            arsenal.Add(Instantiate(ShotGun));
            arsenal.Add(Instantiate(P2000));
            arsenal.Add(Instantiate(AWP));
        }

        public Weapon GetElementFromArsenalById(int ID) {
            return arsenal.First(weapon => weapon.weaponID == ID) ?? arsenal[0];
        }

        private void Kill() {
            photonView.RPC(nameof(KillRPC), RpcTarget.AllBuffered, photonView.ViewID);
            health = PlayerSoldier.localPlayer.health;
            moveAnimSpeed = 0;
            Animate();
        }

        [PunRPC]
        void KillRPC(int viewId) {
            if (photonView.ViewID == viewId) {
                collider.enabled = false;
                sprite.sortingOrder = 1;
                audio.clip = deathSound;
                audio.Play();
            }
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
            transform.position += posChange * moveSpeed * moveCoef * Time.deltaTime;
            slowdownTimer -= Time.deltaTime;
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

        private float CalculateMoveCoef() {
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

            if (player.team == gameManager.ptm.GetAvailableTeams()[0])
            {
                playerTeam.color = Color.Lerp(Color.blue, blueTeam, 1f);
            }
            else
            {
                playerTeam.color = Color.Lerp(Color.red, redTeam, 1f);
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

        public void isDamaged() {
            slowdownTimer = SLOWDOWN_TIME;
        }

        private void SelectWeapon() {
            for (int i = 0; i < SwitchWeponKey.Length; i++) {
                if (Input.GetKeyDown(SwitchWeponKey[i])) {
                    currIdWeapon = i + 1;
                    IAmswitchWeapon?.Invoke(currIdWeapon);
                }
            }
        }

        public void SetDefaultState() {
            transform.position = gameManager.SpawnPoints[PlayerSoldier.localPlayer.team.Code - 1].position;
            PlayerSoldier.localPlayer.weapon = Instantiate(weapon);
            PlayerSoldier.localPlayer.health = soldier.health;
            isDead = false;
            collider.enabled = true;
            sprite.sortingOrder = 2;
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
}