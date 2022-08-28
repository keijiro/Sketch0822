using UnityEngine;
using System.Collections.Generic;

namespace Sketch4 {

sealed class Strands : MonoBehaviour
{
    [SerializeField] Mesh _shape = null;

    Mesh _mesh;

    void Start()
    {
        var shape = new GeometryCache(_shape);

        var stack = new Stack<Modeler>();

        for (var i = 0; i < 10; i++)
        {
            var m = new Modeler()
              { Position = Vector3.forward * 0.1f * i,
                Rotation = 30 * i, Shape = shape };
            stack.Push(m);
        }

        GetComponent<MeshFilter>().sharedMesh = _mesh = MeshBuilder.Build(stack);
    }

    void OnDestroy()
      => Util.DestroyObject(_mesh);
}

} // namespace Sketch4
