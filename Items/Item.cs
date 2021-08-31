using Godot;

namespace Morozurfu.Items
{
    public abstract class Item : Area2D
    {
        public abstract void AreaEntered(Area2D area);
    }
}
