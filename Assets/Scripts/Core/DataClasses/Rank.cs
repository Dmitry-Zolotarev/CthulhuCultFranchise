
public enum MasterRank
{
    ThirdRate,
    SecondRate,
    FirstRate,
    PeakMaster,
    OneFlower
}
[System.Serializable]
public class Rank
{
    public string Name;
    public int NeedMeridians;
    public int MaxBody;
    public RankItems RequiredItem;
}
