using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Person : MonoBehaviour
{
    public PersonType Type;
    
    
    public float loyalty;
    [HideInInspector] public Room Room;
    [HideInInspector] public float maxLoyalty = 500f;
    [SerializeField] private GameObject loyaltyPanel;
    [SerializeField] private TextMeshProUGUI loyaltyLabel;
    [SerializeField] private Slider loyaltyBar;

    private void Awake()
    {
        loyalty = maxLoyalty;
    }
    private void Update()
    {
        if(OfficeManager.Instance.ShiftRunning)
        {
            UpdateUI();
            if(!(Room is Laundry)) loyalty -= Time.deltaTime * OfficeManager.Instance.GetTimeSpeed();
        }
        if (loyalty <= 0) Escape();
    }
    private void UpdateUI()
    {
        loyaltyLabel?.SetText($"ћракобесие: {(int)(loyalty / 5f)}");
        loyaltyBar.value = loyalty / maxLoyalty;
    }
    public void Escape()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.reserve.Remove(this);
            GameManager.Instance.activeWorkers.Remove(this);
            GameManager.Instance.AddAnxiety(1);
        }     
        Destroy(gameObject);
    }

    public void Eat(float hungerReduction)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddAnxiety(1);
            GameManager.Instance.ReduceHunger(hungerReduction);
            GameManager.Instance.reserve.Remove(this);
            GameManager.Instance.activeWorkers.Remove(this);
        }
        Destroy(gameObject);
    }
}