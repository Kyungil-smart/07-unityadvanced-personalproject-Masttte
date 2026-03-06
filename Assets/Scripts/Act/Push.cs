using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 바라보는 방향의 유닛을 밀치는 행동
/// </summary>
[CreateAssetMenu(fileName = "Push", menuName = "Action/Push")]
public class Push : ActBase
{
    public int range = 1;        // 타겟 탐색 거리
    public int pushDistance = 2; // 밀치는 칸 수
    public float pushTime = 0.3f; // 밀치는 이동 시간

    int _dir;
    UnitBase _target;

    public override async Awaitable Act()
    {
        _dir = owner.isLookLeft ? -1 : 1;

        _target = null;
        for (int i = 1; i <= range; i++)
        {
            if (Map.Instance.unitD.TryGetValue(owner.root + _dir * i, out UnitBase unit))
            {
                _target = unit;
                break;
            }
        }
        if (_target == null) return;

        // 타겟부터 밀치는 방향으로 붙어있는 유닛들을 순서대로 수집
        var chain = new List<UnitBase>();
        UnitBase cur = _target;
        while (cur != null && !cur.isDead)
        {
            chain.Add(cur);
            int frontPos = _dir > 0 ? cur.root + cur.size + 1 : cur.root - 1;
            UnitBase next = Map.Instance.GetUnitAt(frontPos);
            cur = (next != null && next != cur) ? next : null;
        }

        owner.ani.SetTrigger("atk2");
        await Awaitable.EndOfFrameAsync();
        float aniLength = owner.ani.GetNextAnimatorStateInfo(0).length;

        await Awaitable.WaitForSecondsAsync(aniLength * 0.5f);

        // 밀치는 방향으로 가장 멀리 있는 유닛부터 처리 (맵 등록 충돌 방지)
        // _dir=1(오른쪽): root 내림차순(오른쪽 먼저) / _dir=-1(왼쪽): root 오름차순(왼쪽 먼저)
        chain.Sort((a, b) => _dir * (b.root - a.root));
        for (int i = 0; i < chain.Count; i++)
        {
            var unit = chain[i];
            if (unit.isDead) continue;
            Map.Instance.Remove(unit);
            unit.root += _dir * pushDistance;
            unit.transform.DOLocalMoveX(unit.transform.localPosition.x + _dir * pushDistance, pushTime).SetEase(Ease.OutCubic);
            Map.Instance.Put(unit);
        }

        await Awaitable.WaitForSecondsAsync(pushTime);
    }
}
