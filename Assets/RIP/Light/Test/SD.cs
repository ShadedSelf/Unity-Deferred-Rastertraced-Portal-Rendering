using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD : MonoBehaviour
{
	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

		Vector3 del = transform.position - Vector3.zero;

		float yr;
		bool yer = false;
		if (Mathf.Abs(del.x) < Mathf.Abs(del.y))
		{
			yr = Mathf.Abs(del.x) / Mathf.Abs(del.y);
			yer = true;
		}
		else
			yr = Mathf.Abs(del.y) / Mathf.Abs(del.x);

		//float max = Mathf.Max(Mathf.Abs(del.x), Mathf.Abs(del.y));

		Vector3 st = Vector3.zero;
		st.x += 1;
		st.y += yr;

		Vector3 signer = st.MultVector(del.SignVector());
		Vector3 doer = new Vector3(Mathf.Round(signer.x), Mathf.Round(signer.y), 0);

		if (yer)
			doer = new Vector3(doer.y, doer.x, doer.z);

		if ((yer && del.x >= 0 && del.y <= 0) || (yer && del.x < 0 && del.y > 0))
		{
			doer.x *= -1;
			doer.y *= -1;
		}

		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(doer, Vector3.one);
	}
}
