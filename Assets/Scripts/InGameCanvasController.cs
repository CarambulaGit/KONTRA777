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
    public CanvasStatus canvasStatus;
    public Texture2D cursorImage;
    private bool init;
    private bool isReloading;

    public const float minutesStart = 2;
    public float currSecond;
    public float currMinutes;

    public CursorMode cursorMode = CursorMode.Auto;

    void Start() {
        currMinutes = minutesStart;
        currSecond = 0;
        canvasStatus = CanvasStatus.StartGameMenu;
        OnChangedCanvasStatus();
        SetNecessaryStartGameMenu();

        Cursor.SetCursor(cursorImage, new Vector2(29, 35), cursorMode);
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
        TimerTick();
    }

    private void ReloadTick() {
        if (!PlayerSoldier.localPlayer.weapon.isReloading) {
            reloadTimer.fillAmount = 0;
            reloadTimer.gameObject.SetActive(false);
        }
    }

    public void TimerTick()
    {
        if (currMinutes <= 0 && currSecond <= 0) return;
        if (currSecond <= 0) {
            currMinutes--;
            currSecond = 60;
            if(currMinutes < 10) minutesText.text = "0"+currMinutes.ToString();
            minutesText.text = currMinutes.ToString();
        }
        currSecond -= Time.deltaTime;
        if (currSecond < 10) {
            if (currMinutes == 0) {
                secondsText.color = new Color(255,0,0);
                minutesText.color = new Color(255, 0, 0);
            } 
            secondsText.text = "0"+System.Math.Round(currSecond).ToString();
            return;
        }
        secondsText.text = System.Math.Round(currSecond).ToString();
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


