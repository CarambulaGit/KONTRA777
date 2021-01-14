using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using PlayerScripts;
using Resources.Classes;
using UnityEngine;

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
    public CanvasStatus canvasStatus;

    void Start() {
        canvasStatus = CanvasStatus.StartGameMenu;
        OnChangedCanvasStatus();
        SetNecessaryStartGameMenu();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            canvasStatus = CanvasStatus.EscMenu;
            OnChangedCanvasStatus();
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