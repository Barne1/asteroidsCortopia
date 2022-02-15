using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleCircleCollision : MonoBehaviour
{
    public float Radius; //Cannot use serializefield and properties at the same time so public makes sense
    public UnityEvent<SimpleCircleCollision> CollisionOccured;
    [NonSerialized] public CollisionManager.CollisionBitfield RelevantCollisions;

    private void Start()
    {
        collisionManager = ManagerGameScript.Singleton.collisionManager;
    }

    protected bool IsColliding(Vector3 otherPos, float otherRadius)
    {
        float posDistanceSqr = (otherPos - transform.position).sqrMagnitude;
        float sqrRadiuses = Radius + otherRadius;
        sqrRadiuses *= sqrRadiuses;
        
        return (posDistanceSqr - sqrRadiuses) <= 0;
    }


    protected void LateUpdate() //Collision Events fire after Update so all happens at once
    {
        if (ManagerGameScript.Singleton.GameOver)
            return;
        
        List<SimpleCircleCollision> candidates = collisionManager.GetColliders(RelevantCollisions);
        if (candidates.Count < 1)
            return;
        
        foreach (var candidate in candidates)
        {
            if (IsColliding(candidate.transform.position, candidate.Radius))
            {
                CollisionOccured.Invoke(candidate);
            }
        }
    }

    //Gizmos
    protected void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }

    protected CollisionManager collisionManager;
}
