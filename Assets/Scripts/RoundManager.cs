﻿using System;
using Resources.Classes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class RoundManager : MonoBehaviour {
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
        foreach (var teamID in teamsIsAlive.Keys.ToList()) {
            ptm.TryGetTeamByCode(teamID, out var team);
            if (PlayerSoldier.GetAllPSByTeam(team).Count == 0) return;
            if (PlayerSoldier.TeamIsDead(team)) {
                teamsIsAlive[teamID] = false;
                Debug.Log($"{team.Name} team is lose");
            }
            else if (teamsIsAlive.Values.Count(val => val) == 1) {
                Debug.Log($"{team.Name} team is win");
            }
        }
    }

    void SetDefaultValues() {
        teamsIsAlive.Clear();
        foreach (var team in ptm.GetAvailableTeams()) {
            teamsIsAlive.Add(team.Code, true);
        }
    }
}