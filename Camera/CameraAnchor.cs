using UnityEngine;
using System.Collections;

public class CameraAnchor : MonoBehaviour
{

    public enum VerticalAnchorPoint
    {
        Top,
        Middle,
        Bottom
    }

    public enum HorizontalAnchorPoint
    {
        Left,
        Center,
        Right
    }

    public VerticalAnchorPoint VerticalAnchor;
    public HorizontalAnchorPoint HorizontalAnchor;
    public Vector3 Offset;

	// Use this for initialization
	void Start () {

        // Reset any original position; we'll use the offset instead
        transform.Translate(-transform.localPosition.x, -transform.localPosition.y, 0);

        // Calculate new position
        Vector3 newPosition = new Vector3(0, 0, 0);

        newPosition.y = Camera.main.orthographicSize * (
            VerticalAnchor == VerticalAnchorPoint.Middle ? 0 :
            VerticalAnchor == VerticalAnchorPoint.Bottom ? -1 :
            1
            );

	    newPosition.x = Camera.main.orthographicSize * Camera.main.aspect * (
	        HorizontalAnchor == HorizontalAnchorPoint.Center ? 0
	            : HorizontalAnchor == HorizontalAnchorPoint.Left
	                    ? -1
	                    : 1
	    );

        // Apply offset
	    newPosition += Offset;

        // Move to new position
        transform.Translate(newPosition);
	}
}
