﻿using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string id;
    public int unitId;
    public int teamId;

    public Vector3 position;
    public Quaternion rotation;
    public float verticalRotation;

    public float health;
    public float lastUpdateTime;
    public List<PlayerCommand> commands;

    public PlayerData( string _id, int _unitId, float maxHealth )
    {
        id = _id;
        unitId = _unitId;
        teamId = _unitId;
        position = new Vector3( Random.value - 0.5f, 0.0f, Random.value - 0.5f ) * 5.0f;
        rotation = Quaternion.identity;
        health = maxHealth;
    }

    public override string ToString()
    {
        return string.Format( "[{0}] pos = {1}, rotation = {2}", id, position, rotation );
    }
}

