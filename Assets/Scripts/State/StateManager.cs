using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager Instance;

    public eState curState;
    [SerializeField] StateBase[] states;

    Dictionary<eState, StateBase> stateD;

#if UNITY_EDITOR
    private void Reset() => states = Resources.LoadAll<StateBase>("State");
#endif
    public void Awake()
    {
        #region ΩÃ±€≈Ê
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion
        stateD = new();
        foreach (StateBase state in states)
        {
            stateD[state.eState] = state;
            state.Init();
        }
    }

    public void ChangeState(eState next, bool force = false)
    {
        if (curState == next && !force) return;

        stateD[curState].Exit();
        curState = next;
        stateD[curState].Enter();
    }
    public void ChangeStateCutScene(eCutSceneType type, int stage)
    {
        CutSceneData cutSceneData = StageData.Instance.GetCutSceneData(type, stage);
        Model.Instance.cutSceneData = cutSceneData;
        ChangeState(eState.CutScene, force: true);
    }
}
