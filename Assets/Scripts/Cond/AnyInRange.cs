using UnityEngine;

/// <summary>
/// 아무 유닛이 거리 안에 있는지 확인합니다
/// </summary>
[CreateAssetMenu(fileName = "AnyInRange", menuName = "Condition/AnyInRange")]
public class AnyInRange : CondBase
{
    public int distance = 1;
    public bool isForward; // 체크: 앞에 있는 유닛만
    public bool exactDistance; // 체크: 정확한 거리일 때만 / 해제: 거리 이내이면

    public override bool Check()
    {
        if (owner._lucidity < behavior.LucidityCost) return false;

        foreach (var target in Map.Instance.GetAllUnits())
        {
            if (target == owner) continue;

            if (isForward)
            {
                bool isTargetInFront = owner.isLookLeft ? target.root < owner.root : target.root > owner.root;
                if (!isTargetInFront) continue;
            }

            int dist = Mathf.Max(0, Mathf.Max(owner.root, target.root) - Mathf.Min(owner.root + owner.size, target.root + target.size));
            bool inRange = exactDistance ? (dist == distance) : (dist <= distance);
            if (inRange) return true;
        }
        return false;
    }
}
