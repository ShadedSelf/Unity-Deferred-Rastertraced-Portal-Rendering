using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryComponent<T> : IDisposable where T : Component
{
	public T component { get; private set; }

	public TemporaryComponent()
	{
		var go = new GameObject();
		component =	go.GetComponent<T>() ?? go.AddComponent<T>(); //Transform case
	}
	public void Dispose() => GameObject.Destroy(component.gameObject);
}

public class TemporaryTransform : TemporaryComponent<Transform>
{
	public Transform transform => component;

	public TemporaryTransform()																: base() { }
	public TemporaryTransform(Vector3 pos)													: this(				  )	=> transform.position	= pos;
	public TemporaryTransform(Vector3 pos, Quaternion rot)									: this(pos			  )	=> transform.rotation	= rot;
	public TemporaryTransform(Vector3 pos, Quaternion rot, Vector3 scale)					: this(pos, rot		  )	=> transform.localScale	= scale;
	public TemporaryTransform(Vector3 pos, Quaternion rot, Vector3 scale, Transform parent)	: this(pos, rot, scale) => transform.parent		= parent;
	public TemporaryTransform(Transform trs)												: this(trs.position, trs.rotation, trs.localScale, trs.parent) { }
}
