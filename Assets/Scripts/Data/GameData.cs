using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    public float MaxHealth;
    public float MaxSpeed;
    public float ShootRate;
}

[System.Serializable]
public class GameData
{
    [Header("Game")]
    public int CurrentSkin;

    [Header("Character Stats")]
    public CharacterStats characterStats;
}
