using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Action/Move")]
public class Move : ActBase
{
    public int moveAmount = 1;
    public float time = 0.15f;

    public override async Awaitable Act()
    {
        if (moveAmount > 0)
        {
            if (owner.isLookLeft) owner.Flip();
        }
        else
        {
            if (!owner.isLookLeft) owner.Flip();
        }

        owner.Move(moveAmount, time);
        await Awaitable.WaitForSecondsAsync(time);
    }
}
