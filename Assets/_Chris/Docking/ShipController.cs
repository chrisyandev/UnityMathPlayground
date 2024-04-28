using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float speed = 1.0f;
    public float rayLength = 10.0f;

    private Transform entryPoint;
    private Transform parkingPoint;
    private Transform leftSensor;
    private Transform rightSensor;
    private Transform frontSensor;
    private Transform backSensor;
    private Vector3 targetPosition;
    private Vector3 targetDirection;

    private bool didLeftHit;
    private bool didRightHit;
    private bool didFrontHit;
    private bool didBackHit;
    private RaycastHit leftHit;
    private RaycastHit rightHit;
    private RaycastHit frontHit;
    private RaycastHit backHit;
    private bool isParking = false;

    private void Start()
    {
        entryPoint = GameObject.Find("EntryPoint").transform;
        parkingPoint = GameObject.Find("ParkingPoint").transform;
        leftSensor = transform.Find("LeftSensor");
        rightSensor = transform.Find("RightSensor");
        frontSensor = transform.Find("FrontSensor");
        backSensor = transform.Find("BackSensor");

        targetPosition = entryPoint.transform.position;
        targetDirection = (targetPosition - transform.position).normalized;
    }

    private void Update()
    {
        didLeftHit = Physics.Raycast(leftSensor.position, -transform.right, out leftHit, rayLength);
        didRightHit = Physics.Raycast(rightSensor.position, transform.right, out rightHit, rayLength);
        didFrontHit = Physics.Raycast(frontSensor.position, transform.forward, out frontHit, rayLength);
        didBackHit = Physics.Raycast(backSensor.position, -transform.forward, out backHit, rayLength);

        if (!isParking && transform.position == entryPoint.position)
        {
            isParking = true;
        }

        CalculateTarget();

        Move();

        DebugRaycasts();
    }

    private void CalculateTarget()
    {
        if (!isParking)
        {
            if (didLeftHit || didRightHit)
            {
                targetPosition = transform.position + Vector3.Project(entryPoint.position - transform.position, transform.forward);
            }
            else if (didFrontHit || didBackHit)
            {
                targetPosition = transform.position + Vector3.Project(entryPoint.position - transform.position, transform.right);
            }
            else
            {
                targetPosition = entryPoint.position;
            }

            if (transform.position == targetPosition)
            {
                targetPosition = entryPoint.position;
            }
        }
        else
        {
            targetPosition = parkingPoint.position;
        }

        targetDirection = (targetPosition - transform.position).normalized;
    }

    private void Move()
    {
        var step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }

    private void DebugRaycasts()
    {
        Debug.DrawRay(transform.position, targetDirection * 100.0f, Color.white, Time.deltaTime);

        if (didLeftHit)
        {
            Debug.DrawLine(leftSensor.position, leftHit.point, Color.cyan, Time.deltaTime);
        }
        else
        {
            Debug.DrawLine(leftSensor.position, leftSensor.position + (-transform.right * rayLength), Color.red, Time.deltaTime);
        }

        if (didRightHit)
        {
            Debug.DrawLine(rightSensor.position, rightHit.point, Color.magenta, Time.deltaTime);
        }
        else
        {
            Debug.DrawLine(rightSensor.position, rightSensor.position + (transform.right * rayLength), Color.red, Time.deltaTime);
        }
        
        if (didFrontHit)
        {
            Debug.DrawLine(frontSensor.position, frontHit.point, Color.green, Time.deltaTime);
        }
        else
        {
            Debug.DrawLine(frontSensor.position, frontSensor.position + (transform.forward * rayLength), Color.red, Time.deltaTime);
        }

        if (didBackHit)
        {
            Debug.DrawLine(backSensor.position, backHit.point, Color.yellow, Time.deltaTime);
        }
        else
        {
            Debug.DrawLine(backSensor.position, backSensor.position + (-transform.forward * rayLength), Color.red, Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(targetPosition, 5.0f);
        }
    }
}
