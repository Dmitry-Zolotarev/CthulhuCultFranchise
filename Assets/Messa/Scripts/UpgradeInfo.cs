using UnityEngine;

[System.Serializable]
public class UpgradeInfo
{ 
    public string Header;
    public string Description;
    public int Price;
    public Sprite Icon;

    [HideInInspector] public bool Unlocked;
}
