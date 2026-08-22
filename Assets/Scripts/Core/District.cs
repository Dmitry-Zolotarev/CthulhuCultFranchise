using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class District : MonoBehaviour
{
    public int Influence = 1;
    public int PeopleFlow = 1;
    public string Name;
    public string Auditory;
    public string Description;
    public PersonType ResidentType = PersonType.OfficeWorker;
    public void SetectDistrict()
    {
        GameManager.Instance.District = this;
        GameManager.Instance.StartWorkPanel?.SetActive(true);
        GameManager.Instance.UpdateDistrictLabels();
    }
}
