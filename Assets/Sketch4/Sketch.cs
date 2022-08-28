using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Sketch4 {

sealed class Strands : MonoBehaviour
{
    [SerializeField] int _poleCount = 5;
    [SerializeField] float _baseRange = 10;
    [SerializeField] int _nodeCount = 5;
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
            var pos = math.float3(rand.NextFloat2(-0.5f, 0.5f) * _baseRange, 0);

            for (var j = 0; j < _nodeCount; j++)
            {
                var m = new Modeler()
                  { Position = pos, Rotation = 30 * j, Shape = shape };

                stack.Push(m);

                pos += math.float3(0, 0, _nodeStride);
            }
        }

        GetComponent<MeshFilter>().sharedMesh = _mesh = MeshBuilder.Build(stack);
    }

    void OnDestroy()
      => Util.DestroyObject(_mesh);
}

} // namespace Sketch4
