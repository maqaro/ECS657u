using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class LightBeam : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask layerMask; 
    public float defaultLength = 50;
    public int numOfReflections = 2;

    private LineRenderer _lineRenderer;
    private Camera _myCam;
    private RaycastHit hit;

    private Ray ray;
    private Vector3 direction;

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

        for (int i = 0; i < numOfReflections; i++) 
        {
            // Set the start position of the laser to the object's current position
            _lineRenderer.SetPosition(0, transform.position);

            // Cast a ray forward to detect objects
            if (Physics.Raycast(ray.origin, ray.direction, out hit, remainLength, layerMask))
            {
                _lineRenderer.positionCount += 1;
                _lineRenderer.SetPosition(1, hit.point); // End at the hit point

                remainLength -= Vector3.Distance(ray.origin, hit.point);

                ray = new Ray(hit.point, Vector3.Reflect (ray.direction, hit.normal) );
            }
            else
            {
                _lineRenderer.positionCount += 1;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, ray.origin + (ray.direction * remainLength));
            }
        }

    }

    void NormalLaser()
    {
        // Set the start position of the laser to the object's current position
        _lineRenderer.SetPosition(0, transform.position);

        // Cast a ray forward to detect objects
        if (Physics.Raycast(transform.position, transform.forward, out hit, defaultLength, layerMask))
        {
            _lineRenderer.SetPosition(1, hit.point); // End at the hit point
        }
        else
        {
            _lineRenderer.SetPosition(1, transform.position + (transform.forward * defaultLength)); // End at max length
        }
    }

}
