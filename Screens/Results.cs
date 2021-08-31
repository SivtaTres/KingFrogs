using Godot;
using Morozurfu.Levels;

namespace Morozurfu.Screens
{
    public class Results : CanvasLayer
    {
        public int _levelNumber;
        public Control _results;
        public VBoxContainer _vBoxContainer;
        public HBoxContainer _hBoxContainer;
        public ColorRect _background;
        public Label _label;
        public Button _restart;
        public Button _nextLevel;
        public Button _menu;
        public override void _Ready()
        {
            _results = GetNode<Control>("Results");
            _background = _results.GetNode<ColorRect>("ColorRect");
            _vBoxContainer = _results.GetNode<VBoxContainer>("VBoxContainer");
            _label = _vBoxContainer.GetNode<Label>("Label");
            _hBoxContainer = _vBoxContainer.GetNode<HBoxContainer>("HBoxContainer");
            _restart = _hBoxContainer.GetNode<Button>("Restart");
            _nextLevel = _hBoxContainer.GetNode<Button>("NextLevel");
            _menu = _hBoxContainer.GetNode<Button>("Menu");
        }
        public void Beaten(bool victory)
        {
            _background.Color = Global.LocationColors[Global.Area];
            _results.Show();

            if (!victory)
            {
                _restart.Show();
                _label.Text = "Try again";
            }
            else
            {
                if (_levelNumber != Global.NumberOfLevels)
                    _nextLevel.Show();
                if (_levelNumber == 8)
                {
                    _nextLevel.Text = "To frozen lake!";
                    StyleBoxFlat styleBox = new StyleBoxFlat();
                    Color color = new Color(0.38f, 0.73f, 0.96f);
                    styleBox.BgColor = color;
                    _nextLevel.AddStyleboxOverride("normal", styleBox);
                    StyleBoxFlat styleBox2 = new StyleBoxFlat();
                    color = new Color(0.14f, 0.27f, 0.35f);
                    styleBox2.BgColor = color;
                    _nextLevel.AddStyleboxOverride("pressed", styleBox2);
                }
                _label.Text = "You win!";
                if (_levelNumber == 16)
                {
                    _label.Text = "Thanks for playing!";
                    if (!((Global.Level16Beaten.Contains(4) && Global.Level16Beaten.Contains(5))))
                    {
                        _nextLevel.Hide();
                        _restart.Show();
                        _restart.Text = "Second solution?";
                    }
                }
            }

            var tween = GetChild(0).GetNode<Tween>("Tween");
            tween.InterpolateProperty(
                GetChild(0).GetNode<VBoxContainer>("VBoxContainer"),
                "rect_position",
                _vBoxContainer.RectPosition - new Vector2(0, 180),
                _vBoxContainer.RectPosition,
                (float) 0.15,
                0,
                0);
            tween.Start();
        }

        private void Restart()
        {
            GetTree().ReloadCurrentScene();
        }

        private void NextLevel()
        {
            if (_levelNumber == Global.NumberOfLevels)
            {
                Menu();
                return;
            }
            int i = _levelNumber + 1;
            string s = i.ToString("00.##");
            GetTree().ChangeScene($"res://Levels/{s}.tscn");
        }

        private void Menu()
        {
            GetTree().ChangeScene("res://Screens/MainMenu.tscn");
        }
    }
}