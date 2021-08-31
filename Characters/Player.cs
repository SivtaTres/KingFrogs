using System.Linq;
using Godot;

namespace Morozurfu.Characters
{
    public abstract class Player : Character
    {
        [Signal]
        public delegate void SpawnItem(Vector2 pos, string item);

        public bool Deactivated = true;

        private bool _canPlace = false;
        private string[] _items;
        private AudioStreamPlayer _move;

        public override void _Ready()
        {
            base._Ready();
            _items = Enumerable.Range(1, Global.NumberOfTileTypes)
                .Select(number => number.ToString())
                .ToArray();
            _move = GetNode<AudioStreamPlayer>("Move");
        }

        public override void _Process(float delta)
        {
            if (Deactivated || !CanMove)
                return;

            BlockPlacing(GetOverlappingAreas().Count);
            foreach (var dir in Moves.Keys.Where(Input.IsActionJustPressed))
                Move(dir);

            

            if (!_canPlace)
                return;
            foreach (var item in _items)
                if (Input.IsActionJustPressed(item))
                    EmitSignal("SpawnItem", Position / 64, int.Parse(item) - 1);
        }

        private void Move(string direction)
        {
            var currentDirection = Moves[direction];
            var currentRaycast = Raycasts[direction];

            if (currentRaycast.IsColliding())
                return;

            CanMove = false;
            MoveTween.InterpolateProperty(this,
                "position",
                Position,
                Position + currentDirection * 64,
                0.03f,
                0,
                0);
            MoveTween.Start();
            _move.Play();
        }

        private void BlockPlacing(int overlappingAreas)
        {
            GetNode<Sprite>("Sprite").Frame = overlappingAreas;
            _canPlace = overlappingAreas == 0;

        }

        public void FrozenLake()
        {
            _move = GetNode<AudioStreamPlayer>("MoveSnow");
        }

        public void Activate()
        {
            Deactivated = false;
        }
    }
}