using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPortableObject
{
	Transform transform	{ get;		}
	Rigidbody rb 		{ get;		}
	Vector3 center		{ get;		}
	Vector3 down		{ get; set; }
	float scale			{ get; set; }
}

public interface IPortableMesh : IPortableObject
{
	Mesh mesh		{ get; }
	Material mat	{ get; }
}