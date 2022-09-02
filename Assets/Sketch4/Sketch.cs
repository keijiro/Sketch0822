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
    [SerializeField] Mesh _boardMesh = null;
    [SerializeField] Mesh _poleMesh = null;
    [SerializeField] uint _seed = 1234;

    Mesh _mesh;

    void Start()
    {
        // Meshes
        var board = new GeometryCache(_boardMesh);
        var pole = new GeometryCache(_poleMesh);

        // PRNG
        var (hash, seed) = (new XXHash(_seed), 0u);

        // Modeler stack
        var modeling = new Stack<Modeler>();

        // Pole population
        for (var i = 0; i < _poleCount; i++)
        {
            // Position / angle
            var pos = math.float3(hash.InCircle(seed++) * _baseRange, 0);
            var angle = hash.Bool(seed++) ? 0 : 45;

            // Probability decay coefficient
            var decay = hash.Float(0.1f, 0.96f, seed++);

            for (var prob = 1.0f; prob > 0.3f;)
            {
                for (var k = 0; k < 4; k++)
                {
                    // Rotation
                    angle += 90;

                    // Probability test
                    if (hash.Float(seed++) > prob) continue;

                    // Node position
                    var d = math.float2(_nodeStride * 0.5f, 0);
                    d = math.mul(float2x2.Rotate(math.radians(angle)), d);
                    var p = pos + math.float3(d, 0);

                    // Modeler addition
                    modeling.Push(new Modeler()
                      { Position = p, Rotation = angle, Shape = board });
                }

                // Stride
                pos.z += _nodeStride;

                // Probability decay
                prob *= hash.Float(decay, 1.0f, seed++);
            }
        }

        // Mesh building
        _mesh = MeshBuilder.Build(modeling);
        GetComponent<MeshFilter>().sharedMesh = _mesh;
    }

    void OnDestroy()
      => Util.DestroyObject(_mesh);
}

} // namespace Sketch4
