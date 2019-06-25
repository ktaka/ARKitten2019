using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObject : MonoBehaviour
{
    public GameObject placedPrefab;
    public bool useAR = true;
    public GameObject floorPlane;

    GameObject spawnedObject;
    ARRaycastManager raycastManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Start is called before the first frame update
    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        if (floorPlane != null)
        {
            floorPlane.SetActive(!useAR);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        if (HitTest(touchPosition, out Pose hitPose))
        {
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
            }
        }
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;
            touchPosition = new Vector2(mousePosition.x, mousePosition.y);
            return true;
        }
#else
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
#endif

        touchPosition = default;
        return false;
    }

    bool HitTest(Vector2 touchPosition, out Pose hitPose)
    {
        if (useAR)
        {
            if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                hitPose = hits[0].pose;
                return true;
            }
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                hitPose = new Pose();
                hitPose.position = hit.point;
                hitPose.rotation = hit.transform.rotation;
                Debug.DrawRay(ray.origin, ray.direction, Color.red, 3);
                return true;
            }
        }
        hitPose = default;
        return false;
    }
}
