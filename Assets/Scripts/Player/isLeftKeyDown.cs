public class isLeftKeyDown : CondBase
{
    public override bool Check()
    {
        if (Commander.Instance.curTurn is not tIdle) return false;

        if (owner._lucidity < behavior.LucidityCost) return false;

        if (!Player.isLeftKeyDown) return false;
        Player.isLeftKeyDown = false;
        return true;
    }
}
