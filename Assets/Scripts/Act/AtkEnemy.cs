using UnityEngine;
/// <summary>
/// 가장 가까운 적을 공격하는 행동 (좌우 탐색)
/// </summary>
[CreateAssetMenu(fileName = "AtkEnemy", menuName = "Action/Atk Enemy")]
public class AtkEnemy : ActBase
{
    public float atkX = 1;  // 공격 배율
    public int atkSize = 1; // 공격 크기 (2이상은 해당 방향으로 관통 공격)
    public int atkCount = 1;// 공격 횟수
    public int range = 1;   // 탐색 가능 거리

    public async override Awaitable Act()
    {
        UnitBase target = Map.Instance.GetClosestUnit(owner, owner.nexus.GetEnemyNexus(), out int dist);

        if (target == null || dist > range) return;

        int dir = target.root > owner.root ? 1 : -1;

        // 방향 전환
        if (dir == 1) { if (owner.isLookLeft)  owner.Flip(); }
        else          { if (!owner.isLookLeft)  owner.Flip(); }

        // 관통 타겟 수집
        UnitBase[] targets = new UnitBase[atkSize];
        targets[0] = target;
        int targetCount = 1;
        for (int i = 1; i < atkSize; i++)
        {
            if (Map.Instance.unitD.TryGetValue(target.root + dir * i, out UnitBase next))
                targets[targetCount++] = next;
        }

        for (int i = 0; i < atkCount; i++)
        {
            if (target.isDead) break;

            owner.ani.SetTrigger("atk");
            await Awaitable.EndOfFrameAsync();
            float aniLength = owner.ani.GetNextAnimatorStateInfo(0).length;

            await Awaitable.WaitForSecondsAsync(aniLength * 0.5f);
            for (int j = 0; j < targetCount; j++)
            {
                if (!targets[j].isDead)
                    targets[j].TakeDamage((int)(owner.atk * atkX));
            }
            await Awaitable.WaitForSecondsAsync(aniLength * 0.9f);
        }
    }
}
