using Godot;
using System.Linq;
using System.Collections.Generic;

public class UI : CanvasLayer
{
    private int _defaultTiles = -1;
    private List<int> _buttonToTile = new List<int>();
    private List<int> _numberOfTiles = new List<int>();
    private List<Label> _labels = new List<Label>();
    private Node _container;
    private TileMap _ground;

    public override void _Ready()
    {
        _container = GetNode<Control>("Control").GetChild(0);
        _ground = GetParent().GetParent().GetNode<TileMap>("Ground");
        //GetNode<ColorRect>("ColorRect").Show();
    }

    public void InitializeUI(List<int> tiles, int level_number)
    {
        var j = 0;
        var saved_j = 0;
        _numberOfTiles = tiles;
        for (var i = 0; i < Global.NumberOfTileTypes; i++)
        {
            if (tiles[i] == 0)
                continue;
            j++;
            if (tiles[i] == 100)
            {
                _defaultTiles = i;
                saved_j = j;
                _buttonToTile.Insert(0, i);
            }
            else
                _buttonToTile.Add(i);

            TextureRect texture = (TextureRect)_container.GetChild(i);
            texture.Show();
            var label = texture.GetNode<Label>("Label");
            label.Text = tiles[i].ToString();
            if (i != _defaultTiles)
                _labels.Add(label);
        }

        var defaultTile = _container.GetChild(_defaultTiles);
        _container.MoveChild(defaultTile, 0);
        var label0 = _container.GetChild(0).GetNode<Label>("Label");
        _labels.Insert(0, label0);

        _labels[0].Text = "";
    }

    public void SpawnItem(Vector2 position, int buttonPressed)
    {
        if (buttonPressed + 1 > _buttonToTile.Count)
            return;
        var changingTile = _ground.GetCellv(position);
        var settingTile = _buttonToTile[buttonPressed];
        var changingButton = _buttonToTile.IndexOf(changingTile);
        if (_numberOfTiles[settingTile] <= 0)
            return;

        _ground.SetCellv(position, settingTile);
        _numberOfTiles[changingTile]++;
        _numberOfTiles[settingTile]--;

        _labels[changingButton].Text = _numberOfTiles[changingTile].ToString();
        _labels[buttonPressed].Text = _numberOfTiles[settingTile].ToString();

        _labels[0].Text = "";
        
        GetNode<AudioStreamPlayer>("Spawn").Play();
    }

    public void Hide()
    {
        GetNode<Control>("Control").Hide();
    }
}