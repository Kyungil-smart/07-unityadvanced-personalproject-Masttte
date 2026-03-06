using UnityEngine;

/// <summary>
/// 현재 게임상태를 동적으로 관리. 싱글톤SO
/// </summary>
public class Model : ScriptableObject
{
    private static Model _instance;
    public static Model Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<Model>("Manager/Model");
            }
            return _instance;
        }
    }

    // 스테이지 data
    // 인게임 대화 data
    public int cutStage;
    public eState eState;
    public CutSceneData cutSceneData;
}
