using UnityEngine;
using System;

namespace Sketch4 {

sealed class Modeler
{
    #region Model properties

    public Vector3 Position { get; set; }
    public float Rotation { get; set; }
    public GeometryCache Shape { get; set; }

    #endregion

    #region Private utility properties

    public int VertexCount => Shape.Vertices.Count;
    public int IndexCount => Shape.Indices.Count;

    #endregion

    #region Public methods

    public void BuildGeometry(Span<Vector3> vertices,
                              Span<int> indices,
                              int indexOffset)
    {
        CopyVertices(vertices);
        CopyIndices(indices, indexOffset);
    }

    #endregion

    #region Builder methods

    void CopyVertices(Span<Vector3> dest)
    {
        var rot = Quaternion.AngleAxis(Rotation, Vector3.forward);
        var mtx = Matrix4x4.TRS(Position, rot, Vector3.one);
        for (var i = 0; i < Shape.Vertices.Count; i++)
            dest[i] = (Vector3)(mtx * Shape.Vertices[i]) + Position;
    }

    void CopyIndices(Span<int> dest, int offs)
    {
        for (var i = 0; i < Shape.Indices.Count; i++)
            dest[i] = Shape.Indices[i] + offs;
    }

    #endregion
}

} // namespace Sketch4
