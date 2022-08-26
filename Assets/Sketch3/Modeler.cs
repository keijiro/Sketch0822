using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Sketch3 {

sealed class Modeler
{
    #region Model properties

    public float3 Origin { get; set; } = 0;
    public float Length { get; set; } = 10;
    public float Frequency { get; set; } = 5;
    public float Extent { get; set; } = 0.02f;
    public float Shrink { get; set; } = 1;
    public float Radius { get; set; } = 1;
    public uint Resolution { get; set; } = 1024;

    #endregion

    #region Private utility properties

    public uint VertexCount => Resolution * 4;
    public uint IndexCount => (Resolution - 1) * 4 * 2 * 3;

    #endregion

    #region Public methods

    public void BuildGeometry(NativeSlice<float3> vertices,
                              NativeSlice<uint> indices,
                              uint indexOffset)
    {
        BuildVertexArray(vertices);
        BuildIndexArray(indices, indexOffset);
    }

    #endregion

    #region Builder methods

    float3 GetPointOnLine(float z)
    {
        var phi = math.PI * 2 * Frequency * z;
        var r = math.float2(math.cos(phi), math.sin(phi));
        r *= Extent / (1 + z * Shrink);
        return math.float3(r, z * Length);
    }

    void BuildVertexArray(NativeSlice<float3> buffer)
    {
        var vc = 0;
        for (var i = 0; i < Resolution; i++)
        {
            var p0 = GetPointOnLine((i - 0.1f) / (Resolution - 1));
            var p1 = GetPointOnLine((i + 0.0f) / (Resolution - 1));
            var p2 = GetPointOnLine((i + 0.1f) / (Resolution - 1));
            var v1 = Radius * math.normalize(math.cross(p2 - p0, math.float3(0, 0, 1)));
            var v2 = Radius * math.normalize(math.cross(v1, p2 - p0));
            buffer[vc++] = p1 - v1;
            buffer[vc++] = p1 + v2;
            buffer[vc++] = p1 + v1;
            buffer[vc++] = p1 - v2;
        }
    }

    void BuildIndexArray(NativeSlice<uint> buffer, uint offs)
    {
        var ic = 0;
        for (var i = 0u; i < Resolution - 1; i++)
        {
            for (var j = 0u; j < 4u; j++)
            {
                var i1 = offs + i * 4 + j;
                var i2 = offs + i * 4 + (j == 3 ? 0 : j + 1);
                buffer[ic++] = i1;
                buffer[ic++] = i2;
                buffer[ic++] = i1 + 4;
                buffer[ic++] = i2;
                buffer[ic++] = i2 + 4;
                buffer[ic++] = i1 + 4;
            }
        }
    }

    #endregion
}

} // namespace Sketch3
