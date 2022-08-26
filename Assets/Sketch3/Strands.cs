using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;
using Random = Unity.Mathematics.Random;

namespace Sketch3 {

sealed class Strands : MonoBehaviour
{
    [SerializeField] uint _count = 10;
    [SerializeField] float _length = 20;
    [SerializeField] float _frequency = 5;
    [SerializeField] float _extent = 1;
    [SerializeField] float _shrink = 1;
    [SerializeField] float _radius = 0.02f;
    [SerializeField] uint _seed = 123;

    Mesh _mesh;

    void Start()
    {
        var rand = new Random(_seed);
        rand.NextUInt4();

        var stack = new Stack<Modeler>();

        for (var i = 0u; i < _count; i++)
        {
            var m = new Modeler()
              { Extent = _extent + rand.NextFloat(-0.2f, 0.2f),
                Length = _length, Radius = _radius, Shrink = _shrink,
                Frequency = _frequency + rand.NextFloat(-1.0f, 1.0f) };
            stack.Push(m);
        }

        GetComponent<MeshFilter>().sharedMesh = _mesh = MeshBuilder.Build(stack);
    }

    void OnDestroy()
      => Util.DestroyObject(_mesh);
}

} // namespace Sketch3
