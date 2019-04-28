using UnityEngine;
using System.Collections;

public class SplitMeshTriangles : MonoBehaviour
{
    private Vector3 scale;

    void Start()
    {
        scale = transform.lossyScale;

        //StartCoroutine(SplitMesh());
    }

    IEnumerator SplitMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        Mesh mesh = meshFilter.mesh;

        Vector3[] verts = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uvs = mesh.uv;

        // Go through each submesh
        for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
        {
            int[] indices = mesh.GetTriangles(submesh);

            // For each triangle in submesh
            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3[] newVerts = new Vector3[3];
                Vector3[] newNormals = new Vector3[3];
                Vector2[] newUvs = new Vector2[3];

                for (int n = 0; n < 3; n++)
                {
                    int index = indices[i + n];
                    newVerts[n] = verts[index];
                    newUvs[n] = uvs[index];
                    newNormals[n] = normals[index];
                }

                // New tri
                Mesh newMesh = new Mesh();
                newMesh.vertices = newVerts;
                newMesh.normals = newNormals;
                newMesh.uv = newUvs;

                newMesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };

                SpawnTri(i, submesh, meshRenderer, newMesh);
            }
        }
        meshRenderer.enabled = false;

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    // Spawn tri
    private void SpawnTri(int i, int subMesh, MeshRenderer meshRenderer, Mesh newMesh)
    {
        GameObject gameObject = new GameObject("Tri " + (i / 3));

        // Set tri transforms
        gameObject.transform.position = transform.position;
        gameObject.transform.rotation = transform.rotation;
        gameObject.transform.localScale = scale;

        // Add components to seperate tris
        gameObject.AddComponent<MeshRenderer>().material = meshRenderer.materials[subMesh];
        gameObject.AddComponent<MeshFilter>().mesh = newMesh;
        gameObject.AddComponent<BoxCollider>();

        // Explode the tri
        gameObject.AddComponent<Rigidbody>().AddExplosionForce(100, transform.position, 30);

        // Destroy after random time
        Destroy(gameObject, Random.Range(1.0f, 3.0f));
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            // Half speed from impact
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            rb.velocity = new Vector3(rb.velocity.x / 2, rb.velocity.y, rb.velocity.z / 2);

            StartCoroutine(SplitMesh());
        }
        else if (other.gameObject.CompareTag("Projectile"))
        {
            StartCoroutine(SplitMesh());
        }
    }
}