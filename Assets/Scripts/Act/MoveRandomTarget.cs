using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무작위 적의 앞 또는 뒤 빈칸으로 이동합니다
/// </summary>
[CreateAssetMenu(fileName = "MoveRandomTarget", menuName = "Action/MoveRandomTarget")]
public class MoveRandomTarget : ActBase
{
    public bool isAlly;

    public override async Awaitable Act()
    {
        Nexus targetNexus = isAlly ? owner.nexus : owner.nexus.GetEnemyNexus();
        var target = new List<UnitBase>(Map.Instance.NexusUnitD[targetNexus]);

        // 순서 셔플
        for (int i = target.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (target[i], target[j]) = (target[j], target[i]);
        }

        // 비어있는 자리 탐색
        bool found = false;
        int targetRoot = 0;

        foreach (var enemy in target)
        {
            // 앞: 적이 바라보는 방향 / 뒤: 반대 방향
            int frontRoot = enemy.isLookLeft
                ? enemy.root - owner.size - 1
                : enemy.root + enemy.size + 1;
            int backRoot = enemy.isLookLeft
                ? enemy.root + enemy.size + 1
                : enemy.root - owner.size - 1;

            bool frontEmpty = CanFit(frontRoot);
            bool backEmpty  = CanFit(backRoot);

            if (!frontEmpty && !backEmpty) continue;

            // 둘 다 비어있으면 랜덤 선택
            if (frontEmpty && backEmpty)
                targetRoot = Random.value > 0.5f ? frontRoot : backRoot;
            else
                targetRoot = frontEmpty ? frontRoot : backRoot;

            found = true;
            break;
        }

        if (!found) return;

        // 이동
        Map.Instance.Remove(owner);
        float deltaX = targetRoot - owner.root;
        owner.root = targetRoot;

        // 도착 방향에 따라 flip
        owner.sr.flipX = deltaX < 0;

        //owner.transform.DOLocalMoveX(owner.transform.localPosition.x + deltaX, moveTime)
        owner.transform.localPosition += new Vector3(deltaX, 0, 0);

        Map.Instance.Put(owner);

        // Idle 강제 재생
        owner.ani.Play("Idle", 0, 0f);
        await Awaitable.EndOfFrameAsync();

        float idleLength = owner.ani.GetCurrentAnimatorStateInfo(0).length;
        await Awaitable.WaitForSecondsAsync(idleLength);
    }

    /// <summary>targetRoot에 owner가 들어갈 수 있는지 확인</summary>
    bool CanFit(int targetRoot)
    {
        foreach (int offset in owner.offsets)
        {
            UnitBase unit = Map.Instance.GetUnitAt(targetRoot + offset);
            if (unit != null && unit != owner) return false;
        }
        return true;
    }
}
