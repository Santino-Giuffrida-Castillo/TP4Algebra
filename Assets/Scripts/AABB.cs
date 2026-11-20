using UnityEngine;
using UnityEngine.UIElements;

public class AABB : MonoBehaviour
{
    [SerializeField] Frustrum frustrum;

    private Vector3 localCenter;
    private Vector3 localExtents;

    private Mesh mesh;
    private MeshRenderer meshRenderer;

    private bool isVisible = false;

    void Start()
    {

        mesh = GetComponentInChildren<MeshFilter>().sharedMesh;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        Bounds localBounds = mesh.bounds;

        localCenter = localBounds.center;
        localExtents = localBounds.extents;
    }

    private void Update()
    {
        isVisible = IsAnyVertexVisible();

        if(!isVisible)
        {
            meshRenderer.enabled = false;
        }
        else
        {
            meshRenderer.enabled = true;
        }

        if (isVisible)
        {
            Debug.Log($"[DENTRO] {gameObject.name} esta en el rango de vision.");
        }
        else
        {
            Debug.Log($"[FUERA] {gameObject.name} esta fuera de vision.");
        }
    }

    void OnDrawGizmos()
    {
        if (GetComponentInChildren<MeshFilter>() == null) 
        {
        return;
        }

        if (mesh == null) 
        {
            return;
        }

        if (localExtents == Vector3.zero)
        {
            Bounds localBounds = mesh.bounds;
            localCenter = localBounds.center;
            localExtents = localBounds.extents;
        }

        // Matriz de transformacion actual
        Matrix4x4 localToWorld = transform.localToWorldMatrix;

        // Aca saco declaro el centro del mundo
        Vector3 worldCenter = localToWorld.MultiplyPoint3x4(localCenter);

        // Extraemos ejes
        Vector3 right = new Vector3(localToWorld.m00, localToWorld.m01, localToWorld.m02);
        Vector3 up = new Vector3(localToWorld.m10, localToWorld.m11, localToWorld.m12);
        Vector3 forward = new Vector3(localToWorld.m20, localToWorld.m21, localToWorld.m22);

        // Le hacemos valor absoluto (lo dejamos positivo)
        right = new Vector3(Mathf.Abs(right.x), Mathf.Abs(right.y), Mathf.Abs(right.z));
        up = new Vector3(Mathf.Abs(up.x), Mathf.Abs(up.y), Mathf.Abs(up.z));
        forward = new Vector3(Mathf.Abs(forward.x), Mathf.Abs(forward.y), Mathf.Abs(forward.z));

        // Extents en mundo
        Vector3 worldExtents =
            right * localExtents.x +
            up * localExtents.y +
            forward * localExtents.z;

        // Bounds en mundo
        Bounds worldBounds = new Bounds(worldCenter, worldExtents * 2f);

        // Dibujar
        if (isVisible)    
        {    
            Gizmos.color = Color.green; 
        }        
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
    }

    private bool IsPointInsideFrustum(Vector3 worldPoint)
    {
        return (frustrum.GetNear().GetSide(worldPoint) && frustrum.GetFar().GetSide(worldPoint) && frustrum.GetLeft().GetSide(worldPoint) &&
            frustrum.GetRight().GetSide(worldPoint) && frustrum.GetTop().GetSide(worldPoint) && frustrum.GetBottom().GetSide(worldPoint));
    }

    private bool IsAnyVertexVisible()
    {
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldVertex = transform.TransformPoint(vertices[i]);
            if (IsPointInsideFrustum(worldVertex))
            {
                return true;
            }
        }

        return false;
    }
}
