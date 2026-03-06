using UnityEngine;

public enum eState: byte
{
    Title,
    Story,
    Lobby,
    CutScene,
    Play,
    EndGame,
    Result,
}
public abstract class StateBase : ScriptableObject
{
    public eState eState;

    public virtual void Init() { }
    public abstract void Enter();
    public virtual void Exit() { }
    public virtual void Update() { }
}
