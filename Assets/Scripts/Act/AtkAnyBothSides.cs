using UnityEngine;

/// <summary>
/// 양옆의 유닛을 공격합니다 (세력 무관, 좌우 각각 탐색)
/// </summary>
[CreateAssetMenu(fileName = "Atk Any Both Sides", menuName = "Action/Atk Any Both Sides")]
public class AtkAnyBothSides : ActBase
{
    public float atkX;    // 공격 배율
    public int range;      // 좌우 각각의 탐색 거리
    public int size;       // 공격 크기 (2이상은 첫 타겟 뒤 유닛 관통 공격)
    public bool isOneSide;     // true면 가장 가까운 한 방향 타겟만 공격

    public async override Awaitable Act()
    {
        // 양방향 탐색: 가까운 칸부터 순서대로
        UnitBase leftTarget = FindTarget(-1, out int leftDist);
        UnitBase rightTarget = FindTarget(1, out int rightDist);

        // isOneSide: 더 가까운 방향 하나만 남김 (거리 같으면 오른쪽 우선)
        if (isOneSide)
        {
            if (leftTarget != null && rightTarget != null)
            {
                if (leftDist < rightDist) rightTarget = null;
                else leftTarget = null;
            }
        }

        if (leftTarget == null && rightTarget == null) return;

        // 한쪽만 있으면 해당 방향으로 flip
        if (leftTarget == null) { if (owner.isLookLeft) owner.Flip(); }
        else if (rightTarget == null) { if (!owner.isLookLeft) owner.Flip(); }

        // 공격 애니메이션
        owner.ani.SetTrigger("atk");
        await Awaitable.EndOfFrameAsync();
        float aniLength = owner.ani.GetNextAnimatorStateInfo(0).length;
        float hitTime = aniLength * 0.5f;

        await Awaitable.WaitForSecondsAsync(hitTime);

        if (leftTarget != null) DealDamage(leftTarget, -1);
        if (rightTarget != null) DealDamage(rightTarget, 1);

        await Awaitable.WaitForSecondsAsync(aniLength);
    }

    UnitBase FindTarget(int dir, out int dist)
    {
        for (int i = 1; i <= range; i++)
        {
            if (Map.Instance.unitD.TryGetValue(owner.root + dir * i, out UnitBase unit))
            {
                dist = i;
                return unit;
            }
        }
        dist = int.MaxValue;
        return null;
    }

    void DealDamage(UnitBase firstTarget, int dir)
    {
        firstTarget.TakeDamage((int)(owner.atk * atkX));

        // 관통 (size 2 이상)
        int count = size;
        for (int i = 1; i < range + 1; i++)
        {
            if (Map.Instance.unitD.TryGetValue(firstTarget.root + dir * i, out UnitBase splash))
            {
                count--;
                if (count <= 0) break;
                splash.TakeDamage((int)(owner.atk * atkX));
            }
        }
    }
}
