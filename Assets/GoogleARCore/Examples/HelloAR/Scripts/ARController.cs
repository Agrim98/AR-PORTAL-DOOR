using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

/// <summary>
/// Manages the visualization of detected planes in the scene.
/// </summary>
public class ARController : MonoBehaviour
{
    public GameObject Portal;
    private List<DetectedPlane> m_NewPlanes = new List<DetectedPlane>();
    public GameObject GridPrefab;
    public GameObject ARCamera;

    /// <summary>
    /// The Unity Update method.
    /// </summary>
    public void Update()
    {

        // Check that motion tracking is tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        // Iterate over planes found in this frame and instantiate corresponding GameObjects to
        // visualize them.
        Session.GetTrackables<DetectedPlane>(m_NewPlanes, TrackableQueryFilter.New);
        for (int i = 0; i < m_NewPlanes.Count; i++)
        {
            // Instantiate a plane visualization prefab and set it to track the new plane. The
            // transform is set to the origin with an identity rotation since the mesh for our
            // prefab is updated in Unity World coordinates.
            GameObject grid = Instantiate(GridPrefab, Vector3.zero, Quaternion.identity, transform);
            grid.GetComponent<GridVisualizer>().Initialize(m_NewPlanes[i]);
        }
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }
        TrackableHit hit;
        if (Frame.Raycast(touch.position.x, touch.position.y, TrackableHitFlags.PlaneWithinPolygon, out hit))
        {
            Portal.SetActive(true);
            _ = hit.Trackable.CreateAnchor(hit.Pose);
            Portal.transform.position = hit.Pose.position;
            Portal.transform.rotation = hit.Pose.rotation;

            Vector3 cameraPosition = ARCamera.transform.position;
            cameraPosition.y = hit.Pose.position.y;

            Portal.transform.LookAt(cameraPosition, Portal.transform.up);
        }
    }
}

