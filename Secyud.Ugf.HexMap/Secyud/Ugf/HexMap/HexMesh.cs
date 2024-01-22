#region

using System;
using System.Collections.Generic;
using Secyud.Ugf.HexUtilities;
using UnityEngine;
using UnityEngine.Pool;

#endregion

namespace Secyud.Ugf.HexMap
{
	/// <summary>
	///     Class containing all data used to generate a mesh while triangulating a hex map.
	/// </summary>
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class HexMesh : MonoBehaviour
	{
		[SerializeField] private bool UseCollider;
		[SerializeField] private bool UseCellData;
		[SerializeField] private bool UseUVCoordinates;
		[SerializeField] private bool UseUV2Coordinates;
		[NonSerialized] private List<Color> _cellWeights;

		private Mesh _hexMesh;
		private MeshCollider _meshCollider;
		[NonSerialized] private List<int> _triangles;
		[NonSerialized] private List<Vector2> _uvs, _uv2S;

		[NonSerialized] private List<Vector3> _vertices, _cellIndices;

		private void Awake()
		{
			GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();
			if (UseCollider)
			{
				_meshCollider = gameObject.AddComponent<MeshCollider>();
			}

			_hexMesh.name = "Hex Mesh";
		}

		/// <summary>
		///     Clear all data.
		/// </summary>
		public void Clear()
		{
			_hexMesh.Clear();
			_vertices = ListPool<Vector3>.Get();
			if (UseCellData)
			{
				_cellWeights = ListPool<Color>.Get();
				_cellIndices = ListPool<Vector3>.Get();
			}
			if (UseUVCoordinates)
			{
				_uvs = ListPool<Vector2>.Get();
			}
			if (UseUV2Coordinates)
			{
				_uv2S = ListPool<Vector2>.Get();
			}
			_triangles = ListPool<int>.Get();
		}

		/// <summary>
		///     Apply all triangulation data to the underlying mesh.
		/// </summary>
		public void Apply()
		{
			_hexMesh.SetVertices(_vertices);
			ListPool<Vector3>.Release(_vertices);
			if (UseCellData)
			{
				_hexMesh.SetColors(_cellWeights);
				ListPool<Color>.Release(_cellWeights);
				_hexMesh.SetUVs(2, _cellIndices);
				ListPool<Vector3>.Release(_cellIndices);
			}

			if (UseUVCoordinates)
			{
				_hexMesh.SetUVs(0, _uvs);
				ListPool<Vector2>.Release(_uvs);
			}

			if (UseUV2Coordinates)
			{
				_hexMesh.SetUVs(1, _uv2S);
				ListPool<Vector2>.Release(_uv2S);
			}

			_hexMesh.SetTriangles(_triangles, 0);
			ListPool<int>.Release(_triangles);
			_hexMesh.RecalculateNormals();
			if (UseCollider) _meshCollider.sharedMesh = _hexMesh;
		}

		/// <summary>
		///     Add a triangle, applying perturbation to the positions.
		/// </summary>
		/// <param name="v1">First vertex position.</param>
		/// <param name="v2">Second vertex position.</param>
		/// <param name="v3">Third vertex position.</param>
		public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
		{
			int vertexIndex = _vertices.Count;
			_vertices.Add(HexMetrics.Perturb(v1));
			_vertices.Add(HexMetrics.Perturb(v2));
			_vertices.Add(HexMetrics.Perturb(v3));
			_triangles.Add(vertexIndex);
			_triangles.Add(vertexIndex + 1);
			_triangles.Add(vertexIndex + 2);
		}

		/// <summary>
		///     Add a triangle verbatim, without perturbing the positions.
		/// </summary>
		/// <param name="v1">First vertex position.</param>
		/// <param name="v2">Second vertex position.</param>
		/// <param name="v3">Third vertex position.</param>
		public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
		{
			int vertexIndex = _vertices.Count;
			_vertices.Add(v1);
			_vertices.Add(v2);
			_vertices.Add(v3);
			_triangles.Add(vertexIndex);
			_triangles.Add(vertexIndex + 1);
			_triangles.Add(vertexIndex + 2);
		}

		/// <summary>
		///     Add UV coordinates for a triangle.
		/// </summary>
		/// <param name="uv1">First UV coordinates.</param>
		/// <param name="uv2">Second UV coordinates.</param>
		/// <param name="uv3">Third UV coordinates.</param>
		public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector3 uv3)
		{
			_uvs.Add(uv1);
			_uvs.Add(uv2);
			_uvs.Add(uv3);
		}

