using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

namespace Sketch4 {

sealed class GeometryCache : System.IDisposable
{
    public NativeArray<float3> Vertices;
    public NativeArray<uint> Indices;

    public void Dispose()
    {
        if (Vertices.IsCreated) Vertices.Dispose();
        if (Indices.IsCreated) Indices.Dispose();
    }

    public GeometryCache(Mesh mesh)
    {
        Vertices = Util.AllocateBuffer<float3>((uint)mesh.vertexCount);
        Indices = Util.AllocateBuffer<uint>((uint)mesh.GetIndexCount(0));
        mesh.GetVertices(Vertices);
        mesh.GetIndices(Indices, 0);
    }
}

} // namespace Sketch4
