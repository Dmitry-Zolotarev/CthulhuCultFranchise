using UnityEngine;

public class CultistThrowAnimationEvents : MonoBehaviour
{
    [SerializeField] private CultistFlyerThrower thrower;

    public void Anim_SpawnFlyerInHand()
    {
        if (thrower != null)
            thrower.Anim_SpawnFlyerInHand();
    }

    public void Anim_ReleaseFlyer()
    {
        if (thrower != null)
            thrower.Anim_ReleaseFlyer();
    }

    public void Anim_FinishThrow()
    {
        if (thrower != null)
            thrower.Anim_FinishThrow();
    }
}
