using UnityEngine;

[System.Serializable]
public class DistrictData
{
    public string Name;
    public int Influence;

    public DistrictData(District district)
    {
        Influence = district.Influence;
        Name = district.Name;
    }
}