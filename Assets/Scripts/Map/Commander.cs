using System.Collections.Generic;
using UnityEngine;

public enum Nexus : byte
{
    Lucid,
    Nightmare,
    Nature,
}

/// <summary>
/// 유닛으로부터 명령을 받아 처리
/// </summary>
public class Commander : MonoBehaviour
{
    public static Commander Instance;
    public static int sessionId; // 씬 리로드마다 증가 → 구 세션 async 탈출용

    public Queue<Behavior> ActQ = new();

    public int turnCount; // 턴이 Idle일 때마다 증가
    public TurnBase curTurn;
    public Dictionary<eTurn, TurnBase> turnD;
    public TurnBase[] turns;

#if UNITY_EDITOR
    private void Reset()
    {
        turns = Resources.LoadAll<TurnBase>("State/Turn");
    }
#endif
    private void Awake()
    {
        sessionId++; // 새 세션 시작
        Instance = this;
        turnCount = 0;
        turnD = new();
        foreach (TurnBase turn in turns)
        {
            turnD.Add(turn.eTurn, turn);
        }
        ChangeTurn(eTurn.None).Cancel();
    }

    public async Awaitable ChangeTurn(eTurn next)
    {
        if (curTurn?.eTurn == next) return;

        curTurn?.Exit();
        curTurn = turnD[next];
        await curTurn.Enter();
    }
}
