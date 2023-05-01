#define OUTER_TO_INNER 0.866025404
#define OUTER_RADIUS 10
#define CHUNK_SIZE_X 5
#define TILING_SCALE (1 / (CHUNK_SIZE_X * 2 * OUTER_RADIUS / OUTER_TO_INNER))

// Convert from 2D world space to hex space,
// where the distance between cell centers of east-west neighbors is one unit.
float2 WoldToHexSpace(const float2 p)
{
    return p * (1.0 / (2.0 * OUTER_RADIUS * OUTER_TO_INNER));
}
