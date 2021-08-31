using System;
using Godot;
using Array = Godot.Collections.Array;

namespace Morozurfu.Screens
{
    public class LevelSelect : Control
    {
        private Array _buttons;
        private Button _currentButton = new Button();
        private int _crutchButton = 0;
        private int _screenNumber;
        public override void _Ready()
        {
            _screenNumber = int.Parse(GetTree().CurrentScene.Filename.Substring(14, 1));
            var i = 0;
            _buttons = GetNode("GridContainer").GetChildren();
            foreach (Button button in _buttons)
            {
                i++;
                button.Connect("pressed", this, "StartLevel", new Array() { button.Text });
                button.Connect("mouse_entered", this, "ChangeCrutch", new Array { button });
                ShortCut shortCut = new ShortCut();
                InputEventAction inputEvent = new InputEventAction();
                inputEvent.Action = i.ToString();
                shortCut.Shortcut = inputEvent;
                button.Shortcut = shortCut;
            }
            ChangeFocus();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Input.IsActionJustPressed("right"))
            {
                if (_crutchButton == -1)
                    Next();
                _crutchButton++;
                ChangeFocus();
            }
            if (Input.IsActionJustPressed("left"))
            {
                if (_crutchButton == -1)
                    Back();
                _crutchButton--;
                ChangeFocus();
            }
            if (Input.IsActionJustPressed("up"))
            {
                _crutchButton -= 4;
                ChangeFocus();
            }
            if (Input.IsActionJustPressed("down"))
            {
                _crutchButton += 4;
                ChangeFocus();
            }
        }

        private void ChangeFocus()
        {
            if (_crutchButton < 0)
            {
                Back();
                return;
            }
            if (_crutchButton > 7)
            {
                _crutchButton = 7;
                Next();
                return;
            }
            _currentButton = (Button)_buttons[_crutchButton];
            _currentButton.GrabFocus();
        }

        public void ChangeCrutch(Button button)
        {
            _crutchButton = _buttons.IndexOf(button);
            ChangeFocus();
        }

        public void StartLevel(string level)
        {
            var levelNumber = int.Parse(level);
            string slevelNumber = levelNumber.ToString("00.##");
            GetTree().ChangeScene($"res://Levels/{slevelNumber}.tscn");
        }

        public void Back()
        {
            if (_screenNumber == 1)
                GetTree().ChangeScene("res://Screens/MainMenu.tscn");
            else
                GetTree().ChangeScene($"res://Screens/{_screenNumber-1}LevelSelect.tscn");
        }

        public void Next()
        {
            if (_screenNumber == Global.NumberOfAreas)
                return;
            GetTree().ChangeScene($"res://Screens/{_screenNumber+1}LevelSelect.tscn");
        }
    }
}