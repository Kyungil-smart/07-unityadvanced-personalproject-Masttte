using UnityEngine;
using System.Linq;

public class tEnd : TurnBase
{
    UnitBase[] units;
    public override async Awaitable Enter()
    {
        int mySession = Commander.sessionId;
        await Awaitable.WaitForSecondsAsync(0.23f); // 턴이 바뀌는 느낌을 주기 위해 잠시 대기
        if (Commander.sessionId != mySession) return;

        // 0. 모든 유닛의 정보를 가져옴
        units = Map.Instance.GetAllUnits();
        // 1. Player와 가까운 유닛 순으로 정렬 (오른쪽 우선)
        units = units.OrderBy(u => Mathf.Abs(u.root - Map.Instance.player.root))
                     .ThenByDescending(u => u.root > Map.Instance.player.root)
                     .ToArray();

        // 2. 유닛들의 행동들을 실행
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i].isDead) continue;
            bool acted = false;

            int count = maxCheckCount; // 무한루프 방지용 카운트
            while (units[i]._lucidity > 0 && count > 0)
            {
                bool executed = false;
                foreach (var behavior in units[i].behaviors)
                {
                    if (behavior.CheckAll(units[i]))
                    {
                        if (!acted)
                        {
                            await Spotlight.Instance.FocusOn(units[i].transform, units[i].isRange);
                            if (Commander.sessionId != mySession) return;
                            acted = true;
                        }
                        await behavior.Execute(units[i]);
                        if (Commander.sessionId != mySession) return;
                        executed = true;
                        break;
                    }
                }
                if (!executed) break;
                count--;
            }
        }

        // 3. 턴 시작으로 전환
        await Commander.Instance.ChangeTurn(eTurn.Start);
    }
}
