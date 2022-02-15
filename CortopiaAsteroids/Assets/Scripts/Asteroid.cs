using System;
using UnityEngine;
using UnityEngine.Events;

public class Asteroid : MonoBehaviour
{
    [SerializeField] protected AsteroidSpawner.AsteroidSizeType Size;
    [SerializeField] private int Score = 10;
    [SerializeField] public float Speed = 5f;
    [SerializeField] protected float InvincibilityTime = 1f;
    
    [NonSerialized] public Vector3 TravelDirection;

    public UnityEvent OnDestruction;
    
    void Start()
    {
        worldBox = ManagerGameScript.Singleton.worldBox;
        spawner = ManagerGameScript.Singleton.asteroidSpawner;
        collisionManager = ManagerGameScript.Singleton.collisionManager;
        
        collider = GetComponent<SimpleCircleCollision>();
        collider.RelevantCollisions = 
            CollisionManager.CollisionBitfield.ASTEROID | 
            CollisionManager.CollisionBitfield.BULLET | 
            CollisionManager.CollisionBitfield.PLAYER;
        collider.CollisionOccured.AddListener(OnCollision);
        
        collisionManager.AddToList(collider, CollisionManager.CollisionBitfield.ASTEROID);
    }

    private void Update()
    {
        if (ManagerGameScript.Singleton.GameOver)
            return;
        
        if(pendingDestruction)
            DeleteObject();

        invincibilityTimer = Math.Max(invincibilityTimer - Time.deltaTime, 0);
        
        transform.position += Speed * Time.deltaTime * TravelDirection;
        transform.position = worldBox.InsideBoundingBox(transform.position);
    }

    void OnCollision(SimpleCircleCollision other)
    {
        if (invincibilityTimer > 0)
            return;

        if (other == collider)
            return;

        pendingDestruction = true;
        
        //Break up away from impact location
        Vector3 breakUpDirection = (transform.position - other.transform.position).normalized;
        spawner.BreakUpAsteroid(transform.position, breakUpDirection, Size);
    }

    void DeleteObject()
    {
        collisionManager.RemoveFromList(collider, CollisionManager.CollisionBitfield.ASTEROID);
        ManagerGameScript.Singleton.scoreUIManager.AddScore(Score);
        Destroy(gameObject);
        OnDestruction.Invoke();
    }

    public void StartInvincbility()
    {
        invincibilityTimer = InvincibilityTime;
    }

    protected float invincibilityTimer = 0;
    protected AsteroidSpawner spawner;
    protected WorldBox worldBox;
    protected SimpleCircleCollision collider;
    protected CollisionManager collisionManager;
    protected bool pendingDestruction = false;
}
