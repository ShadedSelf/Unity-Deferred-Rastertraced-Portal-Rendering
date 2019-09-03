using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ccc : MonoBehaviour
{


	public Transform origin;
	public Transform end;
	public int iter = 10;


	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnDrawGizmos()
	{

		/*Gizmos.color = Color.red;
		Vector3 originV = origin.position;
		Vector3 endV = end.position;

		Vector3 del = endV - originV;
		float yr = del.y / del.x;




		Vector3 myTemp = Vector3.zero;
		myTemp.x = del.x;
		myTemp.y = del.x * yr;


		Gizmos.DrawLine(originV, originV + myTemp);*/




		Gizmos.color = Color.red;
		Gizmos.DrawLine(origin.position, end.position);

		Vector3 del = end.position - origin.position;

		float yr;
		bool yer = false;
		if (Mathf.Abs(del.x) < Mathf.Abs(del.y))
		{
			yr = Mathf.Abs(del.x) / Mathf.Abs(del.y);
			yer = true;
		}
		else
			yr = Mathf.Abs(del.y) / Mathf.Abs(del.x);


		float zr = Mathf.Abs(del.z) / Mathf.Abs(del.x);

		float max = Mathf.Max(Mathf.Abs(del.x), Mathf.Abs(del.y));
		max = Mathf.Abs(del.x);

		Vector3 st = Vector3.zero;
		for (int r = 0; r < iter; r++)
		{
			Vector3 signer = st.MultVector(del.SignVector());
			Vector3 doer = new Vector3(Mathf.Round(signer.x), Mathf.Round(signer.y), 0);

			if (yer)
				doer = new Vector3(doer.y, doer.x, doer.z);

			if ((yer && del.x >= 0 && del.y <= 0) || (yer && del.x < 0 && del.y > 0))
			{
				doer.x *= -1;
				doer.y *= -1;
			}

			if (r > max)
				break;

			float zzz = signer.z;

			zzz = Mathf.Lerp(0, del.z, (float)r / max);
			doer.y = Mathf.Round(Mathf.Lerp(0, del.y, (float)r / max));
			doer.x = Mathf.Round(Mathf.Lerp(0, del.x, (float)r / max));

			Vector3 zoer = new Vector3(doer.x, doer.y, zzz);

			Gizmos.color = Color.green;
			if (yer) Gizmos.color = Color.blue;
			Gizmos.DrawWireCube((origin.position + doer).MultVector(new Vector3(1, 1, 0)), Vector3.one);

			Gizmos.color = Color.magenta;
			Gizmos.DrawLine((origin.position + doer).MultVector(new Vector3(1, 1, 0)), origin.position + zoer);

			st.x += 1;
			st.y += yr;
			st.z += zr;
		}
	}
}
