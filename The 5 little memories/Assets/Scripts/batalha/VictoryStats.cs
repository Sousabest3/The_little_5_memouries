using UnityEngine;

[System.Serializable]
public class VictoryStats
{
    public string characterName;
    public int xpGained;
    public int levelBefore;
    public int levelAfter;
    public int hpBefore;
    public int hpAfter;
    public int atkBefore;
    public int atkAfter;

    public bool LeveledUp => levelAfter > levelBefore;
}
