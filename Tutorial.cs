using Godot;
using System.Collections.Generic;

public class Tutorial : CanvasLayer
{
    [Signal] public delegate void Done();

    public int _levelNumber = -1;
    public bool _hasTutorial = false;
    private readonly List<int> _start = new List<int> { 1, 2, 9 };
    private readonly Dictionary<int, string> _levelNames = new Dictionary<int, string>();
    private readonly Dictionary<int, string> _tutorial = new Dictionary<int, string>();

    public override void _Ready()
    {
        _levelNames.Add(1, "Yellow clan shall prevail!");
        _levelNames.Add(2, "Chromatophobia");
        _levelNames.Add(3, "Favorite colors");
        _levelNames.Add(4, "Distaste");
        _levelNames.Add(5, "Double trouble");
        _levelNames.Add(6, "Three's a crowd");
        _levelNames.Add(7, "Switcheroo!");
        _levelNames.Add(8, "Misdirection");

        _levelNames.Add(9, "Winterland");
        _levelNames.Add(10, "Slippery");
        _levelNames.Add(11, "Oversled");
        _levelNames.Add(12, "Snowfrog");
        _levelNames.Add(13, "Homestuck");
        _levelNames.Add(14, "Cryostasis");
        _levelNames.Add(15, "Defrosting");
        _levelNames.Add(16, "Two solutions");

        _tutorial.Add(1, "Lead yellow frog to crown before others!\nPress enter or space to dismiss");
        _tutorial.Add(2, "Frogs dislike other colored lilies\nMove cursor with wasd or arrow keys\nUse 1,2,3 to spawn lily pads");
        _tutorial.Add(3, "Frogs like to jump\non lilies of their color");
        _tutorial.Add(4, "Your frog dislikes all non-green tiles");
        _tutorial.Add(5, "Press r to restart the level");
        _tutorial.Add(6, "Press esc to exit to menu");
        _tutorial.Add(7, "You can use keyboard\nto navigate through menus");
        _tutorial.Add(9, "Welcome to frozen lake!");
        _tutorial.Add(10, "Frogs slip on ice");
        _tutorial.Add(12, "Snowfrog turns into snowdrift\nafter 2 jumps");
        _tutorial.Add(14, "Snowflake freezes frogs in place");
    }
    public void ChangeText()
    {
        if (_levelNames.TryGetValue(_levelNumber, out string value))
            GetNode<Label>("LevelName").Text = $"{_levelNumber.ToString()}/{Global.NumberOfLevels}: {_levelNames[_levelNumber]}";

        if (_tutorial.TryGetValue(_levelNumber, out string value2) && !Global.LevelsWithBeatenTutorial.ContainsKey(_levelNumber))
        {
            GetNode<TextureButton>("TextureButton").Disabled = false;
            _hasTutorial = true;
            GetNode<Label>("Controls").Text = _tutorial[_levelNumber];
            StyleBoxFlat styleBox = new StyleBoxFlat();
            styleBox.BgColor = Global.LocationColors[Global.Area];
            styleBox.BorderColor = Global.LocationColors[Global.Area];
            styleBox.BorderWidthLeft = 500;
            styleBox.BorderWidthRight = 500;
            styleBox.BorderWidthBottom = 30;
            GetNode<Label>("Controls").AddStyleboxOverride("normal", styleBox);
        }
    }

    public void TutorialDone()
    {
        Global.LevelsWithBeatenTutorial.Add(_levelNumber, true);
        GetNode<Label>("Controls").Hide();
        GetNode<TextureButton>("TextureButton").Disabled = true;
        _hasTutorial = false;
        EmitSignal("Done");
        if (_start.Contains(_levelNumber))
            GetNode<Label>("Start").Show();
    }

    public void Hide()
    {
        GetNode<Label>("Start").Hide();
    }
}

/*
        _hints.Add(1, "Lead yellow frog to crown before others!\nPress space to begin");
        _hints.Add(2, "Fire frog is scared of blue lilies!\nUse arrow keys to move and 1,2,3 to spawn lily pads");
        _hints.Add(3, "Water frog likes to jump on blue lilies!");
        _hints.Add(4, "Your frog is scared of non-green lilies!");
        _hints.Add(5, "You can place both red and blue tiles");
        _hints.Add(6, "Only one can be the king!");
        _hints.Add(7, "Make them swap!");
        _hints.Add(8, "Stall them for time!");
        _hints.Add(9, "Welcome to frozen lake!");
        _hints.Add(10, "Frogs slip on ice");
        _hints.Add(11, "Frogs slip all the way to the end");
        _hints.Add(12, "Snowflake freezes frogs in place!");
        _hints.Add(13, "Going sideways");
*/