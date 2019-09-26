using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputUtils
{
	public static Vector3 KeyboardDirection(bool normalize = true)
	{
		var dir = new Vector3
		(
			Input.GetKey(KeyCode.D).ToInt() - Input.GetKey(KeyCode.A).ToInt(), 
			0,
			Input.GetKey(KeyCode.W).ToInt() - Input.GetKey(KeyCode.S).ToInt()
		);
		return (normalize) ? dir.normalized : dir;
	}
}
