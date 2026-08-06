using UnityEngine;

[System.Serializable]
public class Technique
{
    public int Type = 0;
    public string Name = "";
    public int RequiredRank = 0;
    public int Price = 500;
    public int NeedQi = 20;
    public int Damage = 20;
    public Sprite Icon;
}
