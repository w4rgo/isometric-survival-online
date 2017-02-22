using Assets.UltimateIsometricToolkit.Scripts.Core;
using Assets.UltimateIsometricToolkit.Scripts.External;
using Assets.UltimateIsometricToolkit.Scripts.Utils;
using UltimateIsometricToolkit.physics;
using UnityEngine;

namespace DefaultNamespace
{
    public class MeshExperiment : MonoBehaviour
    {
        [SerializeField]
        public Vector3[] vertices = {
            new Vector3 (0, 0, 0),
            new Vector3 (1, 0, 0),
            new Vector3 (1, 1, 0),
            new Vector3 (0, 1, 0),
            new Vector3 (0, 1, 1),
            new Vector3 (1, 1, 1),
            new Vector3 (1, 0, 1),
            new Vector3 (0, 0, 1)
        };

        [SerializeField]
        public int[] triangles = {
            0, 2, 1, //face front
            0, 3, 2,
            2, 3, 4, //face top
            2, 4, 5,
            1, 2, 5, //face right
            1, 5, 6,
            0, 7, 4, //face left
            0, 4, 3,
            5, 4, 7, //face back
            5, 7, 6,
            0, 6, 7, //face bottom
            0, 1, 6
        };

        void Start()
        {
            GetComponent<MeshFilter>().mesh = CreateCube();

        }
        private Mesh CreateCube () {
            var isoTransform = GetComponent<IsoTransform>();
            Vector3[] newVertices = new Vector3[vertices.Length];
            for (int i = 0; i < newVertices.Length; i++)
            {
                var x = vertices[i].x;
                var y = vertices[i].y;
                var z = vertices[i].z;
                newVertices[i] = Isometric.IsoToUnitySpace(new Vector3(x,y,z)-Vector3.one/2 );
            }
            Mesh mesh = new Mesh();
            mesh.Clear ();
            mesh.vertices = newVertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals ();
            GetComponent<MeshCollider>().sharedMesh = mesh;

            return mesh;
        }


        private void OnDrawGizmos()
        {
            var isoTransform = GetComponent<IsoTransform>();
            transform.position = isoTransform.Position;
            GetComponent<MeshFilter>().mesh = CreateCube();
        }

    }
}