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
    public CanvasStatus canvasStatus;
    private bool init;

    void Start() {
        
        canvasStatus = CanvasStatus.StartGameMenu;
        OnChangedCanvasStatus();
        SetNecessaryStartGameMenu();
    }

    void Update() {
        if (!init && PlayerSoldier.localPlayer != null) {
            healthBar.maxValue = PlayerSoldier.localPlayer.soldier.health;
            fill.color = gradient.Evaluate(1f);
            init = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            canvasStatus = CanvasStatus.EscMenu;
            OnChangedCanvasStatus();
        }

        if (init) {
            healthBar.value = PlayerSoldier.localPlayer.health;
            fill.color = gradient.Evaluate(healthBar.normalizedValue);
        }
       
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
}