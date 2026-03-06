using UnityEngine;

public enum Icon
{
    Turn,
    Atk,
    Move,
    Distance,
    Music,
    Random,
    KeyA,
    KeyD,
    KeyW,
    KeyF,
}
public abstract class CondBase : ScriptableObject
{
    public int lucidityCost;
    public Icon type;
    [TextArea] public string desc;

    [HideInInspector] public UnitBase owner;
    [HideInInspector] public Behavior behavior;

    bool _isCheck; // todo: UI 바인딩 하기
    public bool IsCheck // Check()를 실행하기 때문에 참조연결이 된 후에만 사용해야 함
    {
        get => _isCheck = Check();
        set
        {
            _isCheck = value;
        }
    }

    public abstract bool Check();
}