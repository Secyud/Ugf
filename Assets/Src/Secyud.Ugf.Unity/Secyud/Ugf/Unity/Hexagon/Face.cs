using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.Unity.Hexagon
{
    public struct Face
    {
        public  List<Vector3> Vertices { get; private set; }
        public  List<int> Triangles { get; private set; }
        public  List<Vector2> Uvs { get; private set; }

        public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
        {
            Vertices = vertices;
            Triangles = triangles;
            Uvs = uvs;
        }
    }
}