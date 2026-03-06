using UnityEngine;

[CreateAssetMenu(fileName = "Behavior", menuName = "Scriptable Objects/Behavior")]
public class Behavior : ScriptableObject
{
    [HideInInspector] public UnitBase owner;

    public CondBase[] conditions;
    public ActBase[] actions;

    bool _somethingFalse;

    // ´©Àû LucidityCost
    int _lucidityCost;
    public int LucidityCost
    {
        get
        {
            _lucidityCost = 0;
            for (int i = 0; i < conditions.Length; i++)
            {
                _lucidityCost += conditions[i].lucidityCost;
            }
            return _lucidityCost;
        }
    }

    public bool CheckAll(UnitBase unit)
    {
        //_somethingFalse = false;
        for (int i = 0; i < conditions.Length; i++)
        {
            conditions[i].owner = unit;
            if (!conditions[i].Check()) return false;
        }
        return true;
    }

    public async Awaitable Execute(UnitBase unit)
    {
        unit.Lucidity -= LucidityCost;
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].owner = unit;
            await actions[i].Act();
        }
    }
}
