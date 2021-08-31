using Godot;
using Morozurfu.Characters;
using Morozurfu.Levels;

namespace Morozurfu.Items
{
    public class Trophy : Item
    {
        [Signal]
        public delegate void Lose();

        [Signal]
        public delegate void Win();

        private bool _lost;

        public override void AreaEntered(Area2D area)
        {
            if (area.IsInGroup("Player")) 
                return;
        
            var bot = (Enemy) area;
        
            if (bot.Type != Enemy.EnemyType.Player) 
                _lost = true;
        
            GetNode<Timer>("Timer").Start();
        }

        public void Gone()
        {
            GetParent<Level>().ShowResults(!_lost);
            QueueFree();
        }
    }
}