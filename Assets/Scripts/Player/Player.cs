using UnityEngine.InputSystem;

public class Player : UnitBase
{
    public override int Hp // override «¡∑Œ∆€∆º
    {
        get => _hp;
        set
        {
            _hp = value;
            if (_hp <= 0)
            {
                _hp = 0;
                Die().Cancel();
                GameManager.Instance.GameOver().Cancel();
            }
            ui?.UpdateHp(_hp);
        }
    }
    public override int Lucidity
    {
        get => _lucidity;
        set
        {
            _lucidity = value;
            if (_lucidity <= 0)
            {
                _lucidity = 0;
            }
            ui?.UpdateLucidity(_lucidity);
        }
    }

    // ¿Œ«≤ π∂≈ ¿ÃµÈ
    PlayerInput input; InputAction LeftAction; InputAction RightAction; InputAction UpAction;
    InputAction XAction; InputAction PushAction; InputAction LucidAction;
    public static bool isLeftKeyDown; public static bool isRightKeyDown; public static bool isUpKeyDown; public static bool isFKeyDown;
    private void Awake()
    {
        Init();

        input = GetComponent<PlayerInput>();
        LeftAction = input.actions["Left"]; RightAction = input.actions["Right"]; UpAction = input.actions["Up"];
        XAction = input.actions["Close"]; PushAction = input.actions["Push"]; LucidAction = input.actions["Lucid"];
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        LeftAction.started += OnLeft;
        RightAction.started += OnRight;
        UpAction.started += OnUp;
        XAction.started += OnClose;
        PushAction.started += OnPush;
        LucidAction.started += OnLucid;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        LeftAction.started -= OnLeft;
        RightAction.started -= OnRight;
        UpAction.started -= OnUp;
        XAction.started -= OnClose;
        PushAction.started -= OnPush;
        LucidAction.started -= OnLucid;
    }

    #region ¿Œ«≤ √≥∏Æ ∏ﬁº≠µÂ
    public void OnLeft(InputAction.CallbackContext context)
    {
        if (Commander.Instance.curTurn is not tIdle) return;
        isLeftKeyDown = true;
    }
    public void OnRight(InputAction.CallbackContext context)
    {
        if (Commander.Instance.curTurn is not tIdle) return;
        isRightKeyDown = true;
    }
    public void OnUp(InputAction.CallbackContext context)
    {
        if (Commander.Instance.curTurn is not tIdle) return;
        isUpKeyDown = true;
    }

    public void OnClose(InputAction.CallbackContext context)
    {
        GameManager.Instance.HideUnitInfo();
    }

    public void OnPush(InputAction.CallbackContext context)
    {
        if (Commander.Instance.curTurn is not tIdle) return;
        isFKeyDown = true;
    }

    public void OnLucid(InputAction.CallbackContext context)
    {
        if (Commander.Instance.curTurn is not tIdle) return;
        if (!LucidMode.Instance.canActive) return;

        if (LucidMode.Instance.isActive)
            LucidMode.Instance.Exit();
        else
            LucidMode.Instance.Enter();
    }
    #endregion

    private void Update()
    {
        if (Commander.Instance.curTurn is not tIdle) return;
        if (_lucidity <= 0)
        {
            Commander.Instance.ChangeTurn(eTurn.End).Cancel(); // ≈œ ¡æ∑·
        }
    }
}
