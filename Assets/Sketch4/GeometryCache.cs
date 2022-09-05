using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using System.Collections.Generic;

namespace Sketch4 {

sealed class GeometryCache : System.IDisposable
{
    public NativeArray<float3> Vertices;
    public NativeArray<uint> Indices;

    public GeometryCache(Mesh mesh)
    {
        Vertices = new NativeArray<Vector3>(mesh.vertices, Allocator.Persistent).Reinterpret<float3>();
        Indices = new NativeArray<int>(mesh.triangles, Allocator.Persistent).Reinterpret<uint>();
    }

    public void Dispose()
    {
        if (Vertices.IsCreated) Vertices.Dispose();
        if (Indices.IsCreated) Indices.Dispose();
    }
}

readonly struct GeometryCacheRef
{
    public readonly NativeSlice<float3> Vertices;
    public readonly NativeSlice<uint> Indices;

    public static implicit operator GeometryCacheRef(GeometryCache cache)
      => new GeometryCacheRef(cache);

    public GeometryCacheRef(GeometryCache geo)
    {
        Vertices = new NativeSlice<float3>(geo.Vertices);
        Indices = new NativeSlice<uint>(geo.Indices);
    }
}

} // namespace Sketch4
