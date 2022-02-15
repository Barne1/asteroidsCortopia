using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidSpawner : MonoBehaviour
{
    private const int AsteroidsFromDestruction = 2;
    [SerializeField] GameObject[] AsteroidPrefabs = new GameObject[(int)AsteroidSizeType.MAX];
    [SerializeField] float MinAngleDestruction = -90f;
    [SerializeField] float MaxAngleDestruction = 90f;
    [SerializeField] float MinSmallSpeed = 1f;
    [SerializeField] float MaxSmallSpeed = 8f;
    [SerializeField] float MinTimeBetweenSpawns = 1f;
    [SerializeField] float MaxTimeBetweenSpawns = 5f;

    private void Start()
    {
        WorldBox worldBox = FindObjectOfType<WorldBox>();
        spawnCorners = worldBox.GetCorners();
        worldBox.NewCorners.AddListener(UpdateCorners);
    }

    private void Update()
    {
        if (ManagerGameScript.Singleton.GameOver)
            return;
        
        spawnTimer -= Time.deltaTime;
        if (spawnTimer < 0)
        {
            int AsteroidType = Random.Range((int) AsteroidSizeType.Medium, (int) AsteroidSizeType.MAX); //MAX is exclusive
            float travelAngle = Random.Range(0f, 360f);
            
            Vector3 TravelDirection = (Quaternion.AngleAxis(travelAngle, Vector3.forward) * Vector3.up).normalized;

            int spawnCorner = Random.Range(0, 4);

            GameObject asteroidObject = Instantiate(AsteroidPrefabs[AsteroidType], spawnCorners[spawnCorner], Quaternion.identity);
            Asteroid asteroid = asteroidObject.GetComponent<Asteroid>();
            asteroid.TravelDirection = TravelDirection;
            asteroid.StartInvincbility();

            spawnTimer = Random.Range(MinTimeBetweenSpawns, MaxTimeBetweenSpawns);
        }
    }

    public enum AsteroidSizeType
    {
        Small = 0,
        Medium,
        Large,
        MAX
    }

    public void BreakUpAsteroid(Vector3 pos, Vector3 direction, AsteroidSizeType sizeType)
    {
        if (sizeType == AsteroidSizeType.Small)
            return;

        for (int i = 0; i < AsteroidsFromDestruction; i++)
        {
            float MinAngle = Mathf.Lerp(MinAngleDestruction, MaxAngleDestruction, (float)i / (float)AsteroidsFromDestruction);
            float MaxAngle = Mathf.Lerp(MinAngleDestruction, MaxAngleDestruction, (float)i+1 / (float)AsteroidsFromDestruction);
            float Angle = Random.Range(MinAngle, MaxAngle);

            Vector3 TravelDirection = (Quaternion.AngleAxis(Angle, Vector3.forward) * direction).normalized;

            GameObject asteroidObject = Instantiate(AsteroidPrefabs[(int) sizeType - 1], pos, Quaternion.identity);
            Asteroid asteroid = asteroidObject.GetComponent<Asteroid>();
            asteroid.TravelDirection = TravelDirection;
            if (sizeType - 1 == (int) AsteroidSizeType.Small)
            {
                asteroid.Speed = Random.Range(MinSmallSpeed, MaxSmallSpeed);
            }
            asteroid.StartInvincbility();
        }
    }

    public void UpdateCorners(Vector3[] corners)
    {
        spawnCorners = corners;
    }

    private float spawnTimer = 0;
    private Vector3[] spawnCorners;
}
