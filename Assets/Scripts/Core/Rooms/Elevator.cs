using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Elevator : MonoBehaviour
{
    [SerializeField] private RectTransform ExitPoint;
    [SerializeField] private float ElevateTime = 10f;
    
    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Update()
    {      
        foreach(var person in GameManager.Instance.Escaping)
        {
            if (person != null && Overlaps(person.GetComponent<RectTransform>()) && person.IsEscaping) 
            {
                StartCoroutine(ElevatorCoroutine(person));    
            }        
        }
    }
    bool Overlaps(RectTransform b)
    {
        if (b == null) return false;
        Rect rectA = new Rect(rectTransform.position, rectTransform.rect.size);
        Rect rectB = new Rect(b.position, b.rect.size);
        return rectA.Overlaps(rectB);
    }
    private IEnumerator ElevatorCoroutine(Person person)
    {
        person.gameObject.SetActive(false);
        yield return new WaitForSeconds(ElevateTime / GameManager.Instance.GetTimeSpeed());
        person.gameObject.SetActive(true);
       
        if(!person.HasElevated)
        {
            person.transform.position = ExitPoint.position;
            person.transform.localScale = new Vector2(-1, 1);
            person.HasElevated = true;
        }      
    }
}
