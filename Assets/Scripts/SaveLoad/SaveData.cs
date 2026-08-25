using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int Day = 1;
    public int Money = 100;
    public int Anxiety = 0;
    public int TimeSpeedModificator;
    public float Hunger = 0;
    public float Time;
    public string SelectedDistrictName = "";
    public GamePhase Phase = GamePhase.Map;
    public List<PersonData> Reserve = new List<PersonData>();
    public List<PersonData> ActiveWorkers = new List<PersonData>();
    public List<DistrictData> Districts = new List<DistrictData>();
    public List<int> RoomLevels = new List<int>();

    public void Load()
    {
        GameManager.Instance.Day = Day;
        GameManager.Instance.DayTime = Time;
        GameManager.Instance.Phase = Phase;
        GameManager.Instance.Money = Money;
        GameManager.Instance.Anxiety = Anxiety;
        GameManager.Instance.Hunger = Hunger;

        foreach (var district in GameManager.Instance.Districts)
        {
            foreach (var districtData in Districts)
            {
                if (districtData.Name == district.Name)
                {
                    district.Influence = districtData.Influence;
                    break;
                }
            }
            if (district.Name == SelectedDistrictName) GameManager.Instance.SelectedDistrict = district;
        }
        for (int i = 0; i < RoomLevels.Count; i++)
        {
            GameManager.Instance.Rooms[i].SetLevel(RoomLevels[i]);
        }

        foreach (var personData in Reserve)
        {
            var person = GameManager.Instance.SpawnVisitor();
            personData.Load(person);

            GameManager.Instance.Reserve.Add(person);
        }
        foreach (var personData in ActiveWorkers)
        {
            var worker = GameManager.Instance.SpawnVisitor();
            personData.Load(worker);
            GameManager.Instance.ActiveWorkers.Add(worker);
        }
    }
    public void Save()
    {
        Day = GameManager.Instance.Day;
        Time = GameManager.Instance.DayTime;
        Phase = GameManager.Instance.Phase;
        Money = GameManager.Instance.Money;
        Anxiety = GameManager.Instance.Anxiety;
        Hunger = GameManager.Instance.Hunger;

        if(GameManager.Instance.SelectedDistrict != null)
        {
            SelectedDistrictName = GameManager.Instance.SelectedDistrict.Name;
        }
        foreach (var district in GameManager.Instance.Districts)
        {
            Districts.Add(new DistrictData(district));
        }
        foreach (var person in GameManager.Instance.Reserve)
        {
            Reserve.Add(new PersonData(person));
        }
        foreach (var worker in GameManager.Instance.ActiveWorkers)
        {
            ActiveWorkers.Add(new PersonData(worker));
        }
        foreach(var room in GameManager.Instance.Rooms)
        {
            RoomLevels.Add(room.Level);
        }
    }
}