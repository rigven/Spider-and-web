using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderSpawner : MonoBehaviour
{
    // Config params
    [SerializeField] private Spider _spiderPrefab;
    private float _minRequiredProtrusion = 0.2f;
    private float _offsetFromLine = 0.5f;

    // Event
    public delegate void SpiderCreatedEventHandler();
    public event SpiderCreatedEventHandler SpiderCreated;

    public void InstantiateSpider(List<Vector2> rightLineCoords, List<Vector2> leftLineCoords)
    {
        Instantiate(_spiderPrefab, SelectSpawnPoint(rightLineCoords, leftLineCoords), Quaternion.identity);
        SpiderCreated();
    }

    private Vector3 SelectSpawnPoint(List<Vector2> rightLineCoords, List<Vector2> leftLineCoords)
    {
        float x = FindLedge(rightLineCoords, -1);
        if (x == 0f)
        {
            x = FindLedge(leftLineCoords, 1);
            if (x == 0f)
            {
                x = rightLineCoords[0].x - _offsetFromLine;
            }
        }

        return new Vector3(x, 2.1f, -3f);
    }

    /// <summary>Finds the center of the first ledge in the line.</summary>
    /// <param name="lineCoords">Coordinates of the line.</param>
    /// <param name="direction">In which direction relative to the first point the protrusion will be searched. 1 - right, -1 - left.</param>
    /// <returns>Returns the x-coordinate of the ledge.</returns>
    private float FindLedge(List<Vector2> lineCoords, int direction)
    {
        float referencePoint = lineCoords[0].x + _minRequiredProtrusion * direction;

        for (int i = 1; i < lineCoords.Count; i++)
        {
            if (lineCoords[i].x * direction > referencePoint * direction)
            {
                return (lineCoords[i].x + referencePoint) / 2f;
            }
        }

        return 0f;
    }
}
