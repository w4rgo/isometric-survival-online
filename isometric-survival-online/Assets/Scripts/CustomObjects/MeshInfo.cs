using UnityEngine;

namespace DefaultNamespace.CustomObjects
{
    public class MeshInfo
    {
        private Vector3[] vertices;
        private int[] triangles;

        public MeshInfo(Vector3[] vertices, int[] triangles)
        {
            this.vertices = vertices;
            this.triangles = triangles;
        }

        public Vector3[] Vertices
        {
            get { return vertices; }
        }

        public int[] Triangles
        {
            get { return triangles; }
        }


        public static MeshInfo GetCube()
        {
            Vector3[] vertices =
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 1, 1),
                new Vector3(1, 1, 1),
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 1)
            };

            int[] triangles =
            {
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

            return new MeshInfo(vertices, triangles);
        }


        public static MeshInfo GetNESlope()
        {
            Vector3[] vertices =
            {
                new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(0, 1, 0), new Vector3(1, 1, 0),
                new Vector3(0, 0, 0), new Vector3(1, 0, 0),
            };
            int[] triangles =
            {
                0, 1, 2, 1, 3, 2, 0, 2, 4, 1, 5, 3, 0, 4, 5, 0, 5, 1, 4, 2, 5, 2, 3, 5,
            };


            return new MeshInfo(vertices, triangles);
        }

        public static MeshInfo GetSESlope()
        {
            Vector3[] vertices =
            {
                new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(0, 1, 1), new Vector3(1, 1, 1),
                new Vector3(0, 0, 0), new Vector3(1, 0, 0),
            };
            int[] triangles =
            {
                0, 1, 2, 1, 3, 2, 0, 2, 4, 1, 5, 3, 0, 4, 5, 0, 5, 1, 4, 2, 5, 2, 3, 5,
            };


            return new MeshInfo(vertices, triangles);
        }

        public static MeshInfo GetSWSlope()
        {
            Vector3[] vertices =
            {
                new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 1),
                new Vector3(1, 1, 1), new Vector3(0, 0, 1),
            };
            int[] triangles =
            {
                0, 2, 1, 1, 2, 3, 3, 2, 4, 1, 5, 0, 1, 3, 5, 4, 5, 3, 4, 0, 5, 4, 2, 0,
            };


            return new MeshInfo(vertices, triangles);
        }

        public static MeshInfo GetNWSlope()
        {
            Vector3[] vertices =
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 1),
                new Vector3(1, 0, 1)
            };

            int[] triangles =
            {
                0, 2, 1,
                0, 3, 2,
                2, 3, 4,
                0, 1, 3,
                1, 5, 3,
                4, 3, 5,
                4, 5, 1,
                1, 2, 4
            };

            return new MeshInfo(vertices, triangles);
        }
    }
}