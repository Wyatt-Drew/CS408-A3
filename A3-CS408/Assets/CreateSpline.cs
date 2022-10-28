using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreateSpline : MonoBehaviour
{
    [SerializeField] private Transform sphere;

    private float progress = 0f;
    //private float speed = 1f;
    private int size = 11;
    GameObject[] controlPoints = new GameObject[11];
    public GameObject tempObject;
    private bool drawLine = true;
    private bool isLoaded = false;
    private double startTime;
    const int linear = 1;
    const int sine = 2;
    const int parabolic = 3;
    private int moveMode = 1;
    public List<Transform> points = new List<Transform>();
    public const int pointNum = 50;
    public const int maxSteps = 100;
    Vector3[] splineCurve;
    public TextMeshPro easeStyle;
    void Start()
    {
        Application.targetFrameRate = 60; // lock framerate to 60
        spawnControlPoints();
        //Spawn in title and instructions
        GameObject title = Instantiate(Resources.Load("TitleAndInstructions", typeof(GameObject))) as GameObject;
        title.transform.position = new Vector3(0f, 0f, 0f);
        GameObject title2 = Instantiate(Resources.Load("EaseTitle", typeof(GameObject))) as GameObject;
        title2.transform.position = new Vector3(20.75f, 24.7f, 0f);
        easeStyle = title2.GetComponent<TextMeshPro>();
        calculateSpline();
        if (drawLine) // Draws a custom line along whatever trajectory the object will follow
        {
            for (int i = 0; i <= (size - 3) * maxSteps; i++)
            {
                sphere.position = splineCurve[i];
                recordPosition();
            }
            drawLine = false;
            Transform[] points2 = new Transform[pointNum];
            points2 = points.ToArray();
            FindObjectOfType<lineController>().drawLine(points2);
        }else // Spawns a prefab line to save computation power
        {
            GameObject instance = Instantiate(Resources.Load("LineRend", typeof(GameObject))) as GameObject;
            instance.transform.position = GameObject.Find("/LineRend").transform.position;
        }
    }
    //Purpose: To spawn in the control points.  
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
    //Purpose: Create a new object at each position of the sphere and store its transform in a list.
    // Note: In Unity transforms must be associated with an object.  That is why it is done this way.
    void recordPosition()
    {
        GameObject current = new GameObject();
        current.transform.position = sphere.position;
        points.Add(current.transform);
    }
    //Purpose: Calculate the entire path of the spline and store it for later use
    void calculateSpline()
    {
        splineCurve = new Vector3[maxSteps * (size - 3) + 1];
        for (int i = 0; i < controlPoints.Length - 3; i++)
        {
            for (int j = 0; j <= maxSteps; j++)
            {
                float u = (float)j / (float)maxSteps;
                splineCurve[(i * maxSteps) + j].x =
                    B0(u) * controlPoints[i].transform.position.x +
                    B1(u) * controlPoints[i + 1].transform.position.x +
                    B2(u) * controlPoints[i + 2].transform.position.x +
                    B3(u) * controlPoints[i + 3].transform.position.x;
                splineCurve[(i * maxSteps) + j].y =
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
        return (1f - u) * (1f - u) * (1f - u) / 6f;
    }
    float B1(float u)
    {
        return ((3f * u * u * u) - (6f * u * u) + 4f) / 6f;
    }
    float B2(float u)
    {
        return ((-3f * u * u * u) + (3f * u * u) + (3f * u) + 1f) / 6f;
    }
    float B3(float u)
    {
        return u * u * u / 6f;
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
                        easeStyle.text = "Linear Movement";
                        break;
                    }
                case 'c'://sine mode
                    {
                        moveMode = sine;
                        easeStyle.text = "Sinusoidal Ease-in / Ease-out";
                        break;
                    }
                case 'd'://parabolic mode
                    {
                        moveMode = parabolic;
                        easeStyle.text = "Parabolic Ease-in / Ease-out";
                        break;
                    }
            }
        }
    }
    //Purpose:  Convert progress (float) to an integer which represents how far through the animation we are.
    //Progress has a range from 0 to 1.  This int has a range of each value calculated in calculateSpline
    int getFrame()
    {
        return (int)(progress * (maxSteps * ((float)(size - 3))));
    }
    //Purpose: Move the sphere depending on progress
    void move()
    {
        sphere.position = splineCurve[getFrame()];
    }

    //All progress functions
    //Purpose: Changes progress value to represent how far along the path we should be.
    void linearProgress()
    {
        progress = (float)((Time.realtimeSinceStartupAsDouble - startTime) / 5f) % 1f;
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
        progress = s / f;
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
        progress = s / f;

    }

}
