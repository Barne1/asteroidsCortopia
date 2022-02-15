using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    [Flags]
    public enum CollisionBitfield : short //We use bitfields since we might want an object in multiple collision channels in the future
    {
        PLAYER = 1,    //0001
        ASTEROID = 2,  //0010
        BULLET = 4     //0100
    }

    private List<SimpleCircleCollision> Players = new List<SimpleCircleCollision>();
    private List<SimpleCircleCollision> Bullets = new List<SimpleCircleCollision>();
    private List<SimpleCircleCollision> Asteroids = new List<SimpleCircleCollision>();

    //We use bitfields since we might want an object in multiple collision channels in the future
    public void AddToList(SimpleCircleCollision collider, CollisionBitfield category)
    {
        if ((category & CollisionBitfield.PLAYER) > 0)
            Players.Add(collider);
        if((category & CollisionBitfield.BULLET) > 0)
            Bullets.Add(collider);
        if((category & CollisionBitfield.ASTEROID) > 0)
            Asteroids.Add(collider);
    }
    
    //We use bitfields since we might want an object in multiple collision channels in the future
    public void RemoveFromList(SimpleCircleCollision collider, CollisionBitfield category)
    {
        if ((category & CollisionBitfield.PLAYER) > 0)
            Players.Remove(collider);
        if((category & CollisionBitfield.BULLET) > 0)
            Bullets.Remove(collider);
        if((category & CollisionBitfield.ASTEROID) > 0)
            Asteroids.Remove(collider);
    }

    public List<SimpleCircleCollision> GetColliders(CollisionBitfield requestedColliders)
    {
        List<SimpleCircleCollision> colliders = new List<SimpleCircleCollision>();
        if((requestedColliders & CollisionBitfield.PLAYER) > 0)
            colliders.AddRange(Players);
        if((requestedColliders & CollisionBitfield.BULLET) > 0)
            colliders.AddRange(Bullets);
        if((requestedColliders & CollisionBitfield.ASTEROID) > 0)
            colliders.AddRange(Asteroids);

        return colliders;
    }
}
