using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Sketch4 {

sealed class Modeler
{
    #region Model properties

    public float3 Position { get; set; }
    public float Rotation { get; set; }
    public GeometryCache Shape { get; set; }

    #endregion

    #region Private utility properties

    public uint VertexCount => (uint)Shape.Vertices.Length;
    public uint IndexCount => (uint)Shape.Indices.Length;

    #endregion

    #region Public methods

    public void BuildGeometry(NativeSlice<float3> vertices,
                              NativeSlice<uint> indices,
                              uint indexOffset)
    {
        CopyVertices(vertices);
        CopyIndices(indices, indexOffset);
    }

    #endregion

    #region Builder methods

    void CopyVertices(NativeSlice<float3> dest)
    {
        for (var i = 0; i < Shape.Vertices.Length; i++)
            dest[i] = Shape.Vertices[i];
    }

    void CopyIndices(NativeSlice<uint> dest, uint offs)
    {
        for (var i = 0; i < Shape.Indices.Length; i++)
            dest[i] = Shape.Indices[i] + offs;
    }

    #endregion
}

} // namespace Sketch4
