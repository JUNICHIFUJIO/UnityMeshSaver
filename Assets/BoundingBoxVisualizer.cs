using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxVisualizer : MonoBehaviour {
    public Color NotContainedColor = Color.red;
    public Color ContainedColor = Color.green;
    public bool IsGoContainedInMinMax = false;
    public GameObject MeshObjectToDisplayBoundsFor;
    public Vector3[] MinAndMax = new Vector3[2];
    public Vector3[] BoxCorners = new Vector3[8];

    private Color LineColor = Color.red;
    public bool GOTrigger = false;
    public bool MinMaxTrigger = false;
    public bool CornersTrigger = false;

    #region Editor Inspector Methods
    public void ToggleGameObjectBounds()
    {
        GOTrigger = !GOTrigger;
    }

    public void ToggleMinAndMaxBounds()
    {
        MinMaxTrigger = !MinMaxTrigger;
    }

    public void ToggleBoxCornersBounds()
    {
        CornersTrigger = !CornersTrigger;
    }

    public void VisualizeGameObjectBounds()
    {
        Bounds b = GetBoundsFor(MeshObjectToDisplayBoundsFor);
        BoundCorners corners = CalculateCorners(b, MeshObjectToDisplayBoundsFor);
        VisualizeBox(corners);
        //VisualizeBoxWithRays(corners);
    }

    public void VisualizeBoxFromMinAndMax()
    {
        BoundCorners corners = CalculateCorners(MinAndMax[0], MinAndMax[1]);
        VisualizeBox(corners);
    }

    public void VisualizeBoxFromBoxCorners()
    {
        BoundCorners corners = GetBoundCorners(BoxCorners[0], BoxCorners[1], BoxCorners[2], BoxCorners[3], BoxCorners[4], BoxCorners[5], BoxCorners[6], BoxCorners[7]);
        VisualizeBox(corners);
    }
    #endregion

    #region Logic
    public BoundCorners CalculateCorners(Bounds bounds, GameObject go)
    {
        Vector3 cornerMin = bounds.min; // Front, Bottom, Left
        Vector3 cornerFTL = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z); // Front, Top, Left
        Vector3 cornerFTR = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z); // Front, Top, Right
        Vector3 cornerFBR = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z); // Front, Bottom, Right
        Vector3 cornerBBL = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z); // Back, Bottom, Left
        Vector3 cornerBTL = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z); // Back, Top, Left
        Vector3 cornerMax = bounds.max; // Back, Top, Right
        Vector3 cornerBBR = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z); // Back, Bottom, Right

        if(go != null)
        {
            cornerMin = RotateBoundingBoxPoint(cornerMin, go);
            cornerFTL = RotateBoundingBoxPoint(cornerFTL, go);
            cornerFTR = RotateBoundingBoxPoint(cornerFTR, go);
            cornerFBR = RotateBoundingBoxPoint(cornerFBR, go);
            cornerBBL = RotateBoundingBoxPoint(cornerBBL, go);
            cornerBTL = RotateBoundingBoxPoint(cornerBTL, go);
            cornerMax = RotateBoundingBoxPoint(cornerMax, go);
            cornerBBR = RotateBoundingBoxPoint(cornerBBR, go);
        }

        BoundCorners corners = new BoundCorners(cornerMin, cornerFTL, cornerFTR, cornerFBR, cornerBBL, cornerBTL, cornerMax, cornerBBR);

        return corners;
    }

    public void Update()
    {
        if (GOTrigger)
        {
            VisualizeGameObjectBounds();
        }
        if (MinMaxTrigger)
        {
            VisualizeBoxFromMinAndMax();
        }
        if (CornersTrigger)
        {
            VisualizeBoxFromBoxCorners();
        }

        IsGoContainedInMinMax = IsGoContainedInMinMaxCalc();
        if (IsGoContainedInMinMax)
        {
            LineColor = ContainedColor;
        }
        else
        {
            LineColor = NotContainedColor;
        }
    }

    //public void VisualizeBoxWithRays(BoundCorners corners)
    //{
    //    Debug.Log("Corner0 = " + corners.FrontBottomLeft);
    //    Debug.Log("Corner1 = " + corners.FrontTopLeft);
    //    Debug.Log("Corner2 = " + corners.FrontTopRight);
    //    Debug.Log("Corner3 = " + corners.FrontBottomRight);
    //    Debug.Log("Corner4 = " + corners.BackBottomLeft);
    //    Debug.Log("Corner5 = " + corners.BackTopLeft);
    //    Debug.Log("Corner6 = " + corners.BackTopRight);
    //    Debug.Log("Corner7 = " + corners.BackBottomRight);

    //    // Connect front face
    //    Debug.DrawRay(corners.FrontBottomLeft, (corners.FrontTopLeft - corners.FrontBottomRight), LineColor, (corners.FrontTopLeft - corners.FrontBottomLeft).magnitude);
    //    Debug.DrawLine(corners.FrontBottomLeft, corners.FrontTopLeft, LineColor);
    //    Debug.DrawLine(corners.FrontTopLeft, corners.FrontTopRight, LineColor);
    //    Debug.DrawLine(corners.FrontTopRight, corners.FrontBottomRight, LineColor);
    //    Debug.DrawLine(corners.FrontBottomRight, corners.FrontBottomLeft, LineColor);

    //    // Connect back face
    //    Debug.DrawLine(corners.BackBottomLeft, corners.BackTopLeft, LineColor);
    //    Debug.DrawLine(corners.BackTopLeft, corners.BackTopRight, LineColor);
    //    Debug.DrawLine(corners.BackTopRight, corners.BackBottomRight, LineColor);
    //    Debug.DrawLine(corners.BackBottomRight, corners.BackBottomLeft, LineColor);

    //    // Connect faces
    //    Debug.DrawLine(corners.FrontBottomLeft, corners.BackBottomLeft, LineColor);
    //    Debug.DrawLine(corners.FrontTopLeft, corners.BackTopLeft, LineColor);
    //    Debug.DrawLine(corners.FrontTopRight, corners.BackTopRight, LineColor);
    //    Debug.DrawLine(corners.FrontBottomRight, corners.BackBottomRight, LineColor);
    //}

    public void VisualizeBox(BoundCorners corners)
    {
        // Connect front face
        Debug.DrawLine(corners.FrontBottomLeft, corners.FrontTopLeft, LineColor);
        Debug.DrawLine(corners.FrontTopLeft, corners.FrontTopRight, LineColor);
        Debug.DrawLine(corners.FrontTopRight, corners.FrontBottomRight, LineColor);
        Debug.DrawLine(corners.FrontBottomRight, corners.FrontBottomLeft, LineColor);

        // Connect back face
        Debug.DrawLine(corners.BackBottomLeft, corners.BackTopLeft, LineColor);
        Debug.DrawLine(corners.BackTopLeft, corners.BackTopRight, LineColor);
        Debug.DrawLine(corners.BackTopRight, corners.BackBottomRight, LineColor);
        Debug.DrawLine(corners.BackBottomRight, corners.BackBottomLeft, LineColor);

        // Connect faces
        Debug.DrawLine(corners.FrontBottomLeft, corners.BackBottomLeft, LineColor);
        Debug.DrawLine(corners.FrontTopLeft, corners.BackTopLeft, LineColor);
        Debug.DrawLine(corners.FrontTopRight, corners.BackTopRight, LineColor);
        Debug.DrawLine(corners.FrontBottomRight, corners.BackBottomRight, LineColor);
    }
    
    public BoundCorners GetBoundCorners(Vector3 frontBottomLeft, Vector3 frontTopLeft, Vector3 frontTopRight, Vector3 frontBottomRight, Vector3 backBottomLeft, Vector3 backTopLeft, Vector3 backTopRight, Vector3 backBottomRight)
    {
        BoundCorners corners = new BoundCorners(
            frontBottomLeft,
            frontTopLeft,
            frontTopRight,
            frontBottomRight,
            backBottomLeft,
            backTopLeft,
            backTopRight,
            backBottomRight);

        return corners;
    }

    public BoundCorners CalculateCorners(Vector3 min, Vector3 max)
    {
        Vector3 center = min + ((max - min) / 2);
        Vector3 size = max - min;
        Bounds b = new Bounds(center, size);

        BoundCorners corners = CalculateCorners(b, null);
        return corners;
    }

    public Bounds GetBoundsFor(GameObject go)
    {
        var mf = go.GetComponent<MeshFilter>();

        if(mf != null)
        {
            mf.mesh.RecalculateBounds();
            Bounds b = mf.mesh.bounds;
            b.center += go.transform.position;
            return b;
        }
        else
        {
            Debug.LogError("Could not find MeshFilter component for GameObject. Unable to calculate bounds to draw them.");
            return new Bounds();
        }
    }

    public bool IsGoContainedInMinMaxCalc()
    {
        BoundCorners goCorners = CalculateCorners(GetBoundsFor(MeshObjectToDisplayBoundsFor), MeshObjectToDisplayBoundsFor);
        BoundCorners minMaxCorners = CalculateCorners(MinAndMax[0], MinAndMax[1]);

        bool fbl = IsLesserThanPoint(goCorners.FrontBottomLeft, MinAndMax[0]) && IsGreaterThanPoint(goCorners.FrontBottomLeft, MinAndMax[1]);
        if (!fbl)
        {
            bool ftl = IsLesserThanPoint(goCorners.FrontTopLeft, MinAndMax[0]) && IsGreaterThanPoint(goCorners.FrontTopLeft, MinAndMax[1]);
            if (!ftl)
            {
                bool ftr = IsLesserThanPoint(goCorners.FrontTopRight, MinAndMax[0]) && IsGreaterThanPoint(goCorners.FrontTopRight, MinAndMax[1]);
                if (!ftr)
                {
                    bool fbr = IsLesserThanPoint(goCorners.FrontBottomRight, MinAndMax[0]) && IsGreaterThanPoint(goCorners.FrontBottomRight, MinAndMax[1]);
                    if (!fbr)
                    {
                        bool bbl = IsLesserThanPoint(goCorners.BackBottomLeft, MinAndMax[0]) && IsGreaterThanPoint(goCorners.BackBottomLeft, MinAndMax[1]);
                        if (!bbl)
                        {
                            bool btl = IsLesserThanPoint(goCorners.BackTopLeft, MinAndMax[0]) && IsGreaterThanPoint(goCorners.BackTopLeft, MinAndMax[1]);
                            if (!btl)
                            {
                                bool btr = IsLesserThanPoint(goCorners.BackTopRight, MinAndMax[0]) && IsGreaterThanPoint(goCorners.BackTopRight, MinAndMax[1]);
                                if (!btr)
                                {
                                    bool bbr = IsLesserThanPoint(goCorners.BackBottomRight, MinAndMax[0]) && IsGreaterThanPoint(goCorners.BackBottomRight, MinAndMax[1]);
                                    if (!bbr)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return true;
    }
    
    public bool IsGreaterThanPoint(Vector3 a, Vector3 b)
    {
        // is b greater than a?
        float xcon = b.x - a.x;
        float ycon = b.y - a.y;
        float zcon = b.z - a.z;

        //return (xcon >= 0) && (ycon >= 0) && (zcon >= 0);

        return (b.x >= a.x) && (b.y >= a.y) && (b.z >= a.z);
    }

    public bool IsLesserThanPoint(Vector3 a, Vector3 b)
    {
        // is b less than a?
        float xcon = b.x - a.x;
        float ycon = b.y - a.y;
        float zcon = b.z - a.z;

        //return (xcon <= 0) && (ycon <= 0) && (zcon <= 0);
        return (b.x <= a.x) && (b.y <= a.y) && (b.z <= a.z);
    }

    public Vector3 RotateBoundingBoxPoint(Vector3 point, GameObject go)
    {
        Vector3 pivot = go.transform.position;
        Quaternion quat = go.transform.rotation;
        Vector3 dir = point - pivot;

        Vector3 rotatedDir = quat * dir;
        return pivot + rotatedDir;
    }
    #endregion
}