using System.Collections;
using UnityEngine;

public class HeartSpawner : MonoBehaviour
{
    [SerializeField] HeartTarget heartPrefab;
    [SerializeField] GameController gameController;

    [SerializeField] float spawnInterval = 1f;
    [SerializeField] float minX = -7f;
    [SerializeField] float maxX = 7f;
    [SerializeField] float spawnY = 5f;

    Coroutine spawnRoutine;

    public void StartSpawning()
    {
        StopSpawning();
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnHeart();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnHeart()
    {
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

        HeartTarget heart = Instantiate(
            heartPrefab,
            spawnPosition,
            Quaternion.identity,
            transform
        );

        heart.Initialize(gameController);
    }
}