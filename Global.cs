using System;
using System.Collections.Generic;
using Godot;

public class Global : Node
{
    public static int LevelNumber = -1;
    public static int NumberOfLevels = 16;
    public static int NumberOfTileTypes = 4;
    public static int NumberOfAreas = 2;
    public static int Area = 0;
    public static List<int> Level16Beaten = new List<int>();
    public static List<Color> LocationColors = new List<Color>{ new Color(0.56f, 0.64f, 0.04f, 0.70f), new Color(0.17f, 0.76f, 0.74f, 0.70f) };
    public static Dictionary<int, bool> LevelsWithBeatenTutorial = new Dictionary<int, bool>();

    public enum TileTypes
    {
        Ground,
        Water,
        Fire,
        Ice
    }

    public enum FrogStatuses
    {
        Free,
        Slipped,
        OnHold,
        Stuck,
        Stonefied
    }

    public void TutorialDone(int levelNumber)
    {
        LevelsWithBeatenTutorial.Add(levelNumber, true);
    }
}
