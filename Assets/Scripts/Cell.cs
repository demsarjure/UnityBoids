/*
 * Copyright (c) 2017 Jure Demsar
 * 
 * MIT License - see LICENCE.TXT
 * 
 * */

using System.Collections.Generic;

public class Cell
{
  public List<BoidController> members;
  public List<Cell> neighborCells;

  public Cell()
  {
    members = new List<BoidController>();
    neighborCells = new List<Cell>();
  }

  public void SetNeighbours(int _x, int _y, int _z, int nCells, Cell[,,] cells)
  {
    for (int x = _x - 1; x <= _x + 1; x++)
    {
      for (int y = _y - 1; y <= _y + 1; y++)
      {
        for (int z = _z - 1; z <= _z + 1; z++)
        {
          if (x >= 0 && x < nCells && y >= 0 && y < nCells && z >= 0 && z < nCells && (x != _x || y != _y || z != _z))
          {
            neighborCells.Add(cells[x, y, z]);
          }
        }
      }
    }
  }
}
