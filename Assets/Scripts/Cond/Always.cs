public class Always : CondBase
{
    public override bool Check()
    {
        if (owner._lucidity < behavior.LucidityCost) return false;

        return true;
    }
}
