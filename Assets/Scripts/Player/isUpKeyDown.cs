public class isUpKeyDown : CondBase
{
    public override bool Check()
    {
        if (Commander.Instance.curTurn is not tIdle) return false;

        if (owner._lucidity < behavior.LucidityCost) return false;

        if (!Player.isUpKeyDown) return false;
        Player.isUpKeyDown = false;

        return true;
    }
}
