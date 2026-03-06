using UnityEngine;

public class tNone : TurnBase
{
    public override async Awaitable Enter()
    {
        await base.Enter();
    }
     public override void Exit()
    {
        base.Exit();
    }
}
