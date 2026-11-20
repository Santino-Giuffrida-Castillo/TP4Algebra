using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

public class Frustrum : MonoBehaviour
{
    [SerializeField] private float fov = 60f;
    [SerializeField] private float aspectRatio = 16f / 9f;
    [SerializeField] private MeshFilter targetObject;
    [SerializeField] private Camera camera;

    [SerializeField] private float zNear = 0.3f;
    [SerializeField] private float zFar = 100f;

    private Plane top;
    private Plane bottom;
    private Plane left;
    private Plane right;
    private Plane near;
    private Plane far;

    float aspect;
    float fovY;

    private void Update()
    {
        if (camera == null) return;
    }

    void CalculateFrustrum()
    {
        float fovY = fov * Mathf.Deg2Rad;
        float halfVSide = zFar * Mathf.Tan(fovY * .5f);
        float halfHSide = halfVSide * aspectRatio;

        Vector3 frontMultFar = zFar * camera.transform.forward;
        Vector3 rightMultFar = camera.transform.right * halfHSide;
        Vector3 upMultFar = camera.transform.up * halfVSide;

        near = new Plane(camera.transform.forward, camera.transform.position + zNear * camera.transform.forward);

        far = new Plane(-camera.transform.forward, camera.transform.position + frontMultFar);

        right = new Plane(Vector3.Cross(frontMultFar + rightMultFar, camera.transform.up).normalized, camera.transform.position);

        left = new Plane(Vector3.Cross(camera.transform.up, frontMultFar - rightMultFar).normalized, camera.transform.position);

        top = new Plane(Vector3.Cross(camera.transform.right, frontMultFar + upMultFar).normalized, camera.transform.position);

        bottom = new Plane(Vector3.Cross(frontMultFar - upMultFar, camera.transform.right).normalized, camera.transform.position);
    }

    private Vector3 IntersectPlanes(Plane p1, Plane p2, Plane p3)
    {
        Vector3 n1 = p1.normal;
        Vector3 n2 = p2.normal;
        Vector3 n3 = p3.normal;

        float denominator = Vector3.Dot(n1, Vector3.Cross(n2, n3));

        if (Mathf.Abs(denominator) < 0.00001f)
            return Vector3.zero;

        return (
            -p1.distance * Vector3.Cross(n2, n3) -
            p2.distance * Vector3.Cross(n3, n1) -
            p3.distance * Vector3.Cross(n1, n2)
        ) / denominator;
    }

    private void DrawFrustrum()
    {
        // Near plane vertices
        Vector3 ntl = IntersectPlanes(near, top, left);
        Vector3 ntr = IntersectPlanes(near, top, right);
        Vector3 nbl = IntersectPlanes(near, bottom, left);
        Vector3 nbr = IntersectPlanes(near, bottom, right);

        // Far plane vertices
        Vector3 ftl = IntersectPlanes(far, top, left);
        Vector3 ftr = IntersectPlanes(far, top, right);
        Vector3 fbl = IntersectPlanes(far, bottom, left);
        Vector3 fbr = IntersectPlanes(far, bottom, right);

        // Near plane
        Gizmos.DrawLine(ntl, ntr);
        Gizmos.DrawLine(ntr, nbr);
        Gizmos.DrawLine(nbr, nbl);
        Gizmos.DrawLine(nbl, ntl);

        // Far plane
        Gizmos.DrawLine(ftl, ftr);
        Gizmos.DrawLine(ftr, fbr);
        Gizmos.DrawLine(fbr, fbl);
        Gizmos.DrawLine(fbl, ftl);

        // Connecting edges
        Gizmos.DrawLine(ntl, ftl);
        Gizmos.DrawLine(ntr, ftr);
        Gizmos.DrawLine(nbl, fbl);
        Gizmos.DrawLine(nbr, fbr);
    }

    private void OnDrawGizmos()
    {
        float fovY = fov * Mathf.Deg2Rad;

        CalculateFrustrum();

        DrawFrustrum();
    }

    public Plane GetNear()
    {
        return near;
    }

    public Plane GetFar()
    {
        return far;
    }

    public Plane GetTop()
    {
        return top;
    }

    public Plane GetBottom()
    {
        return bottom;
    }

    public Plane GetRight()
    {
        return right;
    }

    public Plane GetLeft()
    {
        return left;   
    }
}
