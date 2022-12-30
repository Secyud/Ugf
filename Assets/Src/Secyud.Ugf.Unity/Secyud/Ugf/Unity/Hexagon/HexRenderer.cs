using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Secyud.Ugf.Unity.Hexagon
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class HexRenderer : MonoBehaviour
    {
        private Mesh _mesh;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
    
    
        private List<Face> _faces;

        public Material Material;

        public float InnerSize = 0f;
        public float OuterSize = 1f;
        public float Height = 1;

        public bool IsFlapTopped;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        
            _mesh = new Mesh
            {
                name = "Hex"
            };
            _meshFilter.mesh = _mesh;
            _meshRenderer.material = Material;
        }

        private void OnEnable()
        {
            DrawMesh();
        }

        private void OnValidate()
        {
            if (Application.isPlaying && _mesh is not null)
            {
                DrawMesh();
            }
        }

        public void DrawMesh()
        {
            DrawFaces();
            CombineFaces();
        }

        private void DrawFaces()
        {
            _faces = new List<Face>();
            for (var point = 0; point < 6; point++)
                _faces.Add(CreateFace(InnerSize,OuterSize,Height/2f,Height/2f,point));
        
            for (var point = 0; point < 6; point++)
                _faces.Add(CreateFace(InnerSize,OuterSize,-Height/2f,-Height/2f,point,true));
        
            for (var point = 0; point < 6; point++)
                _faces.Add(CreateFace(OuterSize,OuterSize,Height/2f,-Height/2f,point,true));

            for (var point = 0; point < 6; point++)
                _faces.Add(CreateFace(InnerSize,InnerSize,-Height/2f,Height/2f,point,true));
        }

        private void CombineFaces()
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();

            for (var i= 0; i< _faces.Count;i++)
            {
                vertices.AddRange(_faces[i].Vertices);
                uvs.AddRange(_faces[i].Uvs);

                var offset = i * 4;

                triangles.AddRange(
                    _faces[i]
                        .Triangles
                        .Select(triangle => triangle + offset));
            }

            _mesh.vertices = vertices.ToArray();
        
            _mesh.triangles = triangles.ToArray();
            _mesh.uv = uvs.ToArray();
            _mesh.RecalculateNormals();
        }


        private Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point,
            bool reverse = false)
        {
            var pointA = GetPoint(innerRad, heightB, point);
            var pointB = GetPoint(innerRad, heightB, (point + 1) % 6);
            var pointC = GetPoint(outerRad, heightA, (point + 1) % 6);
            var pointD = GetPoint(outerRad, heightA, point);

            var vertices = new List<Vector3> { pointA, pointB, pointC, pointD };
            var triangles = new List<int> { 0, 1, 2, 2, 3, 0 };
            var uvs = new List<Vector2> { new(0,0), new(1,0), new(1,1), new(0,1) };

            if (reverse)
                vertices.Reverse();

            return new Face(vertices, triangles, uvs);
        }

        private Vector3 GetPoint(float size, float height, int index)
        {
            float angleDeg = IsFlapTopped ? index * 60 : index * 60 - 30;
            var angleRad = angleDeg * Mathf.PI / 180;
            return new Vector3(size * Mathf.Cos(angleRad), height, size * Mathf.Sin(angleRad));
        }

        public void SetMaterial(Material material)
        {
            Material = material;
            _meshRenderer.material = Material;
        }
    }
}
