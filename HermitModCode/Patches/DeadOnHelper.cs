namespace HermitMod.Patches;

/// <summary>
/// Static helper tracking Dead On state for the current card play.
/// Dead On triggers when a card is played from the middle position of the hand.
/// </summary>
public static class DeadOnHelper
{
    private static bool _currentCardIsDeadOn;
    private static int _deadOnTriggersThisTurn;

    /// <summary>
    /// When true, the next card played will have Dead On forced on.
    /// Used by Cheat card to propagate Dead On to the selected card.
    /// Reset after the next card's Dead On check.
    /// </summary>
    public static bool ForceNextDeadOn { get; set; }

    public static bool IsDeadOn => _currentCardIsDeadOn;
    public static int DeadOnTriggersThisTurn => _deadOnTriggersThisTurn;

    public static void IncrementDeadOnCount()
    {
        _deadOnTriggersThisTurn++;
    }

    public static void ResetTurnCount()
    {
        _deadOnTriggersThisTurn = 0;
    }

    /// <summary>
    /// Determines if a card at the given index is in the middle of a hand of the given size.
    /// For odd hand sizes: exact middle position.
    /// For even hand sizes: either of the two middle positions.
    /// </summary>
    public static bool IsMiddlePosition(int cardIndex, int handSize)
    {
        if (handSize <= 0)
            return false;
        if (handSize == 1)
            return true;

        int middle = handSize / 2;
        if (handSize % 2 == 1)
            return cardIndex == middle;

        return cardIndex == middle - 1 || cardIndex == middle;
    }

    internal static void SetDeadOn(bool value)
    {
        _currentCardIsDeadOn = value;
    }
}
