using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityCompute
{
    public Transform Target;
    private string targetTag;
    private int rays;
    private float angle;
    private bool isPlayer;

    public VisibilityCompute(string targetTag, int rays, float angle, bool isPlayer = false)
    {
        Target = GameObject.FindGameObjectWithTag(targetTag).transform;
        this.targetTag = targetTag;
        this.rays = rays;
        this.angle = angle;
        this.isPlayer = false;
    }

    public VisibilityCompute(string targetTag, int rays, float angle)
    {
        this.targetTag = targetTag;
        this.rays = rays;
        this.angle = angle;
        this.isPlayer = true;
    }

    private bool GetRaycast(Vector3 dir, Transform transform, float distance)
    {
        bool result = false;
        RaycastHit hit = new RaycastHit();
        Vector3 pos = transform.position;

        if (Physics.Raycast(pos, dir, out hit, distance))
        {
            if (hit.collider.tag == targetTag)
            {
                result = true;
                Debug.DrawLine(pos, hit.point, Color.red);
            }
            else
            {
                Debug.DrawLine(pos, hit.point, Color.blue);
            }
        }
        else
        {
            Debug.DrawRay(pos, dir * distance, Color.green);
        }

        return result;
    }

    public bool RayToScan(Transform transform, float distance)
    {
        bool result = false;
        bool a = false;
        bool b = false;
        float j = 0;
        for (int i = 0; i < rays; i++)
        {
            var x = Mathf.Asin(j);
            var z = Mathf.Acos(j);

            j += angle * Mathf.Deg2Rad / rays;

            Vector3 dir = transform.TransformDirection(new Vector3(x, 0, z));
            if (GetRaycast(dir, transform, distance))
            {
                a = true;
            }

            if (x != 0)
            {
                dir = transform.TransformDirection(new Vector3(-x, 0, z));
                if (GetRaycast(dir, transform, distance))
                {
                    b = true;
                }
            }
        }

        if (a || b) result = true;
        return result;
    }

}
