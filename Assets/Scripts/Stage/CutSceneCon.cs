using UnityEngine;
using UnityEngine.InputSystem;

public class CutSceneCon : MonoBehaviour
{
    public CutSceneState cs;

    public PlayerInput input;
    
    InputAction NextAction;

#if UNITY_EDITOR
    private void Reset()
    {
        cs = Resources.LoadAll<CutSceneState>("State")[0];
        input = FindAnyObjectByType<PlayerInput>();
    }
#endif
    private void Awake()
    {
        NextAction = input.actions["Next"];
    }

    private void OnEnable()
    {
        NextAction.started += OnNext;
    }
    private void OnDisable()
    {
        NextAction.started -= OnNext;
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        if (StateManager.Instance.curState != eState.CutScene) return;
        cs.Next();
    }
}