using UnityEngine;

/// <summary>
/// 바라보는 방향 기준으로 거리 안에 유닛이 있는지 확인합니다
/// </summary>
[CreateAssetMenu(fileName = "SomeInRangeDir", menuName = "Condition/SomeInRangeDir")]
public class SomeInRangeDir : CondBase
{
    public int distance = 1;
    public bool exactDistance; // 체크: 정확한 거리일 때만 / 해제: 거리 이내이면
    public bool isAlly;
    public bool isForward;    // 체크: 바라보는 방향 / 해제: 등 뒤 방향

    public override bool Check()
    {
        if (owner._lucidity < behavior.LucidityCost) return false;

        Nexus targetNexus = isAlly ? owner.nexus : owner.nexus.GetEnemyNexus();
        foreach (var target in Map.Instance.NexusUnitD[targetNexus])
        {
            if (target == owner) continue;

            // 타겟이 앞에 있는가?
            bool isTargetInFront = owner.isLookLeft ? target.root < owner.root : target.root > owner.root; // 왼쪽을 바라보면 왼쪽이 앞 : 오른쪽을 바라보면 오른쪽이 앞
            if (isForward != isTargetInFront) continue;

            // 거리 체크
            int dist = Mathf.Max(0, Mathf.Max(owner.root, target.root) - Mathf.Min(owner.root + owner.size, target.root + target.size));
            bool inRange = exactDistance ? (dist == distance) : (dist <= distance);
            if (inRange) return true;
        }
        return false;
    }
}
