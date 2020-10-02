using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Spider _spiderPrefab;

    private void Start()
    {
        InstantiateSpider();
    }

    public void InstantiateSpider()
    {
        Instantiate(_spiderPrefab, new Vector3(1.8f, 2.1f, -3f), Quaternion.identity);
    }
}
