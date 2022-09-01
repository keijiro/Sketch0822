using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Sketch4 {

sealed class Strands : MonoBehaviour
{
    [SerializeField] int _poleCount = 5;
    [SerializeField] float _baseRange = 10;
    [SerializeField] float _decay = 0.9f;
    [SerializeField] float _nodeStride = 0.1f;
    [SerializeField] Mesh _shape = null;
    [SerializeField] uint _seed = 1234;

    Mesh _mesh;

    void Start()
    {
        var shape = new GeometryCache(_shape);
        var stack = new Stack<Modeler>();

        var rand = new Random(_seed);
        rand.NextUInt4();

        for (var i = 0; i < _poleCount; i++)
        {
            var p0 = math.float3(rand.NextFloat2(-0.5f, 0.5f) * _baseRange, 0);
            var prob = 1.0f;
            var decay = rand.NextFloat(0.1f, 1.0f) * _decay;
            var diag = rand.NextFloat() < 0.5f;

            while (prob > 0.3f)
            {
                var arm = diag ? math.float2(0.707f, 0.707f) : math.float2(1, 0);

                for (var k = 0; k < 4; k++)
                {
                    var p1 = p0;
                    p1.xy += arm * _nodeStride / 2;

                    if (rand.NextFloat() < prob)
                        stack.Push(new Modeler()
                          { Position = p1, Rotation = 90 * k + (diag ? 45 : 0), Shape = shape });

                    arm = arm.yx * math.float2(-1, 1);
                }

                p0.z += _nodeStride;
                prob *= rand.NextFloat(decay, 1.0f);
            }
        }

        GetComponent<MeshFilter>().sharedMesh = _mesh = MeshBuilder.Build(stack);
    }

    void OnDestroy()
      => Util.DestroyObject(_mesh);
}

} // namespace Sketch4
