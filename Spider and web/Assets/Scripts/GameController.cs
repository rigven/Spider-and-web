using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] Spider spiderPrefab;

    public void InstantiateSpider()
    {
        Instantiate(spiderPrefab, new Vector3(1.8f, 2.1f, -3f), Quaternion.identity);
    }
}
