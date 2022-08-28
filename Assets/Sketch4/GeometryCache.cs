using UnityEngine;
using System.Collections.Generic;

namespace Sketch4 {

sealed class GeometryCache
{
    public List<Vector3> Vertices = new List<Vector3>();
    public List<int> Indices = new List<int>();

    public GeometryCache(Mesh mesh)
    {
        mesh.GetVertices(Vertices);
        mesh.GetIndices(Indices, 0);
    }
}

} // namespace Sketch4
