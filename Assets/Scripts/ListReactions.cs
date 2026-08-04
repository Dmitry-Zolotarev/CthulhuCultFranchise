using UnityEngine;

[CreateAssetMenu(fileName = "HumanReact", menuName = "human")]
public class ListReactions : ScriptableObject
{
    [SerializeField] private string[] needWorker;
    [SerializeField] private string[] needStudent;
    [SerializeField] private string[] needRetiree;
    [SerializeField] private string[] needBlogger;
    [SerializeField] private string[] needEsoteric;

    [SerializeField] private int[] lenDaySec;
    [SerializeField] private int[] countHuman;
    [SerializeField] private float[] suspicion;

    public string[] NeedWorker => needWorker;
    public string[] NeedStudent => needStudent;
    public string[] NeedRetiree => needRetiree;
    public string[] NeedBlogger => needBlogger;
    public string[] NeedEsoteric => needEsoteric;

    public int[] LenDaySec => lenDaySec;
    public int[] CountHuman => countHuman;
    public float[] Suspicion => suspicion;
}
