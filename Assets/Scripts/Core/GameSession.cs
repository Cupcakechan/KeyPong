/// <summary>
/// The game mode chosen on the Main Menu. Static so it survives the scene load
/// into Gameplay, where GameManager reads it to pick the rules. Defaults to Classic.
/// </summary>
public enum GameMode { Classic, TimeAttack }

public static class GameSession
{
    public static GameMode Mode = GameMode.Classic;
}
