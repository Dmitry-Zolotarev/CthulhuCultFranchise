using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public enum PersonType
{
    Student,
    OfficeWorker
}
[RequireComponent(typeof(Image))]
public class Person : MonoBehaviour
{
    [HideInInspector] public PersonType Type;
    [HideInInspector] public float loyalty;
    [HideInInspector] public Room Room;    
    [HideInInspector] public float maxLoyalty;
    [SerializeField] private float becomeCultistTime = 6f;
    [SerializeField] private float baseMaxLoyalty = 500f;
    [SerializeField] private GameObject loyaltyPanel;
    [SerializeField] private TextMeshProUGUI loyaltyLabel;
    [SerializeField] private Slider loyaltyBar;
    [HideInInspector] public Image personImage;

    private void Awake()
    {
        personImage = GetComponent<Image>();
       
        maxLoyalty = baseMaxLoyalty;
        loyalty = baseMaxLoyalty;
    }
    private void Update()
    {
        if(GameManager.Instance.phase == GamePhase.Office)
        {
            UpdateUI();
            if(Room is Laundry) 
            {
                if(loyalty < maxLoyalty) loyalty += Time.deltaTime * GameManager.Instance.GetTimeSpeed();
            }
            else loyalty -= Time.deltaTime * GameManager.Instance.GetTimeSpeed();
        }
        if (loyalty <= 0) Escape();
    }
    private void UpdateUI()
    {
        loyaltyLabel?.SetText($"ћракобесие: {GetLoyaltyPercent()}");
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
    public void BecomeCultist()
    {
        GameManager.Instance.activeWorkers.Add(this);
        GameManager.Instance.reserve.Remove(this);
        StartCoroutine(StartRecruitment());
    }
    private IEnumerator StartRecruitment()
    {
        yield return new WaitForSeconds(becomeCultistTime / GameManager.Instance.GetTimeSpeed());
        personImage.sprite = GameManager.Instance.CultistSprite;
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
    private int GetLoyaltyPercent()
    {
        return Mathf.RoundToInt(loyalty / baseMaxLoyalty * 100f);
    }
}