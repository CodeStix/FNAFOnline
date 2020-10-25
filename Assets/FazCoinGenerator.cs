using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FazCoinGenerator : MonoBehaviour
{
    /*[Help("The coin collection count will be stored in LocalData.")]
    public string dataKey = "coins";
    [Space]*/
    public GameObject coin;
    public float minX = -5f;
    public float maxX = 5f;
    public float minY = -3f;
    public float maxY = 3f;
    [Range(0, 20)]
    public int spawnTries = 5;
    [Range(0f, 1f)]
    public float spawnChance = 0.6f;
    [Space]
    public UnityEvent onCoinRemove;

    private List<GameObject> previousCoins = new List<GameObject>();
    private float time = 0f;
    private float originalChance;
    private bool setOriginalChance = true;

    void Start()
    {
        originalChance = spawnChance;
    }

    void Update()
    {
        if (time > 0f)
            time -= Time.deltaTime;

        if (time <= 0f && !setOriginalChance)
        {
            spawnChance = originalChance;
            setOriginalChance = true;
        }
    }

    public void GenerateNewCoins()
    {
        RemovePreviousCoins();
        GenerateCoins();
    }

    public void GenerateCoins()
    {
        for (int i = 0; i < spawnTries; i++)
            if (spawnChance >= Random.Range(0f, 1f))
                SpawnSingle();
    }

    public void RemovePreviousCoins()
    {
        foreach (GameObject c in previousCoins)
            Destroy(c);

        previousCoins.Clear();
    }

    public void SpawnSingle()
    {
        float x = Random.Range(minX,maxX);
        float y = Random.Range(minY, maxY);

        GameObject obj = Instantiate(coin, new Vector2(x, y), Quaternion.identity, transform);
        previousCoins.Add(obj);
        obj.SetActive(true);
    }

    public void CollectedCoin(GameObject coin)
    {
        Destroy(coin);

        time += 5f;
        setOriginalChance = false;
        spawnChance = Mathf.Lerp(0f, spawnChance, 0.8f);

        if (onCoinRemove != null)
            onCoinRemove.Invoke();
    }
}
