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

    public void SetectDistrict()
    {
        GameManager.Instance.SelectedDistrict = this;
        GameManager.Instance.StartWorkPanel?.SetActive(true);
        GameManager.Instance.UpdateDistrictLabels();
        
    }
}
