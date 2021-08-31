using System.Collections.Generic;
using Godot;

namespace Morozurfu.Characters
{
    public class Character : Area2D
    {
        protected bool CanMove = true;
        protected AnimationPlayer AnimPlayer;
        protected Tween MoveTween;
        protected readonly Dictionary<string, Vector2>  Moves = new Dictionary<string, Vector2>();
        protected readonly Dictionary<string, RayCast2D> Raycasts = new Dictionary<string, RayCast2D>();

        public override void _Ready()
        {
            AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            MoveTween = GetNode<Tween>("MoveTween");

            Moves.Add("left", Vector2.Left);
            Moves.Add("right", Vector2.Right);
            Moves.Add("up", Vector2.Up);
            Moves.Add("down", Vector2.Down);
        
            Raycasts.Add("left", GetNode<RayCast2D>("RayCastLeft"));
            Raycasts.Add("right", GetNode<RayCast2D>("RayCastRight"));
            Raycasts.Add("up", GetNode<RayCast2D>("RayCastUp"));
            Raycasts.Add("down", GetNode<RayCast2D>("RayCastDown"));
        }

        public void MoveTweenDone(object obj, NodePath key)
        {
            CanMove = true;
        }
    }
}