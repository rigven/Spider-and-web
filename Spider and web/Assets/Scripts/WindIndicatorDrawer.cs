using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Delete in the final version
public class WindIndicatorDrawer : MonoBehaviour
{
    // State
    private static List<GameObject> _drawnPoints = new List<GameObject>();
    private static GameObject _drawnPoint;
    private static float _opacity;

    public static void EraseWindIndicator()
    {
        foreach (GameObject tPoint in _drawnPoints)
        {
            Destroy(tPoint);
        }
        _drawnPoints = new List<GameObject>();
    }

    public static void DrawWindIndicator(float lowerEdge, float upperEdge, float lowerEdgeCoeff, float upperEdgeCoeff, GameObject pointPrefab)
    {
        lowerEdgeCoeff = 1f / lowerEdge;
        upperEdgeCoeff = -1f / (upperEdge - 2f);

        for (float i = 0f; i <= 2f; i += 0.05f)
        {
            if (i >= lowerEdge && i <= upperEdge)
            {
                _opacity = 1f;
            }
            else
            {
                if (i < lowerEdge)
                {
                    _opacity = lowerEdgeCoeff * i;
                }
                else
                {
                    _opacity = -upperEdgeCoeff * (i - 2);
                }
            }
            _drawnPoint = Instantiate(pointPrefab, new Vector3(0.1f, i, -4f), Quaternion.identity);
            _drawnPoints.Add(_drawnPoint);
            _drawnPoint.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, _opacity);
        }
    }
}
