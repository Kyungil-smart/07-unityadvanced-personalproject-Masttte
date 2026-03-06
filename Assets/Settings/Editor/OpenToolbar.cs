using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

public class OpenToolbar
{
    [MainToolbarElement("Project/ManagerMenu", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement GameMenuDropdown()
    {
        var icon = EditorGUIUtility.IconContent("GameManager Icon").image as Texture2D;
        var content = new MainToolbarContent(icon, "Game Menu");

        // 드롭다운 생성 (클릭 시 OnOpenMenu 호출)
        return new MainToolbarDropdown(content, OnOpenMenu);
    }

    private static void OnOpenMenu(Rect rect)
    {
        var menu = new GenericMenu();

        menu.AddItem(new GUIContent("Open Favorite Objects"), false, () =>
        {
            EditorUtility.OpenPropertyEditor(FavoriteObj.Instance); // 새창
        });
        menu.AddSeparator(""); // 구분선

        menu.AddItem(new GUIContent("Open Model"), false, () =>
        {
            EditorUtility.OpenPropertyEditor(Model.Instance);
        });
        menu.AddItem(new GUIContent("Open StageData"), false, () =>
        {
            EditorUtility.OpenPropertyEditor(StageData.Instance);
        });


        menu.DropDown(rect); // 메뉴 표시
    }
}
