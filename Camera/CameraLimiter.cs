using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraLimiter : MonoBehaviour
{
    public Collider Player;
    public float CameraSpeed = 2.0f;

	public static CameraLimiter Current;

    [System.NonSerialized]
    public bool ForceCameraReset = false;

    private Rect bestCamera;

	string lastCameraName;
	string cameraName;

    private Vector3 lastPlayerPosition;

    private Rect cameraLocation;
    private Rect newCameraLocation;

    private float bestCameraOverlap = 0;
    private float tmpArea = 0;
    private Vector2 cameraMove;

    private Camera cam;

	private Vector3 maxViewportX = new Vector3(1, 0, 0);

    void Awake()
    {
		Current = this;

        cam = GetComponent<Camera>();

        float height = cam.orthographicSize;
        float width = cam.ViewportToWorldPoint(maxViewportX).x - cam.ViewportToWorldPoint(Vector3.zero).x;

        cameraLocation = new Rect(
                Player.transform.position.x - (width / 2),
                Player.transform.position.y - (height / 2),
                width,
                height
            );

        newCameraLocation = cameraLocation;
    }

    /// <summary>
    /// Calculate the area of the rectangle formed when two rectangles intersect.
    /// </summary>
    /// <param name="r1">The first rectangle</param>
    /// <param name="r2">The second rectangle</param>
    /// <returns>Intersection area (will be 0 if no intersection)</returns>
    float IntersectionArea(Rect r1, Rect r2)
    {
        // No intersection
        if (r1.yMin > r2.yMax || r1.xMin > r2.xMax || r1.yMax < r2.yMin || r1.xMax < r2.xMin)
        {
            return 0;
        }

        return (
                   (r1.xMax < r2.xMax ? r1.xMax : r2.xMax) -
                   (r1.xMin > r2.xMin ? r1.xMin : r2.xMin)
               ) * (
                       (r1.yMax < r2.yMax ? r1.yMax : r2.yMax) -
                       (r1.yMin > r2.yMin ? r1.yMin : r2.yMin)
                   );
    }

    bool ContainsRectangle(Rect container, Rect contained)
    {
        return (contained.xMin >= container.xMin &&
                contained.yMin >= container.yMin &&
                contained.xMax <= container.xMax &&
                contained.yMax <= container.yMax);
    }

	private Object[] limiterBoxes = null;
	private Vector3 tmpPos;
    void Update()
    {
        // See if we need to update
        if (Player.transform.position == lastPlayerPosition)
        {
            return;
        }
        lastPlayerPosition = Player.transform.position;

        cameraLocation.height = 2 * cam.orthographicSize;
        //cameraLocation.width = cam.orthographicSize * cam.aspect * 2;
        cameraLocation.width = cam.ViewportToWorldPoint(maxViewportX).x - cam.ViewportToWorldPoint(Vector3.zero).x;

        // Re-center camera on player
		tmpPos = transform.position;
		tmpPos.x = Player.transform.position.x;
		tmpPos.y = Player.transform.position.y;
		transform.position = tmpPos;

        // Update desired camera position
        newCameraLocation = cameraLocation;
		newCameraLocation.x = Player.transform.position.x - (newCameraLocation.width / 2.0f);
		newCameraLocation.y = Player.transform.position.y - (newCameraLocation.height / 2.0f);

        // Determine which zone is the best fit
        bestCameraOverlap = 0;
		if (limiterBoxes == null)
		{
			limiterBoxes = FindObjectsOfType(typeof(CameraLimiterBox));
		}
        foreach (CameraLimiterBox box in limiterBoxes)
        {
            tmpArea = IntersectionArea(newCameraLocation, box.Bounds);
            if (tmpArea > bestCameraOverlap)
            {
				bestCamera = box.Bounds;
                bestCameraOverlap = tmpArea;
				cameraName = box.name;
            }
        }
		if (cameraName != lastCameraName)
		{
			Debug.Log (string.Format("Switching to {0}", cameraName));
			lastCameraName = cameraName;
		}

        // Fit the desired location into the selected zone
        if (!ContainsRectangle(bestCamera, newCameraLocation))
        {
            if (newCameraLocation.width > bestCamera.width)
                newCameraLocation.x = bestCamera.center.x - (newCameraLocation.width / 2.0f);
            else if (newCameraLocation.xMin < bestCamera.xMin)
				newCameraLocation.x = bestCamera.x;
            else if (newCameraLocation.xMax > bestCamera.xMax)
				newCameraLocation.x = bestCamera.xMax - newCameraLocation.width;

            if (newCameraLocation.height > bestCamera.height)
                newCameraLocation.y = bestCamera.center.y - (newCameraLocation.height / 2.0f);
            else if (newCameraLocation.yMin < bestCamera.yMin)
                newCameraLocation.y = bestCamera.y;
            else if (newCameraLocation.yMax > bestCamera.yMax)
				newCameraLocation.y = bestCamera.yMax - newCameraLocation.height;
        }

        // Move the camera towards the best fit
        if (ForceCameraReset || CameraSpeed <= 0)
        {
            cameraLocation = newCameraLocation;
            ForceCameraReset = false;
        }
        else
        {
            cameraMove.x = newCameraLocation.x - cameraLocation.x;
            cameraMove.y = newCameraLocation.y - cameraLocation.y;

            if (cameraMove.magnitude > CameraSpeed * Time.deltaTime)
            {
                cameraMove.Normalize();
                cameraMove *= CameraSpeed * Time.deltaTime;
            }

            cameraLocation.x += cameraMove.x;
            cameraLocation.y += cameraMove.y;
        }

		tmpPos = transform.position;
		tmpPos.x = cameraLocation.center.x;
		tmpPos.y = cameraLocation.center.y;
		transform.position = tmpPos;

        //transform.Translate(
        //        cameraLocation.center.x - Player.transform.position.x,
        //        cameraLocation.center.y - Player.transform.position.y,
        //        0
        //    );

	}

}
