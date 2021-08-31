using System;
using System.Collections.Generic;
using Godot;
using Level = Morozurfu.Levels.Level;
using Morozurfu.Dijkstra;

namespace Morozurfu.Characters
{
    public abstract class Enemy : Character
    {
        [Export(PropertyHint.Enum)] public EnemyType Type;
        [Export] public int DefaultPrice;
        [Export] public int GoodPrice;
        [Export] public int BadPrice;

        [Signal] public delegate void MovementDone();

        public int _timeStopDuration = -1;
        public Global.FrogStatuses _status;
        protected int _turnsSlipped = 0;
        protected int _iceTiles;
        protected Map _levelMap;
        protected Vector2 _trophyPosition;
        protected Vector2 _direction;
        protected AnimationPlayer _animPlayer;
        protected Dictionary<Map.Cell, int> _prices;

        public enum EnemyType
        {
            Water,
            Fire,
            Player
        }

        private readonly Dictionary<EnemyType, Map.Cell[]> _goodCells = new Dictionary<EnemyType, Map.Cell[]>
        {
            {
                EnemyType.Water,
                new[] {Map.Cell.Water, Map.Cell.Ice}
            },
            {
                EnemyType.Fire,
                new[] {Map.Cell.Fire}
            },
            {
                EnemyType.Player,
                new Map.Cell[] { }
            }
        };

        public override void _Ready()
        {
            base._Ready();
            AnimPlayer.Seek((float)GD.RandRange(0, 1.5));

            _prices = new Dictionary<Map.Cell, int>();

            for (var i = 0; i < Global.NumberOfTileTypes; i++)
                _prices.Add((Map.Cell)i, BadPrice);

            _prices[Map.Cell.Ground] = DefaultPrice;

            foreach (var cell in _goodCells[Type])
                _prices[cell] = GoodPrice;

            //_prices[Map.Cell.Ice]--;
        }
        public void ReceiveMap(Map levelMap, Vector2 trophyPosition, int iceTiles)
        {
            _levelMap = levelMap;
            _trophyPosition = trophyPosition;
            _iceTiles = iceTiles;
            _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        }
        public void TileChangedTo(int changedTo, Vector2 tilePosition)
        {
            _levelMap.ChangeCell(changedTo, tilePosition);
        }

        private void Move(Vector2 direction, double time)
        {
            MoveTween.InterpolateProperty(
                this,
                "position",
                Position,
                Position + direction * 64,
                (float)time,
                0,
                0);
            MoveTween.Start();
        }

        public void MovementCooldown()
        {
            if (_timeStopDuration > 0)
            {
                _timeStopDuration--;
                return; 
            }

            if (_status == Global.FrogStatuses.Slipped)
                GetParent().GetParent<Level>().SaveStatusFromPreviousTurn(Global.FrogStatuses.Slipped);

            _status = Global.FrogStatuses.Free;

            _direction = Dijkstra.Dijkstra.FindShortestWay(
                _levelMap,
                Position / 64 - new Vector2(0.5f, 0.5f),
                _trophyPosition / 64,
                _prices);

            if (_direction == new Vector2(0, 0))
                _status = Global.FrogStatuses.Stuck;

            Move(_direction, 0.2);
            _animPlayer.Play("go");
        }

        public void IceSlip(object obj, NodePath key)
        {
            var currentRaycast = Raycasts[DirectionToString(_direction)];
            if (_levelMap[Position / 64 - new Vector2(0.5f, 0.5f)] == Map.Cell.Ice
                && !currentRaycast.IsColliding() && _status != Global.FrogStatuses.Stuck)
            {
                if (_status != Global.FrogStatuses.Slipped)
                {
                    GetNode<AudioStreamPlayer>("Slip").Play();
                    _status = Global.FrogStatuses.Slipped;   
                }
                Move(_direction, Math.Min(0.1, 0.2 / _iceTiles));
            }
            else
                EmitSignal("MovementDone");
        }

        public void TimeStop(int duration)
        {
            _status = Global.FrogStatuses.OnHold;
            _timeStopDuration = duration;
            _animPlayer.Play("frozen");
            GetNode<AudioStreamPlayer>("Defrosting").Play();
        }

        private static string DirectionToString(Vector2 direction) =>
            direction == Vector2.Up
                ? "up"
                : direction == Vector2.Right
                    ? "right"
                    : direction == Vector2.Down
                        ? "down"
                        : "left";
    }
}