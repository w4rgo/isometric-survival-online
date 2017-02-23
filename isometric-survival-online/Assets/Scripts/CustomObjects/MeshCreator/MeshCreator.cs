using System.Collections.Generic;
using System.Text;
using DefaultNamespace.CustomObjects;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.CustomObjects
{
    public class MeshCreator : MonoBehaviour
    {
        [SerializeField] private Vector3[] vertices;

        [SerializeField] private List<Vector3> trianglesVector;

        private Mesh internalMesh;

        private Mesh CreateMesh()
        {


            if (internalMesh != null)
            {
                internalMesh.Clear();
            }
            else
            {
                internalMesh = new Mesh();
            }

            Vector3[] newVertices = new Vector3[vertices.Length];
            for (int i = 0; i < newVertices.Length; i++)
            {
                var x = vertices[i].x;
                var y = vertices[i].y;
                var z = vertices[i].z;
                newVertices[i] = new Vector3(x, y, z) - Vector3.one / 2;
            }
            internalMesh.Clear();
            internalMesh.vertices = newVertices;
            internalMesh.triangles = ProcessTriangles();
            internalMesh.RecalculateNormals();
            return internalMesh;
        }

        private int[] ProcessTriangles()
        {
            var triangles = new int[trianglesVector.Count * 3];
            int index = 0;
            foreach (var triangle in trianglesVector)
            {
                triangles[index] = (int) triangle.x;
                triangles[index + 1] =(int) triangle.y;
                triangles[index + 2] =(int) triangle.z;
                index += 3;
            }
            return triangles;
        }

        private void OnDrawGizmos()
        {
            if (vertices == null)
            {
                return;

            }

            for (var index = 0; index < vertices.Length; index++)
            {
                var vertex = vertices[index];
                var vertexPos = vertex + transform.position - Vector3.one / 2;
                Gizmos.DrawSphere(vertexPos, 0.05f);
                Handles.Label(vertexPos + new Vector3(-0.1f,-0.1f,-0.1f), index.ToString());
                Handles.Label(vertexPos + new Vector3(-0.3f,-0.3f,-0.3f), vertex.ToString());
            }

            if (trianglesVector == null)
            {
                return;
            }

            var mesh = CreateMesh();
            GetComponent<MeshFilter>().mesh = mesh;
            Gizmos.DrawWireMesh(mesh, transform.position);


        }

        public string GetStringArrays()
        {

            StringBuilder output = new StringBuilder();
            output.AppendLine("Vector3[] vertices = {");
            foreach (var vector3 in vertices)
            {
                output.AppendFormat("new Vector3({0},{1},{2}),", vector3.x, vector3.y, vector3.z);
            }
            output.AppendLine("};");
            output.AppendLine("int[] triangles = {");
            foreach (var corner in ProcessTriangles())
            {
                output.Append(corner + ",");
            }
            output.AppendLine("};");
            return output.ToString();
        }
    }
}