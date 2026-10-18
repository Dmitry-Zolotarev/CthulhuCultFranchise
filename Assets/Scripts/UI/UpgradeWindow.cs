using UnityEngine;

public class UpgradeWindow : MonoBehaviour
{
    public Room Room;
    public void UpgradeRoom()
    {
        Room?.LevelUP();
    }
}
