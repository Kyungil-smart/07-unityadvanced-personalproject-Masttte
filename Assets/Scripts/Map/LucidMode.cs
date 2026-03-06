using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

/// <summary>
/// 루시드 모드: 서로 다른 두 유닛의 조건을 교환하는 시스템
/// </summary>
public class LucidMode : MonoBehaviour
{
    public static LucidMode Instance;

    public bool canActive;
    public bool isActive;

    // 선택 상태
    UnitBase _firstUnit;
    int _firstBehaviorIdx;
    int _firstCondIdx;
    bool _hasFirst;
    Button _firstButton;

    // 오버레이
    VisualElement _overlay;
    const string SHIMMER_ID = "lucid_shimmer";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateOverlay();
    }

    void CreateOverlay()
    {
        var docRoot = GameManager.Instance.gameOverDoc.rootVisualElement;

        _overlay = new VisualElement { name = "lucid-overlay" };
        _overlay.style.position = Position.Absolute;
        _overlay.style.left = 0; _overlay.style.top = 0;
        _overlay.style.right = 0; _overlay.style.bottom = 0;
        _overlay.style.display = DisplayStyle.None;
        _overlay.pickingMode = PickingMode.Ignore;

        // 어두운 배경
        var bg = new VisualElement();
        bg.style.position = Position.Absolute;
        bg.style.left = 0; bg.style.top = 0;
        bg.style.right = 0; bg.style.bottom = 0;
        bg.style.backgroundColor = new Color(0.02f, 0f, 0.1f, 0.55f);
        bg.pickingMode = PickingMode.Ignore;
        _overlay.Add(bg);

        // 시머 바 11개
        for (int i = 0; i < 11; i++)
        {
            var shimmer = new VisualElement();
            shimmer.style.position = Position.Absolute;
            shimmer.style.width = Length.Percent(250);
            shimmer.style.height = Length.Percent(2);
            shimmer.style.left = Length.Percent(-75);
            shimmer.style.backgroundColor = new Color(0.3f, 0.7f, 1f, 0.06f);
            shimmer.style.rotate = new Rotate(new Angle(-12));
            shimmer.pickingMode = PickingMode.Ignore;
            _overlay.Add(shimmer);
        }

        docRoot.Insert(0, _overlay);
    }

    #region 진입 / 해제
    public void Enter()
    {
        isActive = true;
        _hasFirst = false;
        _firstUnit = null;
        _firstButton = null;

        // 오버레이 페이드인
        _overlay.style.display = DisplayStyle.Flex;
        _overlay.style.opacity = 0;
        DOTween.To(() => 0f, x => _overlay.style.opacity = x, 1f, 0.5f)
            .SetEase(Ease.OutCubic).SetId(SHIMMER_ID);

        // 시머 바 무한 루프 애니메이션
        int idx = 0;
        foreach (var child in _overlay.Children())
        {
            if (idx == 0) { idx++; continue; } // bg 건너뛰기
            float startY = (idx - 1) * 18f - 10f;
            child.style.top = Length.Percent(startY);
            DOTween.To(
                () => startY,
                y => child.style.top = Length.Percent(y),
                startY + 130f, 5f + idx * 0.7f
            ).SetLoops(-1, LoopType.Restart).SetId(SHIMMER_ID);
            idx++;
        }
    }

    public void Exit()
    {
        isActive = false;
        _hasFirst = false;

        if (_firstButton != null)
        {
            _firstButton.style.backgroundColor = StyleKeyword.Null;
            _firstButton = null;
        }
        _firstUnit = null;

        DOTween.Kill(SHIMMER_ID);
        DOTween.To(() => 1f, x => _overlay.style.opacity = x, 0f, 0.3f)
            .OnComplete(() => _overlay.style.display = DisplayStyle.None);
    }
    #endregion

    #region 선택 / 스왑
    public void SelectCondition(UnitBase unit, int behaviorIdx, int condIdx, Button button)
    {
        if (!isActive) return;

        // 첫 번째 선택
        if (!_hasFirst)
        {
            _firstUnit = unit;
            _firstBehaviorIdx = behaviorIdx;
            _firstCondIdx = condIdx;
            _firstButton = button;
            _hasFirst = true;
            button.style.backgroundColor = new Color(0.2f, 0.85f, 0.4f, 0.3f);
            return;
        }

        // 같은 세력 → 재선택
        if (unit.nexus == _firstUnit.nexus)
        {
            _firstButton.style.backgroundColor = StyleKeyword.Initial;
            _firstUnit = unit;
            _firstBehaviorIdx = behaviorIdx;
            _firstCondIdx = condIdx;
            _firstButton = button;
            button.style.backgroundColor = new Color(0.2f, 0.85f, 0.4f, 0.3f);
            return;
        }

        // 다른 세력 → 스왑 애니메이션 후 교환
        button.style.backgroundColor = new Color(0.2f, 0.85f, 0.4f, 0.3f);
        PlaySwapAnimation(_firstButton, button, () =>
        {
            Swap(
                _firstUnit, _firstBehaviorIdx, _firstCondIdx,
                unit, behaviorIdx, condIdx
            );
            RefreshUI();
            Exit();
        });
    }

    void Swap(UnitBase unitA, int bA, int cA, UnitBase unitB, int bB, int cB)
    {
        CondBase temp = unitA.behaviors[bA].conditions[cA];
        unitA.behaviors[bA].conditions[cA] = unitB.behaviors[bB].conditions[cB];
        unitB.behaviors[bB].conditions[cB] = temp;

        unitA.behaviors[bA].conditions[cA].behavior = unitA.behaviors[bA];
        unitB.behaviors[bB].conditions[cB].behavior = unitB.behaviors[bB];
    }
    #endregion

    #region 애니메이션
    void PlaySwapAnimation(Button a, Button b, System.Action onComplete)
    {
        Sequence seq = DOTween.Sequence();

        // 1. 축소 + 초록 강조
        float t1 = 0;
        seq.Append(DOTween.To(() => t1, x =>
        {
            t1 = x;
            float s = Mathf.Lerp(1f, 0.85f, x);
            a.style.scale = new Scale(new Vector3(s, s, 1));
            b.style.scale = new Scale(new Vector3(s, s, 1));
            float alpha = Mathf.Lerp(0.3f, 0.6f, x);
            a.style.backgroundColor = new Color(0.2f, 1f, 0.5f, alpha);
            b.style.backgroundColor = new Color(0.2f, 1f, 0.5f, alpha);
        }, 1f, 0.2f).SetEase(Ease.InOutSine));

        // 2. 확대 + 밝은 플래시
        float t2 = 0;
        seq.Append(DOTween.To(() => t2, x =>
        {
            t2 = x;
            float s = Mathf.Lerp(0.85f, 1.15f, x);
            a.style.scale = new Scale(new Vector3(s, s, 1));
            b.style.scale = new Scale(new Vector3(s, s, 1));
            float alpha = Mathf.Lerp(0.6f, 0.9f, x);
            a.style.backgroundColor = new Color(0.5f, 1f, 0.7f, alpha);
            b.style.backgroundColor = new Color(0.5f, 1f, 0.7f, alpha);
        }, 1f, 0.15f).SetEase(Ease.OutQuad));

        // 3. 원래 크기로 복귀 + 페이드아웃
        float t3 = 0;
        seq.Append(DOTween.To(() => t3, x =>
        {
            t3 = x;
            float s = Mathf.Lerp(1.15f, 1f, x);
            a.style.scale = new Scale(new Vector3(s, s, 1));
            b.style.scale = new Scale(new Vector3(s, s, 1));
            float alpha = Mathf.Lerp(0.9f, 0f, x);
            a.style.backgroundColor = new Color(0.5f, 1f, 0.7f, alpha);
            b.style.backgroundColor = new Color(0.5f, 1f, 0.7f, alpha);
        }, 1f, 0.15f).SetEase(Ease.InOutSine));

        seq.OnComplete(() =>
        {
            a.style.scale = StyleKeyword.Null;
            b.style.scale = StyleKeyword.Null;
            a.style.backgroundColor = StyleKeyword.Null;
            b.style.backgroundColor = StyleKeyword.Null;
            onComplete?.Invoke();
        });
    }
    #endregion

    void RefreshUI()
    {
        var gm = GameManager.Instance;
        if (gm.uiLeft.currentUnit != null)
            gm.uiLeft.ShowUnitInfo(gm.uiLeft.currentUnit);
        if (gm.uiRight.currentUnit != null)
            gm.uiRight.ShowUnitInfo(gm.uiRight.currentUnit);
    }
}
