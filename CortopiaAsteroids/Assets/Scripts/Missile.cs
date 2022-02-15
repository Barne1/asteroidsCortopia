using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Missile : MonoBehaviour
{
    [SerializeField] private float Speed = 10f;
    [SerializeField] private float MaxTravelDistance = 10f;

    public UnityEvent OnDestruction;
    
    private void Start()
    {
        worldBox = ManagerGameScript.Singleton.worldBox;
        collisionManager = ManagerGameScript.Singleton.collisionManager;
        
        collider = GetComponent<SimpleCircleCollision>();
        collider.RelevantCollisions = CollisionManager.CollisionBitfield.ASTEROID;
        collider.CollisionOccured.AddListener(OnCollision);
        
        collisionManager.AddToList(collider, CollisionManager.CollisionBitfield.BULLET);
    }

    private void Update()
    {
        if (ManagerGameScript.Singleton.GameOver)
            return;
        
        if(pendingDestruction || distanceTraveled > MaxTravelDistance )
        {
            DeleteBullet();
            return;
        }

        Vector3 oldPos = transform.position;
        transform.position += Time.deltaTime * Speed * transform.up;
        distanceTraveled += (oldPos - transform.position).magnitude;
        transform.position = worldBox.InsideBoundingBox(transform.position);
    }

    //With more time, an object pool might be a better fit
    void OnCollision(SimpleCircleCollision other)
    {
        pendingDestruction = true;
    }

    void DeleteBullet()
    {
        collisionManager.RemoveFromList(collider, CollisionManager.CollisionBitfield.BULLET);
        Destroy(gameObject);
    }

    protected WorldBox worldBox;
    protected CollisionManager collisionManager;
    private SimpleCircleCollision collider;
    private float distanceTraveled = 0;
    private bool pendingDestruction = false; //makes sure all collision is performed before destruction
}
