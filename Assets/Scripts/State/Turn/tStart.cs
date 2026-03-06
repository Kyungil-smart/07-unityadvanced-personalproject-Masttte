using UnityEngine;
using System.Linq;

public class tStart : TurnBase
{
    UnitBase[] units;

    public override async Awaitable Enter()
    {
        int mySession = Commander.sessionId;

        // 1. 카메라를 플레이어에게 이동
        await Spotlight.Instance.Release();
        if (Commander.sessionId != mySession) return;
        
        // 2. 모든 유닛의 Lucidity 회복
        units = Map.Instance.GetAllUnits();
        for (int i = 0; i < units.Length; i++)
        {
            units[i].Lucidity = units[i].maxLucidity;
        }

        // 3. 유닛 정렬
        units = units.OrderBy(u => Mathf.Abs(u.root - Map.Instance.player.root))
                     .ThenByDescending(u => u.root > Map.Instance.player.root)
                     .ToArray();

        // 4. 유닛 행동 (유닛당 조건을 만족하는 첫 번째 행동 하나만 실행)
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i].isDead) continue;

            foreach (var behavior in units[i].behaviors)
            {
                if (behavior.CheckAll(units[i]))
                {
                    await Spotlight.Instance.FocusOn(units[i].transform, units[i].isRange);
                    if (Commander.sessionId != mySession) return;
                    await behavior.Execute(units[i]);
                    if (Commander.sessionId != mySession) return;
                    break;
                }
            }
        }

        // 5. tIdle 상태로 전환
        await Commander.Instance.ChangeTurn(eTurn.Idle);
    }
}
