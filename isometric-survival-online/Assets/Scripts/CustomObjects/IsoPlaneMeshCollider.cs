using Assets.Scripts;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Assets.UltimateIsometricToolkit.Scripts.External;
using Assets.UltimateIsometricToolkit.Scripts.Utils;
using DefaultNamespace.CustomObjects;
using UltimateIsometricToolkit.physics;
using UnityEngine;

namespace DefaultNamespace
{
    public class IsoPlaneMeshCollider : IsoCollider
    {
        [HideInInspector, SerializeField] private Vector3 _scale = Vector3.one;
        [HideInInspector, SerializeField] private bool _convex;
        private Mesh internalMesh;

        [ExposeProperty]
        public bool Convex
        {
            get { return _convex; }
            set
            {
                _convex = value;
                if (MCollider != null)
                    MCollider.convex = _convex;
            }
        }

        [ExposeProperty]
        public Vector3 Scale
        {
            get { return _scale; }
            set
            {
                _scale = new Vector3(Mathf.Clamp(value.x, 0, Mathf.Infinity), Mathf.Clamp(value.y, 0, Mathf.Infinity),
                    Mathf.Clamp(value.z, 0, Mathf.Infinity));
                ;
                if (MCollider != null)
                    MCollider.transform.localScale = _scale;
            }
        }

        [ExposeProperty]
        public Mesh Mesh
        {
            get { return MCollider.sharedMesh; }
            set { MCollider.sharedMesh = value; }
        }

        private MeshCollider MCollider
        {
            get { return Collider as MeshCollider; }
        }

        protected override Collider instantiateCollider(GameObject obj)
        {
            var collider = obj.AddComponent<MeshCollider>();
            collider.convex = Convex;
            collider.sharedMesh = internalMesh;
            obj.transform.localScale = Scale;
            return collider;
        }

        void Start()
        {
            internalMesh = CreateMesh();
            GetComponent<MeshFilter>().mesh = internalMesh;
        }

        private Mesh CreateMesh()
        {
            var isoTransform = GetComponent<IsoTransform>();
            if (internalMesh != null)
            {
                internalMesh.Clear();
            }
            else
            {
                internalMesh = new Mesh();
            }

            Vector3[] vertices =
            {
                new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(1, 0, 1),
            };
            int[] triangles =
            {
                0, 3, 2, 0, 1, 3,
            };

            Vector3[] newVertices = new Vector3[vertices.Length];
            for (int i = 0; i < newVertices.Length; i++)
            {
                var x = vertices[i].x;
                var y = vertices[i].y;
                var z = vertices[i].z;
//                newVertices[i] = Isometric.IsoToUnitySpace(new Vector3(x,y,z)-Vector3.one/2 );
                newVertices[i] = new Vector3(x, y, z) - Vector3.one / 2;
            }
            internalMesh.Clear();
            internalMesh.vertices = newVertices;
            internalMesh.triangles = triangles;
            internalMesh.RecalculateNormals();
//            GetComponent<MeshCollider>().sharedMesh = mesh;
            return internalMesh;
        }


        private void OnDrawGizmos()
        {
            var isoTransform = GetComponent<IsoTransform>();

            var mesh = CreateMesh();
            GetComponent<MeshFilter>().mesh = mesh;
            GizmosExtension.DrawIsoMesh(mesh, isoTransform.Position, isoTransform.Size);
        }
    }
}