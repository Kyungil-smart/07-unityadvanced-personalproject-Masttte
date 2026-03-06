using UnityEngine;

public class FavoriteObj : ScriptableObject
{
    private static FavoriteObj _instance;
    public static FavoriteObj Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<FavoriteObj>("Manager/FavoriteObj");
            }
            return _instance;
        }
    }

    public Object[] objects;
}
