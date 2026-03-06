using UnityEngine;

public abstract class ActBase : ScriptableObject
{
    [HideInInspector] public UnitBase owner;
    public Icon type;
    [TextArea] public string desc;

    public virtual async Awaitable Act()
    {
        await Awaitable.WaitForSecondsAsync(1f);
    }
}