using System.Collections;
using System.Collections.Generic;
using PurrNet;
using UnityEngine;

public class Airdrop : NetworkIdentity
{
    public List<GameObject> objectsToDrop;
    public Renderer platform;
    public float maxX, maxZ, minX, minZ;
    public bool isGameRunning = false;
    public bool isDropRunning = false;
    [Range(5, 1000)]
    public float timeInterval = 5f;
    public float dropHeight = 25f;

    void Start()
    {
        maxX = platform.bounds.max.x;
        maxZ = platform.bounds.max.z;
        minX = platform.bounds.min.x;
        minZ = platform.bounds.min.z;


    }

    protected override void OnSpawned()
    {
        base.OnSpawned();
        if (!isServer) return;
        if (!isGameRunning) return;

        StartCoroutine(DropAction());
    }

    void Update()
    {
        if (!isServer) return;
        if (!isGameRunning) return;
        if (isDropRunning) return;
        

        if (!isDropRunning)
        {
            StartCoroutine(DropAction());
            isDropRunning = true;
        }
    }

    IEnumerator DropAction()
    {
        while (true)
        {
            Debug.Log("Dropping");
            DetermineDrop();
            yield return new WaitForSeconds(timeInterval);
        }

    }
    
    void DetermineDrop()
    {
        for (int i = 0; i < objectsToDrop.Count; i++)
        {
            float x = Random.Range(minX, maxX);
            float z = Random.Range(minZ, maxZ);

            Vector3 spawnPos = new Vector3(x, dropHeight, z);

            GameObject item = Instantiate(objectsToDrop[i], spawnPos, Quaternion.identity);
        }
    }
}
