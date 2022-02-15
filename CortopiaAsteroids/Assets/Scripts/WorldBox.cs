using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]

public class WorldBox : MonoBehaviour
{
    //Public Properties
    public Rect boundingBox { get; protected set; }

    //Tweakable Variables
    [SerializeField] private float BoundingBoxGrace = 0.5f; 
    //Gives a grace, so the bounding box can be adjusted to be slightly larger than sprite border
    //This makes the loop over look better

    public UnityEvent<Vector3[]> NewCorners;
    
    //Returns new pos
    public Vector3 InsideBoundingBox(Vector3 pos)
    {
        Vector2 newPos = pos;
        if (pos.x < boundingBox.xMin || pos.x > boundingBox.xMax) //Outside x bounds
        {
            newPos.x = pos.x < boundingBox.xMin
                ? GetNewPositionalValue(pos.x, boundingBox.xMin, boundingBox.xMax)
                : GetNewPositionalValue(pos.x, boundingBox.xMax, boundingBox.xMin);
        }
        if (pos.y < boundingBox.yMin || pos.y > boundingBox.yMax) //Outside x bounds
        {
            newPos.y = pos.y < boundingBox.yMin
                ? GetNewPositionalValue(pos.y, boundingBox.yMin, boundingBox.yMax)
                : GetNewPositionalValue(pos.y, boundingBox.yMax, boundingBox.yMin);
        }

        return newPos;
    }

    public Vector3[] GetCorners()
    {
        const int Corners = 4;
        Vector3[] corners = new Vector3[Corners];
        
        corners[0] = new Vector3(boundingBox.xMin, boundingBox.yMin, 0f);
        corners[1] = new Vector3(boundingBox.xMin, boundingBox.yMax, 0f);
        corners[2] = new Vector3(boundingBox.xMax, boundingBox.yMin, 0f);
        corners[3] = new Vector3(boundingBox.xMax, boundingBox.yMax, 0f);

        return corners;
    }
    
    void Start()
    {
        controller = GetComponent<SpriteShapeController>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        //Unity seems to have no events for changed resolution or screen size to subscribe to
        UpdateDimensions();
    }

    protected void UpdateDimensions()
    {
        if (worldResolution.x == mainCamera.pixelWidth && worldResolution.y == mainCamera.pixelHeight)
            return;

        worldResolution = new Vector2Int(mainCamera.pixelWidth, mainCamera.pixelHeight);

        //Setting up spline
        Vector3 bottomLeft = mainCamera.ScreenToWorldPoint(Vector3.zero);
        Vector3 topLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, worldResolution.y, 0)); 
        Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(worldResolution.x, worldResolution.y, 0));
        Vector3 bottomRight = mainCamera.ScreenToWorldPoint(new Vector3(worldResolution.x, 0, 0));
        Vector3 bottomLeft2 = bottomLeft + Vector3.left; //Unity does not want to place points on top of each other
           
        Vector3[] DrawOrder =
        {
            bottomLeft, topLeft, topRight, bottomRight, bottomLeft2
        };
            
        //Spline should always have 5 points in prefab, so no need for automatic system to create the points
        for (int i = 0; i < splinePoints; i++) 
        { 
            controller.spline.SetPosition(i, DrawOrder[i]);
        }
        //Spline visuals are now done
        
        //Bounding Box Calculation
        boundingBox = Rect.MinMaxRect(
            bottomLeft.x - BoundingBoxGrace,
            bottomLeft.y - BoundingBoxGrace,
            topRight.x + BoundingBoxGrace,
            topRight.y + BoundingBoxGrace
            );

        Vector3[] corners = GetCorners();
        NewCorners.Invoke(corners);
    }

    protected void OnValidate()
    {
        controller = GetComponent<SpriteShapeController>();
        mainCamera = GetComponentInParent<Camera>();
        
        if(mainCamera != null) //OnValidate runs several times, and may not find the camera the first time.
            UpdateDimensions();
    }

    protected void OnDrawGizmosSelected()
    {
        //Visualize the Bounding box
        Vector3 boxBL = new Vector3(boundingBox.xMin, boundingBox.yMin);
        Vector3 boxTL = new Vector3(boundingBox.xMin, boundingBox.yMax);
        Vector3 boxTR = new Vector3(boundingBox.xMax, boundingBox.yMax);
        Vector3 boxBR = new Vector3(boundingBox.xMax, boundingBox.yMin);
        
        Vector3[] boundingBoxVertices = 
        {
            boxBL, boxTL, boxTR, boxBR, boxBL //loop back at end
        };

        Gizmos.color = Color.red;
        for (int i = 0; i < boundingBoxVertices.Length-1; i++)
        {
            Gizmos.DrawLine(boundingBoxVertices[i], boundingBoxVertices[i+1]);
        }
        Gizmos.color = Color.white;
    }

    //Storage Varaibles
    protected SpriteShapeController controller;
    protected Vector2Int worldResolution;
    protected Camera mainCamera;
    private const int splinePoints = 5;
    
    //Utility
    protected float GetNewPositionalValue(float current, float comparison, float newBase)
    {
        float difference = current - comparison;
        return newBase - difference;
    }
}
