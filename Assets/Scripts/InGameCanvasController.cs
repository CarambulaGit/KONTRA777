using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Resources.Classes;
using UnityEngine;

public class InGameCanvasController : MonoBehaviour {
    public enum CanvasStatus : ushort {
        StartGameMenu = 1,
        EscMenu = 2
    }

    public bool isReady { get; private set; }
    public GameObject startGameMenu;
    public GameObject sGMHost;
    public GameObject sGMOthers;
    public GameObject escMenu;
    private CanvasStatus canvasStatus;

    [PunRPC]
    private void StartGameRPC() {
        isReady = true;
        canvasStatus = canvasStatus == CanvasStatus.StartGameMenu ? 0 : canvasStatus;
        OnChangedCanvasStatus();
    }

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

    void OnChangedCanvasStatus() {
        startGameMenu.SetActive(canvasStatus == CanvasStatus.StartGameMenu);
        escMenu.SetActive(canvasStatus == CanvasStatus.EscMenu);
    }

    public void OnLeave() {
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGame() {
        PlayerSoldier.localPlayer.photonView.RPC(nameof(StartGameRPC), RpcTarget.AllBuffered, null);
    }

    private void SetNecessaryStartGameMenu() {
        sGMHost.SetActive(PhotonNetwork.IsMasterClient);
        sGMOthers.SetActive(!PhotonNetwork.IsMasterClient);
    }
}