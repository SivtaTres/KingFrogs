using System;
using Godot;
using Array = Godot.Collections.Array;

namespace Morozurfu.Screens
{
    public class MainMenu : Control
    {
        private Array _buttons = new Array();
        private TextureButton _currentButton = new TextureButton();
        private int _crutchButton = 0;
        public override void _Ready()
        {
            GetTree().Paused = false;
            _buttons = GetNode<VBoxContainer>("Buttons").GetChildren();
            foreach (TextureButton textureButton in _buttons)
            {
                textureButton.Connect("mouse_entered", this, "ChangeCrutch", new Array { textureButton });
            }
            ChangeFocus();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Input.IsActionJustPressed("down"))
            {
                _crutchButton = Math.Min(3, _crutchButton + 1);
                ChangeFocus();
            }
            if (Input.IsActionJustPressed("up"))
            {
                _crutchButton = Math.Max(0, _crutchButton - 1);
                ChangeFocus();
            }
            if (Input.IsActionJustPressed("back"))
            {
                Quit();
            }
        }

        public void ChangeFocus()
        {
            _currentButton = (TextureButton)_buttons[_crutchButton];
            _currentButton.GrabFocus();
        }

        public void ChangeCrutch(TextureButton textureButton)
        {
            _crutchButton = _buttons.IndexOf(textureButton);
            ChangeFocus();
        }

        public void Play()
        {
            GetTree().ChangeScene("res://Levels/01.tscn");
        }

        public void SelectLevel()
        {
            GetTree().ChangeScene("res://Screens/1LevelSelect.tscn");
        }

        public void Controls()
        {
            GetTree().ChangeScene("res://Screens/Controls.tscn");
        }

        public void Quit()
        {
            GetTree().Quit();
        }
    }
}