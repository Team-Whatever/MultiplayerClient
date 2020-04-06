using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int id;
    public int unitId;
    public int teamId;

    public Vector3 position;
    public Quaternion rotation;
    public Color color;

    public float health;
    public float lastUpdateTime;

    public PlayerData( int _id, int _unitId, float maxHealth )
    {
        id = _id;
        unitId = _unitId;
        teamId = _unitId;
        color = new Color( Random.value, Random.value, Random.value );
        position = new Vector3( Random.value - 0.5f, 0.0f, Random.value - 0.5f ) * 5.0f;
        rotation = Quaternion.identity;
        health = maxHealth;
    }

    public override string ToString()
    {
        return string.Format( "[{0}] pos = {1}, rotation = {2}", id, position, rotation );
    }
}

