using UnityEngine;
using UnityEngine.UIElements;

public class UIRight : MonoBehaviour
{
    [SerializeField] VisualTreeAsset condSlot;
    [SerializeField] VisualTreeAsset actSlot;
    [SerializeField] VisualTreeAsset _line; // 조건 밑 구분선

    [SerializeField] Texture2D turnIcon;
    [SerializeField] Texture2D atkIcon;
    [SerializeField] Texture2D moveIcon;
    [SerializeField] Texture2D distanceIcon;
    [SerializeField] Texture2D musicIcon;
    [SerializeField] Texture2D randomIcon;
    [SerializeField] Texture2D KeyAIcon;
    [SerializeField] Texture2D KeyDIcon;
    [SerializeField] Texture2D KeyWIcon;
    [SerializeField] Texture2D KeyFIcon;
    Texture2D _curIcon;

    UIDocument ui;
    public VisualElement _root;
    VisualElement _behavior; // 부모 요소
    ScrollView condScrollView;// 조건 슬롯
    ScrollView actScrollView;// 실행 슬롯

    public UnitBase currentUnit;

    void Awake()
    {
        ui = GetComponent<UIDocument>();

        var root = ui.rootVisualElement;
        _root = root.Q<VisualElement>("root");
        _behavior = root.Q<VisualElement>("Behavior");
        condScrollView = root.Q<ScrollView>("Cond");
        actScrollView = root.Q<ScrollView>("Act");
    }

    public void ShowUnitInfo(UnitBase unit)
    {
        currentUnit = unit;
        // UI 켜기
        _root.style.display = DisplayStyle.Flex;

        // 유닛의 이름, atk, def 표시
        var nameLabel = _root.Q<Label>("name");
        var descLabel = _root.Q<Label>("desc");
        var atkLabel = _root.Q<Label>("atkText");
        var defLabel = _root.Q<Label>("defText");
        nameLabel.text = unit.data.unit;
        descLabel.text = unit.data.desc;
        atkLabel.text = unit.atk.ToString();
        defLabel.text = unit.def.ToString();

        // 모든 요소 지우기
        var lines = _behavior.Query<VisualElement>("line").ToList();
        foreach (var line in lines)
        {
            line.RemoveFromHierarchy();
        }
        condScrollView.Clear();
        actScrollView.Clear();

        // 유닛 행동 가져오기
        for (int i = 0; i < unit.behaviors.Length; i++)
        {
            VisualElement lastSlot = null;
            for (int j = 0; j < unit.behaviors[i].conditions.Length; j++)
            {
                string condText = unit.behaviors[i].conditions[j].desc;
                _curIcon = null;
                switch (unit.behaviors[i].conditions[j].type)
                {
                    case Icon.Turn: _curIcon = turnIcon; break;
                    case Icon.Atk: _curIcon = atkIcon; break;
                    case Icon.Move: _curIcon = moveIcon; break;
                    case Icon.Distance: _curIcon = distanceIcon; break;
                    case Icon.Music: _curIcon = musicIcon; break;
                    case Icon.Random: _curIcon = randomIcon; break;
                    case Icon.KeyA: _curIcon = KeyAIcon; break;
                    case Icon.KeyD: _curIcon = KeyDIcon; break;
                    case Icon.KeyW: _curIcon = KeyWIcon; break;
                    case Icon.KeyF: _curIcon = KeyFIcon; break;
                }
                var slot = condSlot.Instantiate();
                var condButton = slot.Q<Button>("CondSlot");
                condButton.text = condText;
                condButton.iconImage = Background.FromTexture2D(_curIcon);

                int bIdx = i;
                int cIdx = j;
                condButton.RegisterCallback<ClickEvent>(evt =>
                {
                    if (LucidMode.Instance.isActive)
                        LucidMode.Instance.SelectCondition(unit, bIdx, cIdx, condButton);
                });

                condScrollView.Add(slot);
                lastSlot = slot;
            }
            for (int k = 0; k < unit.behaviors[i].actions.Length; k++)
            {
                string actText = unit.behaviors[i].actions[k].desc;
                _curIcon = null;
                switch (unit.behaviors[i].actions[k].type)
                {
                    case Icon.Turn: _curIcon = turnIcon; break;
                    case Icon.Atk: _curIcon = atkIcon; break;
                    case Icon.Move: _curIcon = moveIcon; break;
                    case Icon.Distance: _curIcon = distanceIcon; break;
                    case Icon.Music: _curIcon = musicIcon; break;
                    case Icon.Random: _curIcon = randomIcon; break;
                    case Icon.KeyA: _curIcon = KeyAIcon; break;
                    case Icon.KeyD: _curIcon = KeyDIcon; break;
                    case Icon.KeyW: _curIcon = KeyWIcon; break;
                    case Icon.KeyF: _curIcon = KeyFIcon; break;
                }
                var slot = actSlot.Instantiate();
                var actButton = slot.Q<Button>("ActSlot");
                actButton.text = actText;
                actButton.iconImage = Background.FromTexture2D(_curIcon);
                // 사이즈를 조건갯수 배수로 설정
                actButton.style.height = new StyleLength(Length.Pixels(64 * unit.behaviors[i].conditions.Length));
                actScrollView.Add(slot);
            }

            var lineSlot = _line.Instantiate();
            var line = lineSlot.Q<VisualElement>("line");
            _behavior.Add(line);

            // 레이아웃 계산 후 실제 bottom 위치에 선 배치
            var capturedLine = line;
            var capturedSlot = lastSlot;
            capturedSlot.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                float top = capturedSlot.worldBound.yMax - _behavior.worldBound.yMin;
                capturedLine.style.top = top - 3;
            });
        }

        // x 버튼 클릭 시 UI 끄기
        var closeButton = _root.Q<Button>("close");
        closeButton.clicked += () =>
        {
            _root.style.display = DisplayStyle.None;
        };
    }
}
