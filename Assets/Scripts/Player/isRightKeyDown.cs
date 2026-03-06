public class isRightKeyDown : CondBase
{
    public override bool Check()
    {
        if (Commander.Instance.curTurn is not tIdle) return false;

        if (owner._lucidity < behavior.LucidityCost) return false;

        if (!Player.isRightKeyDown) return false;

        Player.isRightKeyDown = false;
        return true;
    }
}
