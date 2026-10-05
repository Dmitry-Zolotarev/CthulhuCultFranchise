using UnityEngine;

[System.Serializable]
public class District : MonoBehaviour
{
    [HideInInspector] public int Influence = 1;
    [SerializeField] private GameObject startShiftButton; 
    public PersonType ResidentType = PersonType.OfficeWorker;
    public string Description;
    public string Auditory;
    public string Name;

    public void SelectDistrict()
    {
        GameManager.Instance.SelectedDistrict = this;
        GameManager.Instance.StartWorkPanel?.SetActive(true);        
        GameManager.Instance.UpdateDistrictLabels();
        startShiftButton?.SetActive(Influence < 5);
    }
}
