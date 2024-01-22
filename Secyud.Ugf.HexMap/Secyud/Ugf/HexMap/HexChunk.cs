using Secyud.Ugf.HexUtilities;
using UnityEngine;

namespace Secyud.Ugf.HexMap
{
    public class HexChunk : MonoBehaviour
    {
        public readonly int[] Cells = new int[HexMetrics.ChunkSizeX * HexMetrics.ChunkSizeZ];

        [SerializeField] private Canvas Canvas;
        [SerializeField] private HexMesh[] Meshes;
        [SerializeField] private Transform[] Features;

        public Transform Container { get; private set; }
        public HexGrid Grid { get; private set; }
        public HexMesh Get(int index) => Meshes[index];
        public Transform GetFeature(int index) => Features[index];

        private void LateUpdate()
        {
            Triangulate();
            enabled = false;
        }

        public void ShowUI(bool visible)
        {
            Canvas.gameObject.SetActive(visible);
        }

        public void Refresh()
        {
            enabled = true;
        }

        private void Triangulate()
        {
            foreach (HexMesh mesh in Meshes)
            {
                if (mesh)
                {
                    mesh.Clear();
                }
            }
            
            if (Container) Destroy(Container.gameObject);
            Container = new GameObject("Features Container").transform;
            Container.SetParent(transform, false);

            Grid.HexGridDrawer.TriangulateChunk(this);

            foreach (HexMesh mesh in Meshes)
            {
                if (mesh)
                {
                    mesh.Apply();
                }
            }
        }

        public void Initialize(HexGrid grid, int x, int z)
        {
            Grid = grid;

            for (int j = 0; j < HexMetrics.ChunkSizeZ; j++)
            for (int i = 0; i < HexMetrics.ChunkSizeX; i++)
            {
                HexCell cell = grid.HexGridDrawer.CreateCell() ;
                RectTransform uiRect
                    = Instantiate(grid.UiTemplate, Canvas.transform, false);

                cell.Initialize(uiRect, this,
                    i + x * HexMetrics.ChunkSizeX,
                    j + z * HexMetrics.ChunkSizeZ);
                
                Cells[j * HexMetrics.ChunkSizeX + i] = cell.Index;
            }
        }

        public void CreateMap()
        {
            foreach (int index in Cells)
            {
                Grid.GetCell(index).CreateMap();
            }
        }
        
    }
}