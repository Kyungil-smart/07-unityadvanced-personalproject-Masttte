using UnityEngine;

[CreateAssetMenu(fileName = "isFKeyDown", menuName = "Condition/isFKeyDown")]
public class isFKeyDown : CondBase
{
    public override bool Check()
    {
        if (Commander.Instance.curTurn is not tIdle) return false;

        if (owner._lucidity < behavior.LucidityCost) return false;

        if (!Player.isFKeyDown) return false;
        Player.isFKeyDown = false;

        return true;
    }
}
