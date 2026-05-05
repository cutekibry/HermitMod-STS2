using MegaCrit.Sts2.Core.Entities.Players;

namespace HermitMod.Utility;

public static class DeadOnCounter
{
    static private readonly Dictionary<Player, int> CounterValues = [];
    static private readonly Dictionary<Player, bool> IsLastCardDeadOn = [];

    public static int GetCounter(Player player)
    {
        return CounterValues.GetValueOrDefault(player, 0);
    }
    public static bool GetIsLastCardDeadOn(Player player)
    {
        return IsLastCardDeadOn.GetValueOrDefault(player, false);
    }

    public static void IncreaseCounter(Player player)
    {
        CounterValues[player] = GetCounter(player) + 1;
    }
    public static void SetIsLastCardDeadOn(Player player, bool value)
    {
        IsLastCardDeadOn[player] = value;
    }

    public static void Reset()
    {
        for (int i = 0; i < CounterValues.Count; i++)
        {
            var key = CounterValues.ElementAt(i).Key;
            CounterValues[key] = 0;
        }
        for (int i = 0; i < IsLastCardDeadOn.Count; i++)
        {
            var key = IsLastCardDeadOn.ElementAt(i).Key;
            IsLastCardDeadOn[key] = false;
        }
    }
}