		/// <summary>
		///     Add UV2 coordinates for a triangle.
		/// </summary>
		/// <param name="uv1">First UV2 coordinates.</param>
		/// <param name="uv2">Second UV2 coordinates.</param>
		/// <param name="uv3">Third UV2 coordinates.</param>
		public void AddTriangleUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3)
		{
			_uv2S.Add(uv1);
			_uv2S.Add(uv2);
			_uv2S.Add(uv3);
		}

		/// <summary>
		///     Add cell data for a triangle.
		/// </summary>
		/// <param name="indices">Terrain type indices.</param>
		/// <param name="weights1">First terrain weights.</param>
		/// <param name="weights2">Second terrain weights.</param>
		/// <param name="weights3">Third terrain weights.</param>
		public void AddTriangleCellData(
			Vector3 indices, Color weights1, Color weights2, Color weights3
		)
		{
			_cellIndices.Add(indices);
			_cellIndices.Add(indices);
			_cellIndices.Add(indices);
			_cellWeights.Add(weights1);
			_cellWeights.Add(weights2);
			_cellWeights.Add(weights3);
		}

		/// <summary>
		///     Add cell data for a triangle.
		/// </summary>
		/// <param name="indices">Terrain type indices.</param>
		/// <param name="weights">Terrain weights, uniform for entire triangle.</param>
		public void AddTriangleCellData(Vector3 indices, Color weights)
		{
			AddTriangleCellData(indices, weights, weights, weights);
		}

		/// <summary>
		///     Add a quad, applying perturbation to the positions.
		/// </summary>
		/// <param name="v1">First vertex position.</param>
		/// <param name="v2">Second vertex position.</param>
		/// <param name="v3">Third vertex position.</param>
		/// <param name="v4">Fourth vertex position.</param>
		public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
		{
			int vertexIndex = _vertices.Count;
			_vertices.Add(HexMetrics.Perturb(v1));
			_vertices.Add(HexMetrics.Perturb(v2));
			_vertices.Add(HexMetrics.Perturb(v3));
			_vertices.Add(HexMetrics.Perturb(v4));
			_triangles.Add(vertexIndex);
			_triangles.Add(vertexIndex + 2);
			_triangles.Add(vertexIndex + 1);
			_triangles.Add(vertexIndex + 1);
			_triangles.Add(vertexIndex + 2);
			_triangles.Add(vertexIndex + 3);
		}

		/// <summary>
		///     Add a quad verbatim, without perturbing the positions.
		/// </summary>
		/// <param name="v1">First vertex position.</param>
		/// <param name="v2">Second vertex position.</param>
		/// <param name="v3">Third vertex position.</param>
		/// <param name="v4">Fourth vertex position.</param>
		public void AddQuadUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
		{
			int vertexIndex = _vertices.Count;
			_vertices.Add(v1);
			_vertices.Add(v2);
			_vertices.Add(v3);
			_vertices.Add(v4);
			_triangles.Add(vertexIndex);
			_triangles.Add(vertexIndex + 2);
			_triangles.Add(vertexIndex + 1);
			_triangles.Add(vertexIndex + 1);
			_triangles.Add(vertexIndex + 2);
			_triangles.Add(vertexIndex + 3);
		}

		/// <summary>
		///     Add UV coordinates for a quad.
		/// </summary>
		/// <param name="uv1">First UV coordinates.</param>
		/// <param name="uv2">Second UV coordinates.</param>
		/// <param name="uv3">Third UV coordinates.</param>
		/// <param name="uv4">Fourth UV coordinates.</param>
		public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
		{
			_uvs.Add(uv1);
			_uvs.Add(uv2);
			_uvs.Add(uv3);
			_uvs.Add(uv4);
		}

		/// <summary>
		///     Add UV2 coordinates for a quad.
		/// </summary>
		/// <param name="uv1">First UV2 coordinates.</param>
		/// <param name="uv2">Second UV2 coordinates.</param>
		/// <param name="uv3">Third UV2 coordinates.</param>
		/// <param name="uv4">Fourth UV2 coordinates.</param>
		public void AddQuadUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
		{
			_uv2S.Add(uv1);
			_uv2S.Add(uv2);
			_uv2S.Add(uv3);
			_uv2S.Add(uv4);
		}

		/// <summary>
		///     Add UV coordinates for a quad.
		/// </summary>
		/// <param name="uMin">Minimum U coordinate.</param>
		/// <param name="uMax">Maximum U coordinate.</param>
		/// <param name="vMin">Minimum V coordinate.</param>
		/// <param name="vMax">Maximum V coordinate.</param>
		public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
		{
			_uvs.Add(new Vector2(uMin, vMin));
			_uvs.Add(new Vector2(uMax, vMin));
			_uvs.Add(new Vector2(uMin, vMax));
			_uvs.Add(new Vector2(uMax, vMax));
		}

		/// <summary>
		///     Add UV2 coordinates for a quad.
		/// </summary>
		/// <param name="uMin">Minimum U2 coordinate.</param>
		/// <param name="uMax">Maximum U2 coordinate.</param>
		/// <param name="vMin">Minimum V2 coordinate.</param>
		/// <param name="vMax">Maximum V2 coordinate.</param>
		public void AddQuadUV2(float uMin, float uMax, float vMin, float vMax)
		{
			_uv2S.Add(new Vector2(uMin, vMin));
			_uv2S.Add(new Vector2(uMax, vMin));
			_uv2S.Add(new Vector2(uMin, vMax));
			_uv2S.Add(new Vector2(uMax, vMax));
		}

		/// <summary>
		///     Add cell data for a quad.
		/// </summary>
		/// <param name="indices">Terrain type indices.</param>
		/// <param name="weights1">First terrain weights.</param>
		/// <param name="weights2">Second terrain weights.</param>
		/// <param name="weights3">Third terrain weights.</param>
		/// <param name="weights4">Fourth terrain weights.</param>
		public void AddQuadCellData(
			Vector3 indices,
			Color weights1, Color weights2, Color weights3, Color weights4
		)
		{
			_cellIndices.Add(indices);
			_cellIndices.Add(indices);
			_cellIndices.Add(indices);
			_cellIndices.Add(indices);
			_cellWeights.Add(weights1);
			_cellWeights.Add(weights2);
			_cellWeights.Add(weights3);
			_cellWeights.Add(weights4);
		}

		/// <summary>
		///     Add cell data for a quad.
		/// </summary>
		/// <param name="indices">Terrain type indices.</param>
		/// <param name="weights1">First and second terrain weights, both the same.</param>
		/// <param name="weights2">Third and fourth terrain weights, both the same.</param>
		public void AddQuadCellData(Vector3 indices, Color weights1, Color weights2)
		{
			AddQuadCellData(indices, weights1, weights1, weights2, weights2);
		}

		/// <summary>
		///     Add cell data for a quad.
		/// </summary>
		/// <param name="indices">Terrain type indices.</param>
		/// <param name="weights">Terrain weights, uniform for entire quad.</param>
		public void AddQuadCellData(Vector3 indices, Color weights)
		{
			AddQuadCellData(indices, weights, weights, weights, weights);
		}
	}
}