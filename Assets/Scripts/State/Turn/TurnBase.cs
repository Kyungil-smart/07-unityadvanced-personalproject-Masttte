using UnityEngine;

public enum eTurn: byte
{
    None,
    Start,
    Idle,
    End,
}
public abstract class TurnBase : ScriptableObject
{
    public eTurn eTurn;

    protected int maxCheckCount = 32;

    public virtual async Awaitable Enter() { await Awaitable.WaitForSecondsAsync(0.5f); }
    public virtual void Exit() { }
}
