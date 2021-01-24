using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using PlayerScripts;
using Resources.Classes;
using UnityEngine;
using UnityEngine.UI;

public class InGameCanvasController : MonoBehaviour {
    public enum CanvasStatus : ushort {
        StartGameMenu = 1,
        EscMenu = 2
    }

    public bool isReady { get; set; }
    public GameObject startGameMenu;
    public GameObject sGMHost;
    public GameObject sGMOthers;
    public GameObject escMenu;
    public Slider healthBar;
    public Gradient gradient;
    public Image fill;
    public Image reloadTimer;
    public Text healthText;
    public Text CurrentAmmo;
    public Text NumOfBullets;
    public Text minutesText;
    public Text secondsText;
    public Text redTeamScore;
    public Text blueTeamScore;
    public CanvasStatus canvasStatus;
    public Texture2D cursorImage;
    public float currSecond;
    public RoundManager roundManager;
    public CursorMode cursorMode = CursorMode.Auto;

    private bool init;
    private bool isReloading;

    void Start() {
        canvasStatus = CanvasStatus.StartGameMenu;
        OnChangedCanvasStatus();
        SetNecessaryStartGameMenu();
        roundManager.startTimer += TimerTick;
        currSecond = roundManager.timeOfRound;
        Cursor.SetCursor(cursorImage, new Vector2(29, 35), cursorMode);
    }

    private void TimerTick(float time) {
        if (currSecond == 0) {
            roundManager.timerIsRunning = false;
            currSecond = time;
            return;
        }

        if (currSecond < 10) { 
            secondsText.color = new Color(255, 0, 0);
            minutesText.color = new Color(255, 0, 0);
        }

        var t = System.TimeSpan.FromSeconds(currSecond);

        minutesText.text = (t.Minutes < 10) ? "0" + (t.Minutes).ToString() : (t.Minutes).ToString();
        secondsText.text = (t.Seconds < 10) ? "0" + (t.Seconds).ToString() : (t.Seconds).ToString();

        currSecond =Mathf.Clamp(currSecond - Time.deltaTime, 0, time);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            canvasStatus = canvasStatus == CanvasStatus.EscMenu ? 0 : CanvasStatus.EscMenu;
            OnChangedCanvasStatus();
        }

        if (PlayerSoldier.localPlayer == null) return;

        if (!init) {
           
            healthBar.maxValue = PlayerSoldier.localPlayer.soldier.health;
            fill.color = gradient.Evaluate(1f);
            PlayerSoldier.localPlayer.gOPlayer.GetComponent<WeaponController>().IAmReloading += ReloadTimer;
            init = true;
        }
        Tick();
    }

    public void OnChangedCanvasStatus() {
        // startGameMenu.SetActive(canvasStatus == CanvasStatus.StartGameMenu);
        startGameMenu.SetActive(false); // todo
        escMenu.SetActive(canvasStatus == CanvasStatus.EscMenu);
    }

    public void OnLeave() {
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGame() {
        if (PlayerSoldier.localPlayer == null) return;
        PlayerSoldier.localPlayer.playerController.OnStartGame();
    }

    private void SetNecessaryStartGameMenu() {
        sGMHost.SetActive(PhotonNetwork.IsMasterClient);
        sGMOthers.SetActive(!PhotonNetwork.IsMasterClient);
    }

    private void Tick() {
        if (PlayerSoldier.localPlayer == null) {
            return;
        }

        ReloadTick();
        HealthTick();
        AmmoTick();
        roundManager.StartTimerTick();
    }

    private void ReloadTick() {
        if (!PlayerSoldier.localPlayer.weapon.isReloading) {
            reloadTimer.fillAmount = 0;
            reloadTimer.gameObject.SetActive(false);
        }
    }


    private void HealthTick() {
        healthText.text = PlayerSoldier.localPlayer.health.ToString();
        healthBar.value = PlayerSoldier.localPlayer.health;
        fill.color = gradient.Evaluate(healthBar.normalizedValue);
    }

    private void AmmoTick() {
        CurrentAmmo.text = PlayerSoldier.localPlayer.weapon.currentAmmo.ToString();
        NumOfBullets.text = PlayerSoldier.localPlayer.weapon.numOfBullets.ToString();
    }

    private void ReloadTimer() {
        if (PlayerSoldier.localPlayer.weapon.isReloading) reloadTimer.gameObject.SetActive(true);
        reloadTimer.fillAmount += Time.deltaTime / PlayerSoldier.localPlayer.weapon.reloadTime;
    }
}


