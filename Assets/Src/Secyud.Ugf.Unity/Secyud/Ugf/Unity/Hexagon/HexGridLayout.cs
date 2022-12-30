using UnityEngine;

namespace Secyud.Ugf.Unity.Hexagon
{
    public class HexGridLayout:MonoBehaviour
    {
        [Header("GridSetting")] public Vector2Int GridSize;

        [Header("Tile Settings")] public float OuterSize = 1f;
        public float InnerSize = 0f;
        public float Height = 1f;
        public bool IsFlatTopped;
        public Material Material;

        private void OnEnable()
        {
            LayoutGrid();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
                
                LayoutGrid();
            }
        }

        private void LayoutGrid()
        {
            for (var y = 0; y < GridSize.y; y++)
            {
                for (var x = 0; x< GridSize.x; x++)
                {
                    var tile = new GameObject(
                        $"Hex {x},{y}",
                        typeof(HexRenderer))
                    {
                        transform =
                        {
                            position = GetPositionForHexFromCoordinate(new Vector2Int(x, y))
                        }
                    };

                    var hexRenderer = tile.GetComponent<HexRenderer>();
                    hexRenderer.IsFlapTopped = IsFlatTopped;
                    hexRenderer.OuterSize = OuterSize;
                    hexRenderer.InnerSize = InnerSize;
                    hexRenderer.Height = Height;
                    hexRenderer.SetMaterial(Material);
                    hexRenderer.DrawMesh();
                    
                    tile.transform.SetParent(transform,true);
                }
            }
        }

        public Vector3 GetPositionForHexFromCoordinate(Vector2Int coordinate)
        {
            int col = coordinate.x;
            int row = coordinate.y;

            float width, height, xPosition, yPosition;
            bool shouldOffset;
            float horizontalDistance,verticalDistance;
            float offset, size = OuterSize;

            if (IsFlatTopped)
            {
                shouldOffset = col % 2 == 0;
                width =2f * size ;
                height =Mathf.Sqrt(3) * size ;

                horizontalDistance = width * 3f / 4f;
                verticalDistance = height;

                offset = shouldOffset ? height / 2 : 0;

                xPosition = col * horizontalDistance ;
                yPosition = row * verticalDistance- offset;
            }
            else
            {
                shouldOffset = row % 2 == 0;
                width = Mathf.Sqrt(3) * size;
                height = 2f * size;

                horizontalDistance = width;
                verticalDistance = height * 3f / 4f;

                offset = shouldOffset ? width / 2 : 0;

                xPosition = col * horizontalDistance + offset;
                yPosition = row * verticalDistance;

            }

            return new Vector3(xPosition, 0, -yPosition);
        }
    }
}