using UnityEngine;
using UnityEngine.UIElements;

public class CutSceneState : StateBase
{
    // UI 요소
    Label _teller;
    Label _dialog;

    // 현재 재생 중인 인덱스
    public int index;

    public override void Enter()
    {
        index = 0;
        var root = GameManager.Instance.dialogDoc.rootVisualElement;
        _teller = root.Q<Label>("teller");
        _dialog = root.Q<Label>("dialog");
        root.style.display = DisplayStyle.Flex;

        Show();
        Time.timeScale = 0f;

    }
    public override void Exit()
    {
        GameManager.Instance.dialogDoc.rootVisualElement.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }

    public void Next()
    {
        index++;
        if (index >= Model.Instance.cutSceneData.innerData.Length)
        {
            StateManager.Instance.ChangeState(GetNextState());
            return;
        }
        Show();
    }

    void Show()
    {
        if (_teller == null || _dialog == null) return;
        CutSceneInnerData inner = Model.Instance.cutSceneData.innerData[index];
        _teller.text = inner.name;
        _dialog.text = inner.dialog;
    }

    /// <summary>
    /// cutSceneData.type에 따라 컷신 이후 전환할 State 결정.
    /// </summary>
    eState GetNextState() => Model.Instance.cutSceneData.type switch
    {
        eCutSceneType.Story => eState.Lobby,
        _ => eState.Play,
    };
}

