using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public class zz : MonoBehaviour
{
	private Vector3 o;
	private Vector3 s;

    void OnEnable()
    {
		Set();
    }

	public void Set()
	{
        o = transform.position;
        s = transform.localScale;
	}

    void Update()
	{
		Ray ray = new Ray(Controller.currentCamera.transform.position, (o - Controller.currentCamera.transform.position).normalized);

		transform.position = ray.origin + ray.direction * (distance(o, Controller.currentCamera.transform.position) + sin(Time.time) * 2);
		transform.localScale = s * distance(transform.position, Controller.currentCamera.transform.position) / 2;
    }
}

