using UnityEngine;

/// <summary>
/// 어떤 유닛이 거리 안에 있는지 확인합니다
/// </summary>
[CreateAssetMenu(fileName = "SomeInRange", menuName = "Condition/SomeInRange")]
public class SomeInRange : CondBase
{
    public int distance = 1;
    public bool isAlly;
    public bool exactDistance; // 체크: 정확한 거리일 때만 / 해제: 거리 이내이면

    public override bool Check()
    {
        if (owner._lucidity < behavior.LucidityCost) return false;

        Nexus targetNexus = isAlly ? owner.nexus : owner.nexus.GetEnemyNexus();
        foreach (var target in Map.Instance.NexusUnitD[targetNexus])
        {
            if (target == owner) continue;
            int dist = Mathf.Max(0, Mathf.Max(owner.root, target.root) - Mathf.Min(owner.root + owner.size, target.root + target.size));
            bool inRange = exactDistance ? (dist == distance) : (dist <= distance);
            if (inRange) return true;
        }
        return false;
    }
}
