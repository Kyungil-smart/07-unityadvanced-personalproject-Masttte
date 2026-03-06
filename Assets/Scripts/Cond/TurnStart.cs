public class TurnStart : CondBase
{
    public override bool Check()
    {
        if (Commander.Instance.curTurn is not tStart) return false;

        if (owner._lucidity < behavior.LucidityCost) return false;

        return true;
    }
}
