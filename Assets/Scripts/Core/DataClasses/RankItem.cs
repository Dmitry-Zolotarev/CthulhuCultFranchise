using UnityEngine;

public enum RankItems
{
    SecondFlowPill,
    RedGrass,
    ExternalPill,
    PeakFlower,
}

[System.Serializable]
public class RankItem
{
    public bool Unlocked;
    public RankItems ID = RankItems.SecondFlowPill; 
    public string name = "";
}
