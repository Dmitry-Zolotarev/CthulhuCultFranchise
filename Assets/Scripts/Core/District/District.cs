using UnityEngine;

[System.Serializable]
public class District : MonoBehaviour
{
    public string Name;
    public int Influence = 1;
    
    public string Auditory;
    public string Description;
    public PersonType ResidentType = PersonType.OfficeWorker;

    public void SetectDistrict()
    {
        GameManager.Instance.SelectedDistrict = this;
        GameManager.Instance.StartWorkPanel?.SetActive(true);
        GameManager.Instance.UpdateDistrictLabels();
    }
}
