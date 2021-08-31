using Godot;
using Array = Godot.Collections.Array;
using System;
using Level = Morozurfu.Levels.Level;

namespace Morozurfu.Characters
{
    public class EnemyWall : Enemy
    {
        [Signal] public delegate void Continued();
        [Signal] public delegate void TileChanged(int value, Vector2 tilePosition);
        [Export] public int TurnsTillWall;
        private int _turnsSinceStart = 0;

        new public void MovementCooldown()
        {
            if (_status != Global.FrogStatuses.Stonefied)
            {
                base.MovementCooldown();
                if (_status == Global.FrogStatuses.OnHold && _timeStopDuration == 0)
                    EmitSignal("Continued");
                if (_status != Global.FrogStatuses.OnHold)
                    _turnsSinceStart++;
            }
        }

        new public void IceSlip(object obj, NodePath key)
        {
            base.IceSlip(obj, key);
            if (_turnsSinceStart == TurnsTillWall)
                TurnToStone();
        }

        async private void TurnToStone()
        {
            if (_status == Global.FrogStatuses.Slipped)
                await ToSignal(this, "MovementDone");
            if (_status == Global.FrogStatuses.OnHold)
                await ToSignal(this, "Continued");
            GetNode<AudioStreamPlayer>("Stonefied").Play();
            GetNode<AudioStreamPlayer>("Defrosting").Stop();
            GetNode<Sprite>("WallSprite").Show();
            GetNode<StaticBody2D>("WallBody").GetNode<CollisionShape2D>("WallShape").Disabled = false;
            _status = Global.FrogStatuses.Stonefied;
            EmitSignal("TileChanged", -1, new Vector2(Position/ 64 - new Vector2(0.5f, 0.5f)));
        }
    }
}