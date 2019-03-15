using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dup : MonoBehaviour {

	public int times;
	public float ym;
	public float div = 1;

	void Start ()
	{
		if (times > 0)
		{
			GameObject Go = Instantiate(this.gameObject, null);
			//Go.transform.position = Go.transform.position - Vector3.up * ym /** div*/;
			Go.transform.rotation = Quaternion.Euler(Go.transform.rotation.eulerAngles + Vector3.up * 90);
			Go.GetComponent<Dup>().times -= 1;
			Go.GetComponent<ConfigurableJoint>().connectedBody = GetComponent<Rigidbody>();
		}
		GetComponent<Rigidbody>().angularDrag = 10;
		Physics.defaultSolverIterations = 10;
		transform.localScale *= div;
		GetComponent<ConfigurableJoint>().anchor = Vector3.up;
	}

}
