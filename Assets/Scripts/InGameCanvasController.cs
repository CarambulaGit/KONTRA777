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
    public Text CurrentAmmo;
    public Text NumOfBullets;
    public CanvasStatus canvasStatus;
    private bool init;
    private bool isReloading;

    void Start() {
        canvasStatus = CanvasStatus.StartGameMenu;
        OnChangedCanvasStatus();
        SetNecessaryStartGameMenu();
        WeaponController.IAmReloading += ReloadTimer;
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvasStatus = canvasStatus == CanvasStatus.EscMenu ? 0 : CanvasStatus.EscMenu;
            OnChangedCanvasStatus();
        }

        if (PlayerSoldier.localPlayer == null) return;

        if (!init) {
            healthBar.maxValue = PlayerSoldier.localPlayer.soldier.health;
            fill.color = gradient.Evaluate(1f);
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
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().OnStartGame();
    }

    private void SetNecessaryStartGameMenu() {
        sGMHost.SetActive(PhotonNetwork.IsMasterClient);
        sGMOthers.SetActive(!PhotonNetwork.IsMasterClient);
    }

    public void ReloadTick() {
        if (!PlayerSoldier.localPlayer.weapon.isReloading)
        {
            reloadTimer.fillAmount = 1;
            reloadTimer.enabled = false;
        }
    }

    public void Tick() {
        ReloadTick();
        HealthTick();
        AmmoTick();
    }

    public void HealthTick()
    {
        healthBar.value = PlayerSoldier.localPlayer.health;
        fill.color = gradient.Evaluate(healthBar.normalizedValue);
    }

    public void AmmoTick() {
        CurrentAmmo.text = PlayerSoldier.localPlayer.weapon.currentAmmo.ToString();
        NumOfBullets.text = PlayerSoldier.localPlayer.weapon.numOfBullets.ToString();
    }

    public void ReloadTimer() {
        if (PlayerSoldier.localPlayer.weapon.isReloading) reloadTimer.enabled = true;
        reloadTimer.fillAmount -= Time.deltaTime/PlayerSoldier.localPlayer.weapon.reloadTime;
    }

}