using UnityEngine;

public class Stage1 : MonoBehaviour
{
    int step = 0;

    [SerializeField] GameObject dummy1;
    [SerializeField] GameObject dummy2;
    [SerializeField] GameObject knight1;
    [SerializeField] GameObject knight2;

    void Start()
    {
        Model.Instance.cutStage = 1;
        Model.Instance.eState = eState.CutScene;

        StateManager.Instance.ChangeStateCutScene(eCutSceneType.PreGame, 1);
    }

    private void Update()
    {
        if (step == 0 && Map.Instance.player.root <= 4)
        {
            step++;
            StateManager.Instance.ChangeStateCutScene(eCutSceneType.Play, 1);
        }
        if (step == 1 && Map.Instance.player.root <= -1)
        {
            step++;
            StateManager.Instance.ChangeStateCutScene(eCutSceneType.EndGame, 1);
            dummy1.SetActive(false);
            dummy2.SetActive(false);
            Commander.Instance.ChangeTurn(eTurn.Start).Cancel();
            knight1.SetActive(true);
            knight2.SetActive(true);
        }

        if (step == 2 && Map.Instance.NexusUnitD[Nexus.Nightmare].Count == 0)
        {
            step++;
            GameManager.Instance.GameClear().Cancel();
        }
    }
}
