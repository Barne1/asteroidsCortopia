using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SimpleCircleCollision))]

public class Player : MonoBehaviour
{
    //Tweakable Variables
    [SerializeField, Range(0, 1.0f)] private float Friction = 0.5f;
    [SerializeField] private float RotationSpeed = 100f;
    [SerializeField] private float ThrustAcceleration = 50.0f;
    [SerializeField] private short StartingLives = 3;
    [SerializeField] private float TimeBetweenShots = 0.3f;
    [SerializeField] private GameObject BulletObject;
    [SerializeField] private float ShootingOffset = 2;
    [SerializeField] private float InvincibilityTime = 1;
    
    //Events
    public UnityEvent<short> OnLostLife; //Broadcasts lives left
    public UnityEvent OnDeath;
    
    //Accessible Properties
    public Vector3 Velocity {get; protected set; }

    //Functions
    void Start()
    {
        worldBox = ManagerGameScript.Singleton.worldBox;
        collisionManager = ManagerGameScript.Singleton.collisionManager;
        
        collisionComponent = GetComponent<SimpleCircleCollision>();
        collisionComponent.RelevantCollisions = CollisionManager.CollisionBitfield.ASTEROID;
        collisionComponent.CollisionOccured.AddListener(OnCollision);
        
        collisionManager.AddToList(collisionComponent, CollisionManager.CollisionBitfield.PLAYER);

        currentLives = StartingLives;
    }

    void Update()
    {
        if (ManagerGameScript.Singleton.GameOver)
            return;
        
        shotTimer = Math.Max(shotTimer - Time.deltaTime, -1);
        invincibilityTimer = Math.Max(invincibilityTimer - Time.deltaTime, -1);
        
        HandleRotation();
        HandleMovement();
        HandleShoot();

        Velocity *= Mathf.Pow(1 - Friction, Time.deltaTime);
        
        transform.position += Velocity * Time.deltaTime;

        transform.position = worldBox.InsideBoundingBox(transform.position);
    }

    //Movement Functions
    void HandleRotation()
    {
        float frameRotation = inputVector.x * RotationSpeed * Time.deltaTime;
        transform.Rotate(RotationAxis, frameRotation);
    }
    void HandleMovement()
    {
        if (inputVector.y <= 0)
            return;
        
        float FrameThustPower = Mathf.Max(inputVector.y, 0) * Time.deltaTime * ThrustAcceleration;
        Vector2 FrameThrust = transform.up * FrameThustPower;
        Velocity += (Vector3)FrameThrust;
    }
    public void HandleShoot()
    {
        if (bWantsToShoot && shotTimer < 0.000001) //Arbitrary small number
        {
            shotTimer = TimeBetweenShots;
            
            Vector3 shootingPosition = transform.position + (transform.up * ShootingOffset);
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, transform.up);
            
            Instantiate(BulletObject, shootingPosition, lookRotation);
            
            Missile missile = BulletObject.GetComponent<Missile>();
        }
    }
    
    //Input Event functions
    public void OnMovementInput(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }
    public void OnShootInput(InputAction.CallbackContext context)
    {
        bWantsToShoot = context.ReadValueAsButton();
    }

    public void OnCollision(SimpleCircleCollision other)
    {
        if (invincibilityTimer > 0)
            return;
        
        --currentLives;
        OnLostLife.Invoke(currentLives);
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        Velocity = Vector3.zero;
        StartInvincibility();
        
        if (currentLives < 1)
        {
            OnDeath.Invoke();
            this.gameObject.SetActive(false);
            collisionManager.RemoveFromList(collisionComponent, CollisionManager.CollisionBitfield.PLAYER);
        }
    }

    void StartInvincibility()
    {
        invincibilityTimer = InvincibilityTime;
    }
    
    //Storage Variables
    protected Vector2 inputVector = Vector2.zero;
    protected bool bWantsToShoot = false;
    protected readonly Vector3 RotationAxis = new Vector3(0f, 0f, -1.0f);
    protected WorldBox worldBox;
    protected CollisionManager collisionManager;

    private SimpleCircleCollision collisionComponent;
    
    protected short currentLives;

    private float shotTimer = 0;
    public float invincibilityTimer = 0;
}
