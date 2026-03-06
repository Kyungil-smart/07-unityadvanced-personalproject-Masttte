public class PlayState : StateBase
{
    public override void Enter()
    {
        if (Commander.Instance.curTurn is tNone)
        Commander.Instance.ChangeTurn(eTurn.Start).Cancel();
    }
}
