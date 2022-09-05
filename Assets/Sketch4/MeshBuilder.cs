using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using System;
using System.Collections.Generic;

namespace Sketch4 {

static class MeshBuilder
{
    static NativeArray<T> NewBuffer<T>(int length) where T : unmanaged
      => new NativeArray<T>(length, Allocator.Persistent,
                            NativeArrayOptions.UninitializedMemory);

    unsafe static Span<T>
      GetSpan<T>(this NativeArray<T> array) where T : unmanaged
        => new Span<T>(NativeArrayUnsafeUtility.GetUnsafePtr(array), array.Length);

    unsafe public static void Build(Mesh mesh, IEnumerable<Modeler> modelers)
    {
        var (vcount, icount) = (0, 0);
        foreach (var m in modelers)
        {
            vcount += m.VertexCount;
            icount += m.IndexCount;
        }

        using var vbuf = NewBuffer<float3>(vcount);
        using var cbuf = NewBuffer<float4>(vcount);
        using var ibuf = NewBuffer<uint>(icount);

        var vspan = vbuf.GetSpan();
        var cspan = cbuf.GetSpan();
        var ispan = ibuf.GetSpan();

        var (voffs, ioffs) = (0, 0);
        foreach (var m in modelers)
        {
            var (vc, ic) = (m.VertexCount, m.IndexCount);

            var vslice = vspan.Slice(voffs, vc);
            var cslice = cspan.Slice(voffs, vc);
            var islice = ispan.Slice(ioffs, ic);

            m.BuildGeometry(vslice, cslice, islice, (uint)voffs);

            voffs += vc;
            ioffs += ic;
        }

        mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(vbuf);
        mesh.SetUVs(0, cbuf);
        mesh.SetIndices(ibuf, MeshTopology.Triangles, 0);
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}

} // namespace Sketch4
