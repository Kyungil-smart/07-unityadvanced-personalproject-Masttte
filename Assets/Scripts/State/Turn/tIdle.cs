using UnityEngine;

public class tIdle : TurnBase
{
    public override async Awaitable Enter()
    {
        int mySession = Commander.sessionId;
        await Spotlight.Instance.Release();
        if (Commander.sessionId != mySession) return;
        Commander.Instance.turnCount++;

        while (Commander.Instance.curTurn is tIdle && Commander.sessionId == mySession)
        {
            // 1. 수집 단계: 조건 만족하는 행동 큐잉
            foreach (var unit in Map.Instance.GetAllUnits())
            {
                if (unit.isDead || unit.isActing) continue;

                foreach (var behavior in unit.behaviors)
                {
                    if (behavior.CheckAll(unit))
                    {
                        Commander.Instance.ActQ.Enqueue(behavior);
                        unit.isActing = true;
                        break; // 유닛당 하나의 행동만 큐잉
                    }
                }
            }

            // 2. 실행 단계: 큐에서 하나씩 꺼내서 실행
            while (Commander.Instance.ActQ.Count > 0)
            {
                Behavior behavior = Commander.Instance.ActQ.Dequeue();
                UnitBase unit = behavior.owner;

                // 실행 전 사망 또는 파괴 체크
                if (unit == null || unit.isDead)
                {
                    if (unit != null) unit.isActing = false;
                    continue;
                }

                await behavior.Execute(unit);
                if (Commander.sessionId != mySession) return;
                unit.isActing = false;
            }

            await Awaitable.NextFrameAsync();
        }
    }
}
