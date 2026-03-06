using UnityEngine;

[CreateAssetMenu(fileName = "MoveToEnemy", menuName = "Action/MoveToEnemy")]
public class MoveToEnemy : ActBase
{
    public int moveAmount = 1;
    Nexus _targetNexus;
    UnitBase _targetUnit;

    public override async Awaitable Act()
    {
        _targetNexus = owner.nexus.GetEnemyNexus();
        _targetUnit = Map.Instance.GetClosestUnit(owner, _targetNexus, out int closestDist);

        if (_targetUnit != null)
        {
            if (owner.root < _targetUnit.root)
            {
                owner.Move(moveAmount);
                owner.sr.flipX = false;
            }
            else
            {
                owner.Move(-moveAmount);
                owner.sr.flipX = true;
            }
        }

        await Awaitable.WaitForSecondsAsync(0.15f);
    }
}
