using UnityEngine;

/// <summary>
/// 한 방향으로 세력 무관하게 공격합니다
/// </summary>
[CreateAssetMenu(fileName = "Atk Any On Dir", menuName = "Action/Atk Any On Dir")]
public class AtkAnyOnDir : ActBase
{
    public float atkX;  // 공격 배율
    public int range;    // 탐색 거리
    public int size;     // 공격 크기 (2이상은 첫 타겟 뒤 유닛 관통 공격)
    public int atkCount; // 공격 횟수
    public bool isBack;      // false=바라보는 방향, true=반대 방향

    public async override Awaitable Act()
    {
        int dir = owner.isLookLeft ? -1 : 1;
        if (isBack) dir = -dir;

        // 탐색: 가까운 칸부터
        UnitBase target = null;
        for (int i = 1; i <= range; i++)
        {
            if (Map.Instance.unitD.TryGetValue(owner.root + dir * i, out UnitBase unit))
            {
                target = unit;
                break;
            }
        }
        if (target == null) return;

        for (int k = 0; k < atkCount; k++)
        {
            if (target.isDead) break;

            owner.ani.SetTrigger("atk");
            await Awaitable.EndOfFrameAsync();
            float aniLength = owner.ani.GetNextAnimatorStateInfo(0).length;

            await Awaitable.WaitForSecondsAsync(aniLength * 0.5f);

            // 첫 타겟 공격
            if (!target.isDead)
                target.TakeDamage((int)(owner.atk * atkX));

            // 관통 (size 2 이상)
            int count = size;
            for (int i = 1; i < range + 1; i++)
            {
                if (Map.Instance.unitD.TryGetValue(target.root + dir * i, out UnitBase splash))
                {
                    if (!splash.isDead)
                    {
                        count--;
                        if (count <= 0) break;
                        splash.TakeDamage((int)(owner.atk * atkX));
                    }
                }
            }

            await Awaitable.WaitForSecondsAsync(aniLength);
        }
    }
}
