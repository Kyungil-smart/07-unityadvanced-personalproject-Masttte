using UnityEngine;

public class Stage2 : MonoBehaviour
{
    bool cleared;

    void Start()
    {
        Model.Instance.cutStage = 2;
        Model.Instance.eState = eState.CutScene;

        StateManager.Instance.ChangeStateCutScene(eCutSceneType.PreGame, 2);
    }
    private void Update()
    {
        if (!cleared && Map.Instance.NexusUnitD[Nexus.Nightmare].Count == 0)
        {
            cleared = true;
            GameManager.Instance.GameClear().Cancel();
        }
    }
}
