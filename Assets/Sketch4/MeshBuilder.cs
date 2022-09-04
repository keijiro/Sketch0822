using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;

namespace Sketch4 {

static class MeshBuilder
{
    public static void Build(Mesh mesh, IEnumerable<Modeler> modelers)
    {
        var (vcount, icount) = (0, 0);
        foreach (var m in modelers)
        {
            vcount += m.VertexCount;
            icount += m.IndexCount;
        }

        var vbuf = new Vector3[vcount];
        var cbuf = new Vector4[vcount];
        var ibuf = new int[icount];

        var (voffs, ioffs) = (0, 0);
        foreach (var m in modelers)
        {
            var (vc, ic) = (m.VertexCount, m.IndexCount);

            var vspan = new Span<Vector3>(vbuf, voffs, vc);
            var cspan = new Span<Vector4>(cbuf, voffs, vc);
            var ispan = new Span<int>(ibuf, ioffs, ic);

            m.BuildGeometry(vspan, cspan, ispan, voffs);

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
