using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public const int MaxGameStage = 4;

    public UILeft uiLeft;
    public UIRight uiRight;
    public UIDocument dialogDoc;
    public UIDocument gameOverDoc;
    public UIDocument gameClearDoc;

    VisualElement _gameOverRoot;
    VisualElement _gameClearRoot;

#if UNITY_EDITOR
    private void Reset()
    {
        uiLeft = FindFirstObjectByType<UILeft>();
        uiRight = FindFirstObjectByType<UIRight>();
        UIDocument[] docs = FindObjectsByType<UIDocument>(FindObjectsSortMode.None);

        foreach (var doc in docs)
        {
            // 하이어라키(Hierarchy) 창에 설정된 게임 오브젝트 이름으로 구분합니다
            if (doc.gameObject.name == "DialogUI")
            {
                dialogDoc = doc;
            }
            else if (doc.gameObject.name == "GameOverUI")
            {
                gameOverDoc = doc;
            }
            else if (doc.gameObject.name == "GameClearUI")
            {
                gameClearDoc = doc;
            }
        }
    }

#endif

    void Awake()
    {
        Instance = this;

        var root = gameOverDoc.rootVisualElement;
        _gameOverRoot = root.Q<VisualElement>("root");

        root.Q<Button>("title").clicked += () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        };
        root.Q<Button>("reStart").clicked += () =>
        {
            DOTween.KillAll();
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        };

        var clearRoot = gameClearDoc.rootVisualElement;
        _gameClearRoot = clearRoot.Q<VisualElement>("root");

        clearRoot.Q<Button>("title").clicked += () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        };
        clearRoot.Q<Button>("nextStage").clicked += () =>
        {
            DOTween.KillAll();
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        };
    }

    // UI 컨트롤 기능
    public void OnUnitClick(UnitBase unit)
    {
        // nexus 에 따라 UI 좌우 구분
        if (unit.data.nexus == Nexus.Lucid)
        {
            uiLeft.ShowUnitInfo(unit);
        }
        else
        {
            uiRight.ShowUnitInfo(unit);
        }
    }
    public void HideUnitInfo()
    {
        uiLeft._root.style.display = DisplayStyle.None;
        uiRight._root.style.display = DisplayStyle.None;
    }


    public async Awaitable GameOver()
    {
        await Awaitable.WaitForSecondsAsync(0.5f);
        Commander.Instance.ChangeTurn(eTurn.None).Cancel();

        Time.timeScale = 0;
        HideUnitInfo();
        _gameOverRoot.style.display = DisplayStyle.Flex;
    }

    public async Awaitable GameClear()
    {
        await Awaitable.WaitForSecondsAsync(1f);
        Commander.Instance.ChangeTurn(eTurn.None).Cancel();

        Time.timeScale = 0;
        HideUnitInfo();
        _gameClearRoot.style.display = DisplayStyle.Flex;

        int stage = Model.Instance.cutStage;
        if (stage > PlayerPrefs.GetInt("ClearStage", 0))
        {
            if (Model.Instance.cutStage == MaxGameStage) return;

            PlayerPrefs.SetInt("ClearStage", stage);
            PlayerPrefs.Save();
        }
    }
}
