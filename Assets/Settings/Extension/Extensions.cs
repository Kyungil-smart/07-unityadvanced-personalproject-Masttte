using UnityEngine;

public static class Extensions
{
    public static Nexus GetEnemyNexus(this Nexus nexus)
    {
        return nexus switch
        {
            Nexus.Lucid => Nexus.Nightmare,
            Nexus.Nightmare => Nexus.Lucid,
            Nexus.Nature => Random.Range(0, 2) == 0 ? Nexus.Lucid : Nexus.Nightmare,
            _ => Nexus.Nature
        };
    }
}