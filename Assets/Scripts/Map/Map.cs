using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 수직선 그리드 맵에 맞춰 유닛을 관리
/// </summary>
public class Map : MonoBehaviour
{
    public static Map Instance;

    public Dictionary<int, UnitBase> unitD = new(); //위치와 유닛 Dic (큰 유닛이 여러 키에 매핑될 수 있음)
    public HashSet<UnitBase> unitSet = new();       //단순 탐색을 위한 유닛 Set (실제 유닛 수와 일치)

    public UnitBase player;

    // 유닛 세력 별로 저장
    public Dictionary<Nexus, List<UnitBase>> NexusUnitD = new()
    {
        { Nexus.Lucid, new List<UnitBase>() },
        { Nexus.Nightmare, new List<UnitBase>() },
        { Nexus.Nature, new List<UnitBase>() }
    };
    public void AddNexus(UnitBase unit)
    {
        NexusUnitD[unit.nexus].Add(unit);
    }
    public void RemoveNexus(UnitBase unit)
    {
        NexusUnitD[unit.nexus].Remove(unit);
    }

    public void Put(UnitBase unit)
    {
        foreach (int index in unit.GetOccupied())
        {
            unitD[index] = unit;
        }
        unitSet.Add(unit);
    }
    public void Remove(UnitBase unit, bool isRoot = true)
    {
        if (isRoot)
        {
            foreach (int index in unit.GetOccupied())
            {
                unitD.Remove(index);
            }
        }
        else
        {
            foreach (int index in unit.GetReverseOccupied())
            {
                unitD.Remove(index);
            }
        }
        unitSet.Remove(unit);
    }

    public UnitBase GetUnitAt(int index)
    {
        unitD.TryGetValue(index, out UnitBase unit);
        return unit;
    }
    public UnitBase[] GetAllUnits()
    {
        return unitSet.ToArray();
    }

    // 가장 가까운 세력 유닛 탐색
    public UnitBase GetClosestUnit(UnitBase unit, Nexus targetNexus, out int closestDist)
    {
        UnitBase closest = null;
        closestDist = int.MaxValue;
        foreach (var target in NexusUnitD[targetNexus])
        {
            if (target == unit) continue; //자기 자신은 제외
            int dist = Mathf.Abs(unit.root - target.root);
            if (dist == closestDist)
            {   //만약 같은 거리의 유닛이 있다면, 오른쪽 유닛을 타겟.
                if (target.root > closest.root) closest = target;
            }
            else if (dist < closestDist)
            {
                closestDist = dist;
                closest = target;
            }
        }
        closestDist = Mathf.Max(0, Mathf.Max(unit.root, closest.root) - Mathf.Min(unit.root + unit.size, closest.root + closest.size));
        return closest;
    }
    // 가장 가까운 아무 유닛 탐색
    public UnitBase GetClosestUnit(UnitBase unit, out int closestDist)
    {
        UnitBase closest = null;
        closestDist = int.MaxValue;
        foreach (var target in unitSet)
        {
            if (target == unit) continue;
            int dist = Mathf.Abs(unit.root - target.root);
            if (dist == closestDist)
            {
                if (target.root > closest.root) closest = target;
            }
            else if (dist < closestDist)
            {
                closestDist = dist;
                closest = target;
            }
        }
        closestDist = Mathf.Max(0, Mathf.Max(unit.root, closest.root) - Mathf.Min(unit.root + unit.size, closest.root + closest.size));
        return closest;
    }

#if UNITY_EDITOR
    private void Reset()
    {
        player = FindFirstObjectByType<Player>();
    }
#endif
    private void Awake()
    {
        Instance = this;
    }

    //void LogEveryUnitInConsole()
    //{
    //    foreach (var unit in unitSet)
    //    {
    //        Debug.Log($"Unit: {unit.name}, Root: {unit.root}, Occupied: {string.Join(", ", unit.GetOccupied())}");
    //    }
    //}
}
