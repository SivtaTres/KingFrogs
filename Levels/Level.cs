using System;
using System.Collections.Generic;
using Godot;
using Array = Godot.Collections.Array;
using Morozurfu.Characters;
using Morozurfu.Dijkstra;
using Morozurfu.Screens;

namespace Morozurfu.Levels
{
    public class Level : Node2D
    {
        [Signal] public delegate void Move();
        [Signal] public delegate void Beaten(bool victory);

        [Export] public int GroundTiles;
        [Export] public int WaterTiles;
        [Export] public int FireTiles;
        [Export] public int IceTiles;

        public int _levelNumber;
        public int _numberOfFrogs = 0;
        public int _numberOfTurns = 0;
        public int _numberOfTurnsSlipped = 0;
        public int _numberOfTurnsStuck = 0;
        private bool _levelStarted;
        private bool _levelBeaten = false;
        private bool _canPlayMoveSound = true;
        private Tutorial _tutorial;
        private Results _results;
        private Camera2D _camera;
        private TileMap _ground;
        private TileMap _wall;
        private Player _player;
        private UI _uiNode;
        private AudioStreamPlayer _move;
        private AudioStreamPlayer _win;
        private AudioStreamPlayer _lose;
        private AudioStreamPlayer _bg;
        private Timer _enemyTimer;
        private Map _levelMap;
        private Vector2 _mapSize;
        private List<Global.FrogStatuses> _savedStatuses = new List<Global.FrogStatuses>();
        private Dictionary<Global.FrogStatuses, int> _statusCount = new Dictionary<Global.FrogStatuses, int> { };

        public override void _Ready()
        {
            GetTree().Paused = false;
            _ground = GetNode<TileMap>("Ground");
            _wall = GetNode<TileMap>("Wall");
            _player = GetNode<Player>("Player");
            _camera = GetNode<Camera2D>("Camera2D");
            _tutorial = _camera.GetNode<Tutorial>("Tutorial");
            _results = GetNode<Results>("Results");
            _uiNode = _camera.GetNode<UI>("UI");
            _move = GetNode<AudioStreamPlayer>("Move");
            _win = GetNode<AudioStreamPlayer>("Win");
            _lose = GetNode<AudioStreamPlayer>("Lose");
            _bg = GetNode<AudioStreamPlayer>("BG_Swamp");
            _enemyTimer = GetNode<Timer>("EnemyTimer");

            GD.Randomize();
            _levelNumber = int.Parse(GetTree().CurrentScene.Filename.Substring(13, 2));
            Global.LevelNumber = _levelNumber;
            Global.Area = (_levelNumber - 1) / 8;
            _tutorial._levelNumber = _levelNumber;
            _results._levelNumber = _levelNumber;
            _mapSize = _wall.GetUsedRect().Size;
            _levelMap = WallToMap();
            SetCameraLimits();
            _uiNode.InitializeUI(new List<int> { GroundTiles, WaterTiles, FireTiles, IceTiles }, _levelNumber);
            _player.Connect("SpawnItem", _uiNode, "SpawnItem");
            _tutorial.ChangeText();
            foreach (Global.FrogStatuses status in Enum.GetValues(typeof(Global.FrogStatuses)))
                _statusCount.Add(status, 0);

            if (_levelNumber > 8 && _levelNumber <= 16)
                FrozenLake();

            _bg.Play();
            if (_tutorial._hasTutorial == false)
                _player.Deactivated = false;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Input.IsActionJustPressed("start"))
            {
                if (_tutorial._hasTutorial == true)
                    _tutorial.TutorialDone();
                else if (!_levelStarted)
                    StartLevel();
            }
            if (Input.IsActionJustPressed("back"))
                GetTree().ChangeScene("res://Screens/MainMenu.tscn");
            if (Input.IsActionJustPressed("restart"))
                GetTree().ReloadCurrentScene();
        }

        private void SetCameraLimits()
        {
            var viewport = GetViewport().GetVisibleRect();
            var mapSizeRect = _mapSize * new Vector2(64, 64);
            var zoom = Math.Max(mapSizeRect.x / viewport.Size.x, mapSizeRect.y / viewport.Size.y);
            _camera.Offset = mapSizeRect / 2;
            _camera.Zoom = new Vector2(zoom, zoom);
        }

