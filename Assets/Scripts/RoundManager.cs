﻿using System;
using Resources.Classes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class RoundManager : MonoBehaviour, IOnEventCallback {
    public string canvasTag = "InGameCanvas";
    public PhotonTeamsManager ptm;
    public event Action<float> startTimer;
    public float timeOfRound = 120;
    public bool timerIsRunning;
    private Dictionary<byte, bool> teamsIsAlive = new Dictionary<byte, bool>();
    private InGameCanvasController canvasController;

    public int redTeamScore { get; set; }
    public int blueTeamScore { get; set; }

    void Start() {
        SetDefaultValues();
        canvasController = GameObject.FindWithTag(canvasTag).GetComponent<InGameCanvasController>();
    }

    void Update() {
        if (!PhotonNetwork.IsMasterClient) {
            return;
        }

        if (!canvasController.isReady) {
            return;
        }
        
        SetDefaultValues(); // todo remove this when players wouldn't connecting while round is going, if leave 
        CheckForEndRound();
        if (Input.GetKeyDown(KeyCode.F1)) {
            PhotonNetwork.RaiseEvent(0, null, new RaiseEventOptions() {Receivers = ReceiverGroup.All},
                new SendOptions() {Reliability = true});
        }
    }

    void CheckForEndRound() {
        foreach (var teamID in teamsIsAlive.Keys.ToList()) {
            ptm.TryGetTeamByCode(teamID, out var team);
            if (ptm.GetTeamMembersCount(team) == 0) return;
            if (PlayerSoldier.TeamIsDead(team)) {
                teamsIsAlive[teamID] = false;
                Debug.Log($"{team.Name} team is lose");
                redTeamScore++;
            }
            else if (teamsIsAlive.Values.Count(val => val) == 1) {
                Debug.Log($"{team.Name} team is win");
                PhotonNetwork.RaiseEvent(0, null, new RaiseEventOptions() {Receivers = ReceiverGroup.All},
                    new SendOptions() {Reliability = true});
                blueTeamScore++;
                // todo calculate rounds stats
            }
            if (redTeamScore > 15 || blueTeamScore > 15) { 
                //start new game 
            }

        }
    }

    public void OnEvent(EventData photonEvent) {
        switch (photonEvent.Code) {
            case 0:
                StartNewRound();
                break;
        }
    }

    void SetDefaultValues() {
        teamsIsAlive.Clear();
        foreach (var team in ptm.GetAvailableTeams()) {
            teamsIsAlive.Add(team.Code, true);
        }
    }

    public void StartTimerTick() {
        timerIsRunning = true;
        startTimer?.Invoke(timeOfRound);
    }


    private void StartNewRound() {
        Debug.Log("New Round");
        PlayerSoldier.players.ForEach(player => player.playerController.SetDefaultState());
        // todo set default values of round like time and etc.
    }

    private void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}