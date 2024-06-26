using System;
using System.Collections.Generic;
using System.Numerics;
using WarSteel.Scenes;

public class MapGrid {

    private float _gridCellWidth;
    private float _gridCellHeight;
    private int _gridWidth;
    private int _gridHeight;

    private HashSet<(int,int)> _usedGridPositions = new();

    public MapGrid(float gridCellHeight, float gridCellWidth, int gridWidth, int gridHeight){
        _gridCellHeight = gridCellHeight;
        _gridCellWidth = gridCellWidth;
        _gridWidth = gridWidth;
        _gridHeight = gridHeight;
    }

    public Vector3 GetRandomUnusedGridPosition(float height){
       
        Random rand = new();
        int x,y;
        
        do {
        x = (int)rand.NextInt64(-_gridWidth,_gridWidth);
        y = (int)rand.NextInt64(-_gridHeight,_gridHeight);
        } while (_usedGridPositions.Contains((x,y)));

        _usedGridPositions.Add((x,y));

        return new Vector3(x * _gridCellWidth, height, y * _gridCellHeight);

    }

}