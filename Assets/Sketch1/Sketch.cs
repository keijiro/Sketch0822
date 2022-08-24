using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;
using Random = Unity.Mathematics.Random;

namespace Sketch1 {

sealed class Sketch : MonoBehaviour
{
    [SerializeField] Mesh _mesh = null;
    [SerializeField] Material _material = null;
    [SerializeField] uint2 _dimensions = math.uint2(10, 10);
    [SerializeField] uint _seed = 123;

    void Start()
    {
        var rand = new Random(_seed);
        rand.NextUInt4();

        var comps = new [] { typeof(MeshFilter), typeof(MeshRenderer) };

        for (var i_z = 0; i_z < _dimensions.y; i_z++)
        {
            var z = i_z - _dimensions.y / 2 + 0.5f;

            for (var i_x = 0; i_x < _dimensions.x; i_x++)
            {
                var x = i_x - _dimensions.x / 2 + 0.5f;
                var y = rand.NextFloat(0.0f, 0.3f) * (0.7f + math.abs(z)) - 5;

                var go = new GameObject("Box", comps);
                go.transform.parent = transform;
                go.transform.localPosition = new Vector3(x, y, z);
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                go.GetComponent<MeshFilter>().mesh = _mesh;
                go.GetComponent<MeshRenderer>().material = _material;
            }
        }
    }
}

} // namespace Sketch
