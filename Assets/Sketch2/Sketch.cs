using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;
using Random = Unity.Mathematics.Random;

namespace Sketch2 {

sealed class Sketch : MonoBehaviour
{
    [SerializeField] Mesh _mesh = null;
    [SerializeField] Material _material = null;
    [SerializeField] uint2 _dimensions = math.uint2(10, 10);
    [SerializeField] float _displacement = 1;
    [SerializeField] float _steepness = 1;
    [SerializeField] uint _seed = 123;

    void Start()
    {
        var rand = new Random(_seed);
        rand.NextUInt4();

        var comps = new [] { typeof(MeshFilter), typeof(MeshRenderer) };

        for (var i_y = 0; i_y < _dimensions.y; i_y++)
        {
            var y = i_y - _dimensions.y / 2 + 0.5f;

            for (var i_x = 0; i_x < _dimensions.x; i_x++)
            {
                var x = i_x - _dimensions.x / 2 + 0.5f;

                var z = 1 / (1 + math.length(math.float2(x, y)) * _steepness);
                z *= _displacement * rand.NextFloat(0.3f, 1f);

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
