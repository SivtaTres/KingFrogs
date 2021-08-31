using Godot;
using Morozurfu.Characters;
using System;

namespace Morozurfu.Items
{
    public class StopIt : Item
    {
        [Export] public int TurnStop;

        private float _rotato;
        private Tween _tween;
        private Timer _tweenTimer;

        public override void _Ready()
        {
            base._Ready();
            GetNode<AnimationPlayer>("AnimationPlayer").Stop();
            _rotato = (float)GD.RandRange(0.4, 1.2);
            Rotation = _rotato;
            _tween = GetNode<Tween>("Tween");
            _tweenTimer = GetNode<Timer>("TweenTimer");
            _tween.InterpolateProperty(this, "rotation", Rotation, Rotation + _rotato * 2 * Math.Sign(Rotation) * -1, (float)1, Tween.TransitionType.Sine, Tween.EaseType.Out);
            _tween.Start();
        }
        public void TweenTimer()
        {
            _tween.InterpolateProperty(this, "rotation", Rotation, Rotation + _rotato * 2 * Math.Sign(Rotation)*-1, (float)1, Tween.TransitionType.Sine, Tween.EaseType.Out);
            _tween.Start();
        }

        public override void AreaEntered(Area2D area)
        {
            if (area.IsInGroup("Player"))
                return;
            var bot = (Enemy) area;
            bot.TimeStop(TurnStop);
            QueueFree();
        }
    }
}