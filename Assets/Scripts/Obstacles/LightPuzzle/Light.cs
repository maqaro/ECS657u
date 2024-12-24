using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LightBeam : MonoBehaviour
{
    [Header("Settings")]
    public float defaultLength = 50;
    public int numOfReflections = 4;

    private LineRenderer _lineRenderer;
    private RaycastHit hit;

    private Ray ray;

    [Header("Pressure Plate Detection")]
    public LightPressurePlate pressurePlateScript; // Reference to the LightPressurePlate script

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        // Automatically find the LightPressurePlate script if it is not assigned in the Inspector
        if (pressurePlateScript == null)
        {
            GameObject pressurePlate = GameObject.FindGameObjectWithTag("PressurePlate");
            if (pressurePlate != null)
            {
                pressurePlateScript = pressurePlate.GetComponent<LightPressurePlate>();
            }
        }
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

        for (int i = 0; i < numOfReflections; i++)
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, remainLength))
            {
                _lineRenderer.positionCount += 1;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, hit.point);

                remainLength -= Vector3.Distance(ray.origin, hit.point);
                ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));

                if (hit.collider.CompareTag("PressurePlate") && pressurePlateScript != null)
                {
                    pressurePlateScript.CheckLightBeam(); // Notify the LightPressurePlate
                }
            }
            else
            {
                _lineRenderer.positionCount += 1;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, ray.origin + (ray.direction * remainLength));
                break;
            }
        }
    }
}
