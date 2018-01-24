/*
 * Copyright (c) 2017 Jure Demsar
 * 
 * MIT License - see LICENCE.TXT
 * 
 * */

using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
  // list of neighbours
  [HideInInspector]
  public List<BoidController> neighbours;

  // id
  [HideInInspector]
  public int id;

  // speed
  [HideInInspector]
  public Vector3 v;

  // weights
  private float w_c, w_s, w_a, w_r;

  // radii squared
  private float r_s2, r_a2, r_r, r_r2;

  // forces
  private Vector3 f, f_s, f_a, f_c, f_r;

  // max speed
  private float v_max;

  // counters
  private int c_c, c_a;

  // offset
  Vector3 offset;

  void Start()
  {
    // init neighbours list
    neighbours = new List<BoidController>();

    // get GM
    GameManager GM = FindObjectOfType<GameManager>();

    // set weights
    w_s = GM.w_s;
    w_a = GM.w_a;
    w_c = GM.w_c;
    w_r = GM.w_r;

    // set radii
    r_s2 = GM.r_s * GM.r_s;
    r_a2 = GM.r_a * GM.r_a;
    r_r = GM.r_r;
    r_r2 = r_r * r_r;

    // maxS
    v_max = GM.v_max;

    // random v
    v = Random.insideUnitSphere.normalized * Random.Range(.0f, v_max);
  }
	
	void Update()
  {
    // reset
    f = new Vector3();
    f_s = new Vector3();
    f_a = new Vector3();
    c_a = 0;
    f_c = new Vector3();
    c_c = 0;
    f_r = new Vector3();

    // dist var
    float dist = 0;

    foreach (BoidController n in neighbours)
    {
      dist = Vector3.SqrMagnitude(transform.position - n.transform.position);
      // separation
      if (dist < r_s2)
      {
        offset = transform.position - n.transform.position;
        f_s += offset.normalized * (r_s2 - offset.sqrMagnitude);
      }
      // allignment
      else if (dist < r_a2)
      {
        f_a += n.v;
        c_a++;
      }
      // cohesion
      else
      {
        f_c += n.transform.position;
        c_c++;
      }
    }

    // roosting
    dist = Vector3.SqrMagnitude(transform.position - f_r);
    if (dist > r_r2)
    {
      f_r = -transform.position.normalized * (transform.position.magnitude - r_r);
    }

    // calculate force
    if (c_c > 0)
      f_c = (f_c / c_c) - transform.position;

    if (c_a > 0)
      f_a = (f_a / c_a);

    f = f_s * w_s + f_a * w_a + f_c * w_c + f_r * w_r;

    // velocity
    v += f * Time.deltaTime;
    v = Vector3.ClampMagnitude(v, v_max);

    // update position
    transform.forward = v.normalized;
    transform.position += v * Time.deltaTime;
	}
}
