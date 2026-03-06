using System.Threading.Tasks;
using UnityEngine;

public class Stage4 : MonoBehaviour
{
    int step = 0;
    bool cleared;
    public tStart tStart;

    void Start()
    {
        Model.Instance.cutStage = 4;
        Model.Instance.eState = eState.CutScene;
        LucidMode.Instance.canActive = false;

        StateManager.Instance.ChangeStateCutScene(eCutSceneType.PreGame, 4);
    }

    private async Awaitable Update()
    {
        if (Commander.Instance.turnCount >= 2 && step == 0)
        {
            step++;
            StateManager.Instance.ChangeStateCutScene(eCutSceneType.Play, 4);
            LucidMode.Instance.canActive = true;
        }

        if (LucidMode.Instance.isActive && step == 1)
        {
            step++;
            await Awaitable.WaitForSecondsAsync(1);
            StateManager.Instance.ChangeStateCutScene(eCutSceneType.EndGame, 4);
        }


        if (!cleared && Map.Instance.NexusUnitD[Nexus.Nightmare].Count == 0)
        {
            cleared = true;
            GameManager.Instance.GameClear().Cancel();
        }
    }
}
