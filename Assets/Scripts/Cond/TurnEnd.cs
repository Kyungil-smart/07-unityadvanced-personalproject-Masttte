public class TurnEnd : CondBase
{
    public override bool Check()
    {
        if (Commander.Instance.curTurn is not tEnd) return false;

        if (owner._lucidity < behavior.LucidityCost) return false;
        
        return true;
    }
}
