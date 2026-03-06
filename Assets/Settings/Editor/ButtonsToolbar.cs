using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

public class ButtonsToolbar {
    [MainToolbarElement("Project/Open Project Settings", defaultDockPosition = MainToolbarDockPosition.Left)]
    public static MainToolbarElement ProjectSettingsButton() {
        var icon = EditorGUIUtility.IconContent("SettingsIcon").image as Texture2D;
        var content = new MainToolbarContent(icon);
        return new MainToolbarButton(content, () => { SettingsService.OpenProjectSettings(); });
    }

    //[MainToolbarElement("Timescale/Reset", defaultDockPosition = MainToolbarDockPosition.Middle)]
    //public static MainToolbarElement ResetTimeScaleButton() {
    //    var icon = EditorGUIUtility.IconContent("Refresh").image as Texture2D;
    //    var content = new MainToolbarContent(icon, "Reset");
    //    var button = new MainToolbarButton(content, () => {
    //        Time.timeScale = 1f;
    //        MainToolbar.Refresh("Timescale/Slider");
    //    });
        
    //    return button;
    //}
}