/*
* Copyright (c) 2017 Jure Demsar
* 
* MIT License - see LICENCE.TXT
* 
* */

using KDTree;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  [Header("Boid Prefab")]
  public GameObject boid;

  [Header("World Settings")]
  // number of boids
  public int n;
  // roosting weight
  public float w_r;
  // size of roosting area
  public float r_r;
  // search type
  public Enums.SearchType sT;

  [Header("Boid Settings")]
  // separation weight
  public float w_s;
  // separation radius
  public float r_s;
  // allignment weight
  public float w_a;
  // allignment radius
  public float r_a;
  // cohesion weight
  public float w_c;
  // cohesion radius
  public float r_c;
  private float r_c2;
  // max speeed
  public float v_max;

  // list of boids
  List<BoidController> boids;

  // list of cells
  Cell[,,] cells;
  int nCells;
  float cOffset;


  void Start()
  {
    // rC squared
    r_c2 = r_c * r_c;

    // spawn boids
    boids = new List<BoidController>();
    Respawn();

    // cells
    nCells = Mathf.CeilToInt(r_r / r_c) + 2;
    cOffset = nCells * r_c;
    nCells *= 2;
    cells = new Cell[nCells, nCells, nCells];

    // init
    for (int x = 0; x < nCells; x++)
    {
      for (int y = 0; y < nCells; y++)
      {
        for (int z = 0; z < nCells; z++)
        {
          cells[x, y, z] = new Cell();
        }
      }
    }

    // set neighbours
    for (int x = 0; x < nCells; x++)
    {
      for (int y = 0; y < nCells; y++)
      {
        for (int z = 0; z < nCells; z++)
        {
          cells[x, y, z].SetNeighbours(x, y, z, nCells, cells);
        }
      }
    }
  }

  public void Respawn()
  {
    // destroy
    foreach (BoidController bc in boids)
    {
      Destroy(bc.gameObject);
    }

    // spawn boids
    boids.Clear();
    for (int i = 0; i < n; i++)
    {
      GameObject go = Instantiate(boid, Random.insideUnitSphere * r_r, Random.rotation);
      BoidController bc = go.GetComponent<BoidController>();
      bc.id = i;
      boids.Add(bc);
    }
  }

  private void Update()
  {
    if (sT == Enums.SearchType.A)
      ArraySearch();
    else if (sT == Enums.SearchType.KD)
      KDSearch();
    else
      SPSearch();

    if (Input.GetKeyDown(KeyCode.Escape))
    {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
  }

  private void ArraySearch()
  {
    // clear neighbours
    foreach (BoidController bc in boids)
    {
      bc.neighbours.Clear();
    }

    // pair-wise search
    for (int i = 0; i < boids.Count - 1; i++)
    {
      for (int j = i + 1; j < boids.Count; j++)
      {
        float dist = Vector3.SqrMagnitude(boids[i].transform.position - boids[j].transform.position);
        if (dist < r_c2)
        {
          boids[i].neighbours.Add(boids[j]);
          boids[j].neighbours.Add(boids[i]);
        }
      }
    }
  }

  private void KDSearch()
  {
    KDTree<int> kd = new KDTree<int>(3);

    // clear neighbours and init tree
    int i = 0;
    foreach (BoidController bc in boids)
    {
      bc.neighbours.Clear();
      kd.AddPoint(new double[3] { bc.transform.position.x, bc.transform.position.y, bc.transform.position.z }, i);
      i++;
    }

    // search
    foreach (BoidController bc in boids)
    {
      NearestNeighbour<int> neighbours = kd.NearestNeighbors(new double[3] { bc.transform.position.x, bc.transform.position.y, bc.transform.position.z }, n, r_c2);
      while (neighbours.MoveNext())
      {
        int id = neighbours.Current; 
        if (id != bc.id)
        {
          bc.neighbours.Add(boids[id]);
        }
      }
    }
  }

  private void SPSearch()
  {
    // clear cells
    int x, y, z;
    for (x = 0; x < nCells; x++)
    {
      for (y = 0; y < nCells; y++)
      {
        for (z = 0; z < nCells; z++)
        {
          cells[x, y, z].members.Clear();
        }
      }
    }

    // clear neighbours and add to cell
    foreach (BoidController bc in boids)
    {
      bc.neighbours.Clear();

      // calculate index
      x = Mathf.FloorToInt((bc.transform.position.x + cOffset) / r_c);
      y = Mathf.FloorToInt((bc.transform.position.y + cOffset) / r_c);
      z = Mathf.FloorToInt((bc.transform.position.z + cOffset) / r_c);

      cells[x, y, z].members.Add(bc);

      // add to neighbours
      foreach (Cell c in cells[x, y, z].neighborCells)
      {
        c.members.Add(bc);
      }
    }

    // search
    foreach (BoidController bc in boids)
    {
      // calculate index
      x = Mathf.FloorToInt((bc.transform.position.x + cOffset) / r_c);
      y = Mathf.FloorToInt((bc.transform.position.y + cOffset) / r_c);
      z = Mathf.FloorToInt((bc.transform.position.z + cOffset) / r_c);

      foreach (BoidController neighbour in cells[x, y, z].members)
      {
        if (bc.id != neighbour.id)
        {
          float dist = Vector3.SqrMagnitude(bc.transform.position - neighbour.transform.position);
          if (dist < r_c2)
            bc.neighbours.Add(neighbour);
        }
      }
    }
  }
}
