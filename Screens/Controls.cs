using Godot;

namespace Morozurfu.Screens
{
    public class Controls : Control
    {
        public void Back()
        {
            GetTree().ChangeScene("res://Screens/MainMenu.tscn");
        }
    }
}
