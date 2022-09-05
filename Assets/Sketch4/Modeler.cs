using UnityEngine;
using Unity.Mathematics;
using System;

namespace Sketch4 {

readonly struct Modeler
{
    #region Model properties

    readonly float3 _position;
    readonly float _rotation;
    readonly float4 _color;
    readonly GeometryCacheRef _shape;

    #endregion

    #region Private utility properties

    public int VertexCount => _shape.Vertices.Length;
    public int IndexCount => _shape.Indices.Length;

    #endregion

    #region Public methods

    public Modeler(Vector3 position,
                   float rotation,
                   Color color,
                   GeometryCacheRef shape)
    {
        _position = position;
        _rotation = rotation;
        _color = (Vector4)color;
        _shape = shape;
    }

    public void BuildGeometry(Span<float3> vertices,
                              Span<float4> uvs,
                              Span<uint> indices,
                              uint indexOffset)
    {
        CopyVertices(vertices);
        FillUVs(uvs);
        CopyIndices(indices, indexOffset);
    }

    #endregion

    #region Builder methods

    void CopyVertices(Span<float3> dest)
    {
        var rot = quaternion.RotateZ(_rotation);
        var mtx = float4x4.TRS(_position, rot, 1);
        for (var i = 0; i < _shape.Vertices.Length; i++)
            dest[i] = math.transform(mtx, _shape.Vertices[i]);// + _position;
    }

    void FillUVs(Span<float4> dest)
    {
        for (var i = 0; i < _shape.Vertices.Length; i++) dest[i] = _color;
    }

    void CopyIndices(Span<uint> dest, uint offs)
    {
        for (var i = 0; i < _shape.Indices.Length; i++)
            dest[i] = _shape.Indices[i] + offs;
    }

    #endregion
}

} // namespace Sketch4
