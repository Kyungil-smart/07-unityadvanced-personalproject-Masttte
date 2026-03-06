using UnityEngine;

[CreateAssetMenu(fileName = "UnitSO", menuName = "Unit/UnitSO")]
public class UnitSO : ScriptableObject
{
    [Header("Id")]
    public Nexus nexus;
    public string unit;
    [TextArea] public string desc;

    [Header("Stats")]
    public int hp;
    public int lucidity;
    public int atk;
    public int def;
}
