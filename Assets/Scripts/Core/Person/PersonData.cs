[System.Serializable]
public class PersonData
{
    public PersonType Type;
    public RoomType RoomType;
    public int Loyalty;
    
    public float MaxLoyalty;
    public bool IsCultist;
    public PersonData(Person person)
    {
        Type = person.Type;
        Loyalty = (int)person.Loyalty;
        RoomType = person.RoomType;
        MaxLoyalty = person.MaxLoyalty;
        IsCultist = person.IsCultist && RoomType != RoomType.Reception;
        
    }
    public void Load(Person person)
    {
        person.Type = Type;
        person.Loyalty = Loyalty;
        person.RoomType = RoomType;
        person.MaxLoyalty = MaxLoyalty;

        person.IsCultist = IsCultist;
        if (person.IsCultist) 
        {
            GameManager.Instance.ActiveWorkers.Add(person);
            person.Image.sprite = GameManager.Instance.CultistSprite;
        }
        person.FindRoom();
    }
}