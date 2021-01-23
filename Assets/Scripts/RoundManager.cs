using System;
using Resources.Classes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

public class RoundManager : MonoBehaviour, IOnEventCallback {
    public PhotonTeamsManager ptm;
    private Dictionary<byte, bool> teamsIsAlive = new Dictionary<byte, bool>();

    void Start() {
        SetDefaultValues();
    }

    void Update() {
        if (!PhotonNetwork.IsMasterClient) {
            return;
        }

        SetDefaultValues(); // todo remove this when players wouldn't connecting while round is going
        CheckForEndRound();
        if (Input.GetKeyDown(KeyCode.F1)) {
            PhotonNetwork.RaiseEvent(0, null, new RaiseEventOptions() {Receivers = ReceiverGroup.All},
                new SendOptions() {Reliability = true});
        }
    }

    void CheckForEndRound() {
        foreach (var teamID in teamsIsAlive.Keys.ToList()) {
            ptm.TryGetTeamByCode(teamID, out var team);
            if (PlayerSoldier.GetAllPSByTeam(team).Count == 0) return;
            if (PlayerSoldier.TeamIsDead(team)) {
                teamsIsAlive[teamID] = false;
                Debug.Log($"{team.Name} team is lose");
            }
            else if (teamsIsAlive.Values.Count(val => val) == 1) {
                Debug.Log($"{team.Name} team is win");
                PhotonNetwork.RaiseEvent(0, null, new RaiseEventOptions() {Receivers = ReceiverGroup.All},
                    new SendOptions() {Reliability = true});
                // todo calculate rounds stats
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