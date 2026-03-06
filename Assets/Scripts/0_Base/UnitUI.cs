using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

/// <summary>
/// 유닛 월드 스페이스 UI. HP바, Lucidity바, 데미지 텍스트를 관리합니다.
/// UnitBase와 같은 오브젝트에 부착합니다.
/// </summary>
public class UnitUI : MonoBehaviour
{
    ProgressBar hpBar;
    ProgressBar lucidityBar;
    UIDocument damageTextDoc;
    Transform damageTextTr;
    Label damageText;
    Vector3 textOffset;
    float _dmgOpacity;

    public void Init(int maxHp, int hp, int maxLucidity, int lucidity)
    {
        UIDocument[] docs = GetComponentsInChildren<UIDocument>(true);
        for (int i = 0; i < docs.Length; i++)
        {
            VisualElement root = docs[i].rootVisualElement;
            switch (docs[i].name)
            {
                case "Hp":
                    hpBar = root.Q<ProgressBar>("bar");
                    hpBar.highValue = maxHp;
                    hpBar.value = hp;
                    hpBar.title = hp.ToString();
                    break;
                case "Lucidity":
                    lucidityBar = root.Q<ProgressBar>("bar");
                    lucidityBar.highValue = maxLucidity;
                    lucidityBar.value = lucidity;
                    lucidityBar.title = lucidity.ToString();
                    break;
                case "DamageText":
                    damageTextDoc = docs[i];
                    damageTextTr = docs[i].transform;
                    textOffset = damageTextTr.localPosition;
                    damageTextDoc.rootVisualElement.style.display = DisplayStyle.None;
                    break;
            }
        }
    }

    public void UpdateHp(int hp)
    {
        hpBar.value = hp;
        hpBar.title = hp.ToString();
    }

    public void UpdateLucidity(int lucidity)
    {
        lucidityBar.value = lucidity;
        lucidityBar.title = lucidity.ToString();
    }

    public void ShowDamageText(int amount)
    {
        damageText = damageTextDoc.rootVisualElement.Q<Label>("damage");
        damageTextDoc.rootVisualElement.style.display = DisplayStyle.Flex;

        damageTextTr.localPosition = textOffset;
        damageText.text = amount.ToString();
        _dmgOpacity = 1f;
        damageText.style.opacity = 1f;

        damageTextTr.DOLocalMoveY(textOffset.y + 0.5f, 0.45f).SetEase(Ease.OutCirc);
        DOTween.To(
            () => _dmgOpacity,
            x => { _dmgOpacity = x; damageText.style.opacity = x; },
            0f, 0.45f
        ).SetEase(Ease.InCirc);
    }
}
