using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;

namespace Sketch3 {

static class MeshBuilder
{
    static NativeArray<T> AllocateBuffer<T>(uint length) where T : unmanaged
      => new NativeArray<T>((int)length, Allocator.Persistent,
                            NativeArrayOptions.UninitializedMemory);

    public static Mesh Build(IEnumerable<Modeler> modelers)
    {
        var (vcount, icount) = (0u, 0u);
        foreach (var m in modelers)
        {
            vcount += m.VertexCount;
            icount += m.IndexCount;
        }

        using var vbuf = AllocateBuffer<float3>(vcount);
        using var ibuf = AllocateBuffer<uint>(icount);

        var (voffs, ioffs) = (0u, 0u);
        foreach (var m in modelers)
        {
            var (vc, ic) = (m.VertexCount, m.IndexCount);

            var vslice = new NativeSlice<float3>(vbuf, (int)voffs, (int)vc);
            var islice = new NativeSlice<uint>(ibuf, (int)ioffs, (int)ic);

            m.BuildGeometry(vslice, islice, voffs);

            voffs += vc;
            ioffs += ic;
        }

        var mesh = new Mesh();
        mesh.SetVertices(vbuf);
        mesh.SetIndices(ibuf, MeshTopology.Triangles, 0);
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}

} // namespace Sketch3
