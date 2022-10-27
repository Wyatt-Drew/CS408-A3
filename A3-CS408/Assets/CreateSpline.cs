using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSpline : MonoBehaviour
{
    [SerializeField] private Transform sphere;
    private float interpolateAmount = 0f;
    private float speed = 1f;
    private int size = 11;
    GameObject[] controlPoints = new GameObject[11];
    public GameObject tempObject;
    private bool drawLine = false;
    public List <Transform> points = new List<Transform>();
    private bool isLoaded;
    private double startTime;
    const int linear = 1;
    const int sine = 2;
    const int parabolic = 3;
    private int moveMode = 1;

    void Start()
    {
        isLoaded = false;
        Application.targetFrameRate = 60;
        spawnControlPoints();
        if (!drawLine)
        {
            GameObject instance = Instantiate(Resources.Load("LineRend", typeof(GameObject))) as GameObject;
            instance.transform.position = GameObject.Find("/LineRend").transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //useful debug helper to ensure each animation plays for exactly 300 frames aka 5 seconds.
        //float temp2 = interpolateAmount + Time.deltaTime * speed;
        //if (temp2 % 1f != temp2)
        //{
        //    Debug.Log(Time.frameCount);
        //}
        //interpolateAmount = (interpolateAmount + Time.deltaTime * speed) % 1f;

        //Save time of start up to ensure animation plays from the start
        if (isLoaded == false)
        {
            isLoaded = true;
            startTime = Time.realtimeSinceStartupAsDouble;
        }
        switch (moveMode)
        {
            case linear:
                linearMove();
                break;
            case sine:
                sineMove();
                break;
            case parabolic:
                parabolicMove();
                break;
        }
        Vector3[] level1 = new Vector3[size];
        for (int i = 0; i < size - 1; i++)
        {
            level1[i] = Vector3.Lerp(controlPoints[i].transform.position, controlPoints[i + 1].transform.position, interpolateAmount);
        }
        int j = 1;
        while (j < size - 1)
        {
            for (int i = 0; i < size - 1 - j; i++)
            {
                level1[i] = Vector3.Lerp(level1[i], level1[i + 1], interpolateAmount);
            }
            j = j + 1;
        }
        sphere.position = level1[0];
        if (drawLine)
        {
            recordPosition();
        }
    }
    void linearMove()
    {
        interpolateAmount = (float)((Time.realtimeSinceStartupAsDouble - startTime) / 5f) % 1f;
    }
    void sineMove()
    {
        float progress = (float)((Time.realtimeSinceStartupAsDouble - startTime) / 5f) % 1f;
        if (progress <= 1f / 6f)
        {

        }
        else if (progress >= 5f / 6f)
        {
            // = progress + 2/6 - ( sine 1/6)
        }
        else
        {

        }
    }
    void parabolicMove()
    {
        float progress = (float)((Time.realtimeSinceStartupAsDouble - startTime) / 5f) % 1f;
        if (progress <= 1f/6f)
        {
            //interpolateAmount = 
        }
        else if (progress >= 5f / 6f)
        {

        } else
        {

        }
    }
    void spawnControlPoints()
    {
        Vector3[] locations = new Vector3[]
            {
                new Vector3( 0.0f,  0.0f, 0.0f),
                new Vector3( 1.0f,  3.5f, 0.0f),
                new Vector3( 4.8f,  1.8f, 0.0f),
                new Vector3( 6.5f,  7.0f, 0.0f),
                new Vector3( 9.0f,  3.5f, 0.0f),
                new Vector3(32.5f,  4.8f, 0.0f),
                new Vector3(33.2f,  2.6f, 0.0f),
                new Vector3(36.8f,  7.0f, 0.0f),
                new Vector3(37.8f,  5.0f, 0.0f),
                new Vector3(41.2f, 20.5f, 0.0f),
                new Vector3(41.5f, 21.5f, 0.0f)
            };
        for (int i = 0; i < size; i++)
        {
            controlPoints[i] = Instantiate(tempObject, locations[i], Quaternion.identity) as GameObject;
        }
    }
    void recordPosition()
    {
        GameObject current = new GameObject();
        current.transform.position = sphere.position;
        points.Add(current.transform);
        float temp = interpolateAmount + Time.deltaTime * speed;
        if (temp % 1f != temp)
        {
            drawLine = false;
            Transform[] points2 = new Transform[points.Count];
            points2 = points.ToArray();
            FindObjectOfType<lineController>().drawLine(points2);
        }
    }

}
