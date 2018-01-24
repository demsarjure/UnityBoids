/*
 * Copyright (c) 2017 Jure Demsar
 * 
 * MIT License - see LICENCE.TXT
 * 
 * */

using System.IO;
using UnityEngine;

public class FpsLogger : MonoBehaviour
{
  // save metrics
  [Header("Settings")]
  public float refreshRate;
  public int frameLimit;
  public int boidIncrement;
  public int boidLimit;

  // helper variables
  private string fileName;
  private int frame = 1;
  private float refreshTimer;
  private float fps;
  private int startN;
  private string dir;

  // Game Manager
  private GameManager GM;
  void Start()
  {
    // GameManager 
    GM = FindObjectOfType<GameManager>();
    startN = GM.n;

    // name
    dir = "Analysis\\";
    if (!Directory.Exists(dir))
    {
      Directory.CreateDirectory(dir);
    }

    InitLog();
  }

  void InitLog()
  {
    fileName = dir + GM.sT + ".csv";

    using (StreamWriter sw = File.CreateText(fileName))
    {
      sw.WriteLine("N,frame,fps");
    }
  }

  void Update()
  {
    // decrease timer
    refreshTimer -= Time.deltaTime;

    if (refreshTimer < .0f)
    {
      // calculate fps
      fps = 1.0f / Time.deltaTime;

      // reset timer
      refreshTimer = refreshRate;

      if (frame > frameLimit)
      {
        GM.n += boidIncrement;

        if (GM.n > boidLimit)
        {
          if (GM.sT == Enums.SearchType.SP)
          {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
          }
          else
          {
            GM.sT++;

            GM.n = startN;

            InitLog();
          }
        }

        frame = 1;

        GM.Respawn();
      }
      else
      {
        using (StreamWriter sw = File.AppendText(fileName))
        {
          sw.WriteLine(GM.n + "," + frame + "," + fps);
        }

        // next frame
        frame++;
      }
    }
  }
}