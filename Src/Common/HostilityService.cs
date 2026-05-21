using System.Collections.Generic;

namespace ApexOverride.Common;

public enum Hostility
{
    Allied,
    Neutral,
    Hostile
}

public enum Faction
{
    Neutral,
    Player,
    Enemy
}

public static class HostilityService
{
    private static readonly Dictionary<(Faction, Faction), Hostility> Relations = new();

    public static void Bootstrap()
    {
        Relations.Clear();
        SetHostility(Faction.Player, Faction.Enemy, Hostility.Hostile);
    }

    public static void SetHostility(Faction a, Faction b, Hostility rel, bool symmetric = true)
    {
        Relations[(a, b)] = rel;
        if (symmetric) Relations[(b, a)] = rel;
    }

    public static Hostility GetHostility(Faction a, Faction b) =>
        a == b
            ? Hostility.Allied
            : Relations.GetValueOrDefault((a, b), Hostility.Neutral);
}
