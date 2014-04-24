using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Attach this script to the camera. It will adjust the Z coordinates of all
// GameObjects that have CameraSorterObject components. (Remember that your
// object bounding boxes Z scale should take into account the BaseZ, LayerZ,
// and ZStep values.
//
// An object's Z coordinate will be calculated as:
// BaseZ + (Layer * LayerZ) + (ZStep * zIndex)
public class CameraSorter : MonoBehaviour
{
    // The lowest Z coordinate each object will use
    public float BaseZ = 0;

    // The base Z-index for each layer
    public float LayerZ = 0.1f;

    // The increment for each Z-index value
    public float ZStep = 0.01f;

    private List<CameraSorterObject> objectsInView = new List<CameraSorterObject>();
    private Renderer objectRenderer;
    private int zIndex = 0;
    private BoxCollider boxCollider;
    void LateUpdate()
    {
        // Collect all objects in the camera view
        objectsInView.Clear();
        foreach (CameraSorterObject cso in FindSceneObjectsOfType(typeof (CameraSorterObject)))
        {
            if (cso.renderer.isVisible)
            {
                boxCollider = cso.GetComponent<BoxCollider>();

                cso.YSort = cso.gameObject.transform.position.y +
                            (boxCollider != null ? boxCollider.center.y : 0);
                objectsInView.Add(cso);
            }
        }

        // Sort those objects by ZIndex and then by Y
        objectsInView.Sort((o, o1) =>
            {
                return o.YSort.CompareTo(o1.YSort);
            });

        zIndex = 0;
        foreach (CameraSorterObject cso in objectsInView)
        {
            // Set new Z value so objects get "stacked" properly.
            cso.gameObject.transform.Translate(0, 0, GetZ(cso.Layer, zIndex) - cso.gameObject.transform.position.z);
            zIndex++;
        }
    }

    public float GetZ(int layer, int zIndex)
    {
        return BaseZ + (LayerZ * layer) + (ZStep * zIndex);
    }
}
