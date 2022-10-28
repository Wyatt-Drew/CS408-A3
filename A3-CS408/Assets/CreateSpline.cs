using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSpline : MonoBehaviour
{
    [SerializeField] private Transform sphere;
    private float interpolateAmount = 0f;
    //private float speed = 1f;
    private int size = 11;
    GameObject[] controlPoints = new GameObject[11];
    public GameObject tempObject;
    private bool drawLine = true;
    private bool isLoaded;
    private double startTime;
    const int linear = 1;
    const int sine = 2;
    const int parabolic = 3;
    private int moveMode = 1;
    public List<Transform> points = new List<Transform>();
    public const int pointNum = 50;
    public const int maxSteps = 100;
    Vector3[] splineCurve;

    void Start()
    {
        isLoaded = false;
        Application.targetFrameRate = 60;
        spawnControlPoints();
        if (!drawLine) // Spawns a prefab line to save computation power
        {
            GameObject instance = Instantiate(Resources.Load("LineRend", typeof(GameObject))) as GameObject;
            instance.transform.position = GameObject.Find("/LineRend").transform.position;
        }
        else // Draws a custom line along whatever trajectory the object will follow
        {
            calculateSpline();
            for (int i = 0; i < (size - 3) * maxSteps; i++)
            {
                sphere.position = splineCurve[i];
                recordPosition();
            }
            drawLine = false;
            Transform[] points2 = new Transform[pointNum];
            points2 = points.ToArray();
            FindObjectOfType<lineController>().drawLine(points2);
        }

        if (false)
        {
            for (interpolateAmount = 0f; interpolateAmount < 1f; interpolateAmount = interpolateAmount + 1f/(float)pointNum)
            {
                moveBeizer();
                recordPosition();
            }
            drawLine = false;
            Transform[] points2 = new Transform[pointNum];
            points2 = points.ToArray();
            FindObjectOfType<lineController>().drawLine(points2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Save time of start up to ensure animation plays from the start
        if (isLoaded == false)
        {
            isLoaded = true;
            startTime = Time.realtimeSinceStartupAsDouble;
        }
        getInput();
        switch (moveMode)
        {
            case linear:
                linearProgress();
                break;
            case sine:
                sineProgress();
                break;
            case parabolic:
                parabolicProgress();
                break;
        }
        move();
    }
    void getInput()
    {
        foreach (char c in Input.inputString)
        {
            switch (c)
            {
                case 'b'://default mode
                    {
                        moveMode = linear;
                        break;
                    }
                case 'c'://sine mode
                    {
                        moveMode = sine;
                        break;
                    }
                case 'd'://parabolic mode
                    {
                        moveMode = parabolic;
                        break;
                    }
            }
        }
    }
    int getFrame()
    {
        return (int)(interpolateAmount * (maxSteps * ((float)(size - 3))));
    }
    void move()
    {
        sphere.position = splineCurve[getFrame()];
    }
    void calculateSpline()
    {
        splineCurve = new Vector3[maxSteps* (size-3)];
        for (int i = 0; i < controlPoints.Length-3; i++)
        {
            for (int j = 0; j < maxSteps; j++)
            {
                float u = (float)j/ (float)maxSteps;
                splineCurve[(i*maxSteps)+j].x =
                    B0(u) * controlPoints[i].transform.position.x +
                    B1(u) * controlPoints[i + 1].transform.position.x +
                    B2(u) * controlPoints[i + 2].transform.position.x +
                    B3(u) * controlPoints[i + 3].transform.position.x;
                splineCurve[(i * maxSteps)+j].y =
                    B0(u) * controlPoints[i].transform.position.y +
                    B1(u) * controlPoints[i + 1].transform.position.y +
                    B2(u) * controlPoints[i + 2].transform.position.y +
                    B3(u) * controlPoints[i + 3].transform.position.y;
                splineCurve[(i * maxSteps) + j].z =
                    B0(u) * controlPoints[i].transform.position.z +
                    B1(u) * controlPoints[i + 1].transform.position.z +
                    B2(u) * controlPoints[i + 2].transform.position.z +
                    B3(u) * controlPoints[i + 3].transform.position.z;
            }
        }
    }
    float B0(float u)
    {
        return (1f - u) *(1f - u) *(1f - u) /6f;
    }
    float B1(float u)
    {
        return ((3f * u * u * u) - (6f * u * u) + 4f) / 6f;
    }
    float B2(float u)
    {
        return ((-3f * u*u*u) + (3f * u*u) + (3f * u) + 1f)/6f;
    }
    float B3(float u)
    {
        return u * u * u / 6f;
    }
    void moveBeizer()
    {
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
    }
    void linearProgress()
    {
        interpolateAmount = (float)((Time.realtimeSinceStartupAsDouble - startTime) / 5f) % 1f;
    }
    void sineProgress()
    {
        float t = (float)((Time.realtimeSinceStartupAsDouble - startTime) / 5f) % 1f;
        float k1 = 1f / 6f;
        float k2 = 5f / 6f;
        float f, s;
        f = (k1 * 2f / Mathf.PI) + k2 - k1 + ((1.0f - k2) * 2f / Mathf.PI);
        if (t < k1)
        {
            s = k1 * (2f / Mathf.PI) * (Mathf.Sin((t / k1) * Mathf.PI / 2f - Mathf.PI / 2f) + 1f);
        }
        else if (t < k2)
        {
            s = (2f * k1 / Mathf.PI + t - k1);
        }
        else
        {
            s = 2f * k1 / Mathf.PI + k2 - k1 + ((1f - k2) * (2f / Mathf.PI)) * Mathf.Sin(((t - k2) / (1.0f - k2)) * Mathf.PI / 2f);
        }
        interpolateAmount = s / f;
    }
    void parabolicProgress()
    {
        float t = (float)((Time.realtimeSinceStartupAsDouble - startTime) / 5f) % 1f;
        float k1 = 1f / 6f;
        float k2 = 5f / 6f;
        float f, s;
        f = (k1 * 2f / Mathf.PI) + k2 - k1 + ((1.0f - k2) * 2f / Mathf.PI);
        if (t < k1)
        {
            s = k1 * (2f / Mathf.PI) * (Mathf.Sin((t / k1) * Mathf.PI / 2f - Mathf.PI / 2f) + 1f);
        }
        else if (t < k2)
        {
            s = (2f * k1 / Mathf.PI + t - k1);
        }
        else
        {
            s = 2f * k1 / Mathf.PI + k2 - k1 + ((1f - k2) * (2f / Mathf.PI)) * Mathf.Sin(((t - k2) / (1.0f - k2)) * Mathf.PI / 2f);
        }
        interpolateAmount = s / f;

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
    }
    void ease()
    {

    }

}
