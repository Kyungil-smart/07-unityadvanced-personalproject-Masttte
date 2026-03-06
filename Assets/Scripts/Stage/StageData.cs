using System;
using UnityEngine;

public enum eCutSceneType
{
    Story,
    PreGame,
    Play,
    EndGame,
}

[Serializable]
public class CutSceneData
{
    public eCutSceneType type;
    public int stage;
    //public Sprite background;
    public CutSceneInnerData[] innerData;
}
[Serializable]
public class CutSceneInnerData
{
    public string name;
    [TextArea] public string dialog;
}
[Serializable]
public class PortraitData
{
    public string name;
    public Sprite portrait; // todo : 초상화 데이터 작업 
}

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    private static StageData _instance;
    public static StageData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<StageData>("Manager/StageData");
            }
            return _instance;
        }
    }

    public CutSceneData[] cutSceneDatas;
    public PortraitData[] portraitDatas;

    public CutSceneData GetCutSceneData(eCutSceneType type, int stage)
    {
        return Array.Find(cutSceneDatas, d => d.type == type && d.stage == stage);
    }

    public PortraitData GetPortraitData(string name)
    {
        return Array.Find(portraitDatas, d => d.name == name);
    }
}
