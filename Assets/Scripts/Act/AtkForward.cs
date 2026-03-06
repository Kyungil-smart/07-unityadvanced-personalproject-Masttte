using UnityEngine;
/// <summary>
/// 바라보는 방향을 공격 또는 돌진(이동공격)합니다
/// </summary>
[CreateAssetMenu(fileName = "AtkForward", menuName = "Action/Atk Forward")]
public class AtkForward : ActBase
{
    public float atkX; // 공격 배율
    public int range; // 공격 거리
    public int size; // 공격 크기 (2이상은 뒤의 유닛 스플레시 공격)

    public bool isDash; // 돌진 여부 (공격과 동시에 이동, 이동하지 않으려면 false)
    public bool isAtkAllRange; // Range 범위내 모든 유닛 공격
    public bool canAtkAlly; // 아군 공격 허용
    public bool exactRange; // 체크: 정확한 거리일 때만 / 해제: 거리 이내이면
    Nexus _enemyNexus;
    UnitBase _target;
    int _dir;

    public async override Awaitable Act()
    {
        _enemyNexus = owner.nexus.GetEnemyNexus();
        _dir = owner.isLookLeft ? -1 : 1;

        // 범위내 첫 번째 타겟 탐색
        _target = null;
        if (exactRange) // 정확한 거리 체크
        {
            if (Map.Instance.unitD.TryGetValue(owner.root + _dir * range, out UnitBase unit))
            {
                if (canAtkAlly || unit.nexus == _enemyNexus)
                    _target = unit;
            }
        }
        else // 범위 이내 체크
        {
            for (int i = range; i > 0; i--)
            {
                if (Map.Instance.unitD.TryGetValue(owner.root + _dir * i, out UnitBase unit))
                {
                    _target = unit;
                    break;
                }
            }
        }
        if (_target == null) return;

        // 관통 타겟 및 스플레시 수집 (최대 range + size - 1)
        UnitBase[] targets = new UnitBase[range + size - 1];
        targets[0] = _target;
        int targetCount = 1;
        if (isAtkAllRange) // 거리내 모든 타겟 공격
        {
            for (int i = 1; i < range; i++)
            {
                if (Map.Instance.unitD.TryGetValue(owner.root + _dir * i, out UnitBase frontUnit))
                {
                    if (frontUnit != _target && (canAtkAlly || frontUnit.nexus == _enemyNexus)) //range 미만 에서 _target을 찾았으면 frontUnit이 _target이 될수있어 예외처리
                    {
                        targets[targetCount++] = frontUnit;
                    }
                }
            }
        }
        for (int i = 1; i < size; i++) // 공격후 뒤의 타겟 공격
        {
            if (Map.Instance.unitD.TryGetValue(_target.root + _dir * i, out UnitBase backUnit))
            {
                if (canAtkAlly || backUnit.nexus == _enemyNexus) targets[targetCount++] = backUnit;
            }
        }

        // 공격 애니메이션 시작
        if (isDash)
            owner.ani.SetTrigger("atk2");
        else
            owner.ani.SetTrigger("atk");

        await Awaitable.EndOfFrameAsync();
        float aniLength = owner.ani.GetNextAnimatorStateInfo(0).length;
        float hitTime = aniLength * 0.5f;

        if (isDash)
        {
            // 돌진: 타겟 바로 앞 칸까지 이동
            int dashSteps = Mathf.Abs(_target.root - owner.root) - 1;
            if (dashSteps > 0)
                owner.Move(_dir * dashSteps, hitTime);
        }

        await Awaitable.WaitForSecondsAsync(hitTime); // 공격 타이밍 (돌진 도착 타이밍)
        for (int j = 0; j < targetCount; j++)
        {
            if (!targets[j].isDead)
                targets[j].TakeDamage((int)(owner.atk * atkX));
        }
        await Awaitable.WaitForSecondsAsync(aniLength);
    }
}

