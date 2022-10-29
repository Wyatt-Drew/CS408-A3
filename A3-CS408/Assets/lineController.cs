using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineController : MonoBehaviour
{
    private LineRenderer lr;
    [SerializeField] private lineController line;

    public void drawLine(Transform[] points)
    {
        lr = gameObject.GetComponent<LineRenderer>();
        lr.enabled = true;
        lr.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            lr.SetPosition(i, points[i].position);
        }
    }
}
