using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

public class FavoriteToolbar
{
    #region TimeScale 슬라이더
    const float k_minTimeScale = 0f;
    const float k_maxTimeScale = 5f;

    [MainToolbarElement("Project/Time Scale", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement TimeSlider()
    {
        var content = new MainToolbarContent("Time Scale", "Time Scale");
        var slider = new MainToolbarSlider(content, Time.timeScale, k_minTimeScale, k_maxTimeScale, OnSliderValueChanged);

        slider.populateContextMenu = (menu) =>
        {
            menu.AppendAction("Reset", _ =>
            {
                Time.timeScale = 1f;
                MainToolbar.Refresh("Project/Time Scale");
            });
        };

        return slider;
    }

    static void OnSliderValueChanged(float newValue)
    {
        Time.timeScale = newValue;
    }
    #endregion

    #region Favorite 드롭다운
    [MainToolbarElement("Project/Favorites", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement FavoritesDropdown()
    {
        // 아이콘 설정
        var icon = EditorGUIUtility.IconContent("Favorite Icon").image as Texture2D;
        var content = new MainToolbarContent(icon, "Favorites");

        // MainToolbarDropdown 생성
        // 두 번째 인자는 클릭 시 실행될 콜백(Action<Rect>)
        return new MainToolbarDropdown(content, OnOpenDropdown);
    }

    private static void OnOpenDropdown(Rect buttonRect)
    {
        var menu = new GenericMenu();
        var favoriteData = FavoriteObj.Instance;

        if (favoriteData == null || favoriteData.objects == null || favoriteData.objects.Length == 0)
        {
            menu.AddDisabledItem(new GUIContent("No Favorites Found (Check Resources/Manager/FavoriteObj)"));
            menu.DropDown(buttonRect);
            return;
        }

        // 목록 순회하며 메뉴 아이템 추가
        foreach (var obj in favoriteData.objects)
        {
            if (obj == null) continue;

            // 메뉴 항목 이름과 클릭 시 실행할 액션 등록
            menu.AddItem(new GUIContent(obj.name), false, () =>
            {
                SelectAndPing(obj);
            });
        }

        // 버튼 위치 아래에 드롭다운 메뉴 표시
        menu.DropDown(buttonRect);
    }

    private static void SelectAndPing(Object obj)
    {
        if (obj == null) return;

        // 1. 프로젝트 창에서 선택
        Selection.activeObject = obj;

        // 2. 창 포커스
        EditorUtility.FocusProjectWindow();

        // 3. 핑(깜빡임) 효과
        EditorGUIUtility.PingObject(obj);
    }
    #endregion
}
