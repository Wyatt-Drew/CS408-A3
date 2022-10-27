using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineController : MonoBehaviour
{
    private LineRenderer lr;
    private Transform[] points;
    [SerializeField] private lineController line;

    public void drawLine(Transform[] points)
    {
        lr = gameObject.GetComponent<LineRenderer>();
        lr.enabled = true;
        lr.positionCount = points.Length;
        this.points = points;
        for (int i = 0; i < points.Length; i++)
        {
            lr.SetPosition(i, points[i].position);
        }
        draw(points);
    }
    public void draw(Transform[] points)
    {
        line.drawLine(points);
    }
}
