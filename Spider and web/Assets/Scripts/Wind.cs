using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    //TODO Delete this. Change to 1 point
    [SerializeField] GameObject pointPrefab;
    List<GameObject> drawnPoints = new List<GameObject>();
    GameObject drawnPoint;
    float opacity;

    private float speed = 0f;
    private float amplitude = 0f;
    private float stepFactor = 0f;
    const int accelerationTime = 3;

    private float lowerEdge = 0f;
    private float upperEdge = 0f;
    private float lowerEdgeCoeff = 0f;
    private float upperEdgeCoeff = 0f;

    // Wind power is determined by a value from 0 to 1. 
    // Edges of the wind (y1 and y2) are generated randomly.
    // The slope coefficient of the first inclined line: k = 1 / y1.
    // The slope coefficient of the second inclined line: k = -1 / (y2 - 2).
    // 
    // Power
    //  |    
    // 1|    /¯¯¯¯¯¯¯\
    //  |   /         \
    //  |  /           \
    //  | /             \
    //  |/_______________\___
    //  0     y1    y2   2   y

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateParameters());
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSpeed();
    }

    private IEnumerator GenerateParameters()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3, 15));
            EraseWindIndicator();
            amplitude = UnityEngine.Random.Range(-1f, 1f);
            stepFactor = Mathf.Abs(speed - amplitude) / accelerationTime * Mathf.Sign(amplitude);

            lowerEdge = UnityEngine.Random.Range(0f, 2f);
            upperEdge = UnityEngine.Random.Range(lowerEdge, Mathf.Min(2f, lowerEdge + 0.3f));

            DrawWindIndicator();
        }
    }

    private void EraseWindIndicator()       //TODO: Delete in the final version
    {
        foreach (GameObject tPoint in drawnPoints)
        {
            Destroy(tPoint);
        }
        drawnPoints = new List<GameObject>();
    }

    private void DrawWindIndicator()       //TODO: Delete in the final version
    {
        lowerEdgeCoeff = 1f / lowerEdge;
        upperEdgeCoeff = -1f / (upperEdge - 2f);

        for (float i = 0f; i <= 2f; i += 0.05f)
        {
            if (i >= lowerEdge && i <= upperEdge)
            {
                opacity = 1f;
            }
            else
            {
                if (i < lowerEdge)
                {
                    opacity = lowerEdgeCoeff * i;
                }
                else
                {
                    opacity = -upperEdgeCoeff * (i - 2);
                }
            }
            drawnPoint = Instantiate(pointPrefab, new Vector3(0.1f, i, -4f), Quaternion.identity);
            drawnPoints.Add(drawnPoint);
            drawnPoint.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, opacity);
        }
    }

    private void ChangeSpeed()
    {
        float error = float.Epsilon + Mathf.Abs(stepFactor) * Time.deltaTime;

        // If speed and amplitude are close to zero
        if (Mathf.Abs(speed) < error && Mathf.Abs(amplitude) < error)
        {
            return;
        }

        if (Mathf.Abs(amplitude) > Mathf.Abs(speed))
        {
            speed += Time.deltaTime * stepFactor;
        }
        else
        {
            amplitude = 0f;
            speed -= Time.deltaTime * stepFactor;
        }
    }


    public Vector3 GetSpeed(float y)
    {
        return new Vector3(speed * GetPower(y), 0f, 0f);
    }

    private float GetPower(float y)
    {
        lowerEdgeCoeff = 1f / lowerEdge;
        upperEdgeCoeff = -1f / (upperEdge - 2f);

        if (y >= lowerEdge && y <= upperEdge)
        {
            return 1f;
        }

        if (y < lowerEdge)
        {
            return lowerEdgeCoeff * y;
        }

        return -upperEdgeCoeff * (y - 2);

    }
}