        private void StartLevel()
        {
            _levelStarted = true;
            _player.Hide();
            _player.Deactivated = true;
            _levelMap = WallToMap();
            _enemyTimer.Start();
            //_uiNode.Hide();
            _tutorial.Hide();
            
            _bg.Stop();
            GetNode<Timer>("EnemyTimer").Start();

            foreach (Enemy enemy in GetNode<Node>("Enemies").GetChildren())
            {
                _numberOfFrogs++;
                enemy.ReceiveMap(_levelMap, GetNode<Area2D>("Trophy").Position, IceTiles);
                Connect("Move", enemy, "MovementCooldown");
                if (enemy.IsInGroup("FrogWall"))
                    enemy.Connect("TileChanged", this, "FrogTurnedToStone");
            }
        }

        private Map WallToMap()
        {
            var map = new Map.Cell[(int) _mapSize.y, (int) _mapSize.x];

            foreach (Vector2 ground in _ground.GetUsedCells())
                map[(int) ground.y, (int) ground.x] = (Map.Cell) _ground.GetCellv(ground);

            foreach (Vector2 wall in _wall.GetUsedCells())
                map[(int) wall.y, (int) wall.x] = Map.Cell.Wall;

            return new Map(map);
        }

        private void MoveEnemies()
        {
            _numberOfTurns++;
            EmitSignal("Move");
            GetFrogStatuses();
            //foreach (Global.FrogStatuses status in Enum.GetValues(typeof(Global.FrogStatuses))) GD.Print($"{status}: {_statusCount[status]}");
            if (_canPlayMoveSound)
                _move.Play();
        }

        public void GetFrogStatuses()
        {
            _canPlayMoveSound = true;
            foreach (Global.FrogStatuses status in Enum.GetValues(typeof(Global.FrogStatuses)))
                _statusCount[status] = 0;

            foreach (Enemy enemy in GetNode<Node>("Enemies").GetChildren())
                _statusCount[enemy._status]++;

            foreach (Global.FrogStatuses status in _savedStatuses)
                _statusCount[status]++;

            int tempNoF = _numberOfFrogs - _statusCount[Global.FrogStatuses.Stonefied];
            if (_statusCount[Global.FrogStatuses.OnHold] == tempNoF)
                _canPlayMoveSound = false;

            if (_statusCount[Global.FrogStatuses.Stuck] == tempNoF)
            {
                _numberOfTurnsStuck++;
                if (_numberOfTurnsStuck == 4)
                {
                    ShowResults(false);
                    _canPlayMoveSound = false;
                }
            }
            else
                _numberOfTurnsStuck = 0;

            if (_statusCount[Global.FrogStatuses.Slipped] + _statusCount[Global.FrogStatuses.Stuck] == tempNoF)
            {
                _numberOfTurnsSlipped++;
                if (_numberOfTurnsSlipped == 4)
                {
                    ShowResults(false);
                    _canPlayMoveSound = false;
                }
            }
            else
                _numberOfTurnsSlipped = 0;

            _savedStatuses.Clear();
        }

        public void SaveStatusFromPreviousTurn(Global.FrogStatuses status)
        {
            _savedStatuses.Add(status);
        }

        public void ShowResults(bool victory)
        {
            if (_levelBeaten)
                return;
            _levelBeaten = true;
            _enemyTimer.Stop();

            if (victory)
            {
                _win.Play();
                if (_levelNumber == 16)
                    Global.Level16Beaten.Add(_numberOfTurns);
            }
            else
                _lose.Play();

            EmitSignal("Beaten", victory);
            GetTree().Paused = true;
        }

        public void FrogTurnedToStone(int value, Vector2 tilePosition)
        {
            foreach (Enemy enemy in GetNode<Node>("Enemies").GetChildren())
                enemy.TileChangedTo(value, tilePosition);
        }

        private void FrozenLake()
        {
            Particles2D snow = GetNode("Particles").GetNode<Particles2D>("Snow");
            snow.Show();
            snow.ProcessMaterial.Set("initial_velocity", GD.Randi() % 15);
            snow.ProcessMaterial.Set("angular_velocity", GD.Randi() % 25);
            snow.ProcessMaterial.Set("orbit_velocity", GD.RandRange(-0.01, 0.01));
            _bg = GetNode<AudioStreamPlayer>("BG_Snow");
            _player.FrozenLake();
        }
    }
}