using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LightBeam : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask layermask;
    public float defaultLength = 50;
    public int numOfReflections = 4;

    private LineRenderer _lineRenderer;
    private Camera _myCam;
    private RaycastHit hit;

    private Ray ray;

    [Header("Pressure Plate Detection")]
    public LightPressurePlate pressurePlateScript; // Reference to the PressurePlate script

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _myCam = Camera.main;
    }

    private void Update()
    {
        ReflectLaser();
    }

    void ReflectLaser()
    {
        ray = new Ray(transform.position, transform.forward);

        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0, transform.position);

        float remainLength = defaultLength;
        Vector3 currentPosition = transform.position;

        for (int i = 0; i < numOfReflections; i++)
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, remainLength, layermask))
            {
                _lineRenderer.positionCount += 1;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, hit.point);

                remainLength -= Vector3.Distance(ray.origin, hit.point);
                ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));

                currentPosition = hit.point;

                // Check if the light beam hits the Pressure Plate
                if (hit.collider.CompareTag("PressurePlate") && pressurePlateScript != null)
                {
                    pressurePlateScript.CheckLightBeam(); // Notify the pressure plate
                }
            }
            else
            {
                _lineRenderer.positionCount += 1;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, ray.origin + (ray.direction * remainLength));
            }
        }
    }
}
