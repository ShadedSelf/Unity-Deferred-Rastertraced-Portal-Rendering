using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBControllerCollisions : MonoBehaviour
{

    public LayerMask mask;

    RBControllerTest controller;

    void Start()
    {
        Time.fixedDeltaTime = .01f;
        controller = GetComponent<RBControllerTest>();
    }

    void FixedUpdate()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.BoxCast(transform.position, new Vector3(.25f, .01f, .25f) * transform.localScale.y, -transform.up, out hit, transform.rotation, 1f * transform.localScale.y, mask))
        {
            if (!controller.floored)
            {
                controller.OnFloorHit();
                controller.floored = true;
            }
            if (hit.collider.CompareTag("Wakker"))
                controller.downNormal = new Vector3(
					(Mathf.Abs(hit.normal.x) < .00001f) ? 0 : -hit.normal.x, 
                    (Mathf.Abs(hit.normal.y) < .00001f) ? 0 : -hit.normal.y,
                    (Mathf.Abs(hit.normal.z) < .00001f) ? 0 : -hit.normal.z);
        }
        else
            controller.floored = false;

        for (int i = 0; i < 4; i++)
        {
            Vector3 relDir = transform.TransformDirection(RandomExtensions.sixNormals[2 + i]);
            if (Physics.BoxCast(transform.position, new Vector3(.249f, .749f, .0001f) * transform.localScale.y, relDir,
                Quaternion.LookRotation(relDir, transform.TransformDirection(transform.up)), .5f * transform.localScale.y, mask))
            {
                controller.walled = true;
                break;
            }
            controller.walled = false;
        }
    }
}
