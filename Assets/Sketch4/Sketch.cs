using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Sketch4 {

[ExecuteInEditMode]
sealed class Sketch : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] int _poleCount = 5;
    [SerializeField] float _baseRange = 10;
    [SerializeField] float _nodeStride = 0.1f;
    [SerializeField, Range(0, 1)] float _emissionRate = 0.5f;
    [SerializeField] Color _emissionColor1 = Color.white;
    [SerializeField] Color _emissionColor2 = Color.white;
    [SerializeField] float _emissionIntensity = 10;
    [SerializeField] Mesh _boardMesh = null;
    [SerializeField] Mesh _poleMesh = null;
    [SerializeField] uint _seed = 1234;

    #endregion

    #region Modeling

    (GeometryCache board, GeometryCache pole) _shapes;
    Stack<Modeler> _modelers;

    void AddModelers()
    {
        // PRNG
        var (hash, seed) = (new XXHash(_seed), 0u);

        // Pole population
        for (var i = 0; i < _poleCount; i++)
        {
            // Position / angle
            var pos = math.float3(hash.InCircle(seed++) * _baseRange, 0);
            var angle = hash.Bool(seed++) ? 0 : 45;

            // Emitter
            var emitter = hash.Float(seed++) < _emissionRate;
            var ecolor = hash.Bool(seed++) ? _emissionColor1 : _emissionColor2;
            ecolor *= _emissionIntensity;

            // Probability decay coefficient
            var decay = hash.Float(0.1f, 0.96f, seed++);

            for (var prob = 1.0f; prob > 0.2f;)
            {
                for (var k = 0; k < 4; k++)
                {
                    // Rotation
                    var rot = float2x2.Rotate(math.radians(angle));

                    // Board
                    var d1 = math.float2(_nodeStride * 0.5f, 0);
                    var p1 = pos + math.float3(math.mul(rot, d1), 0);

                    // Pole
                    var d2 = (float2)(_nodeStride * 0.5f);
                    var p2 = pos + math.float3(math.mul(rot, d2), 0);

                    // Modeler addition
                    if (hash.Float(seed++) < prob)
                        _modelers.Push(new Modeler()
                          { Position = p1, Rotation = angle, Shape = _shapes.board });

                    _modelers.Push(new Modeler()
                      { Position = p2, Rotation = angle, Shape = _shapes.pole });

                    if (emitter)
                        _modelers.Push(new Modeler()
                          { Position = pos, Color = ecolor, Shape = _shapes.pole });

                    // Rotation advance
                    angle += 90;
                }

                // Stride
                pos.z += _nodeStride;

                // Probability decay
                prob *= hash.Float(decay, 1.0f, seed++);
            }

            // Emitter extension
            if (emitter)
            {
                var ext = hash.Int(10, seed++);
                for (var j = 0; j < ext; j++)
                {
                    _modelers.Push(new Modeler()
                      { Position = pos, Color = ecolor, Shape = _shapes.pole });
                    pos.z += _nodeStride;
                }
            }
        }
    }

    #endregion

    #region MonoBehaviour implementation

    Mesh _mesh;

    void Start()
    {
        // Geometry cache
        _shapes.board = new GeometryCache(_boardMesh);
        _shapes.pole = new GeometryCache(_poleMesh);

        // Temporary mesh object
        _mesh = new Mesh();
        _mesh.hideFlags = HideFlags.DontSave;
        GetComponent<MeshFilter>().sharedMesh = _mesh;

        // Initial mesh construction
        _modelers = new Stack<Modeler>();
        AddModelers();
        MeshBuilder.Build(_mesh, _modelers);
    }

    void OnValidate()
    {
        if (_mesh == null || _modelers == null) return;

        // Clear
        _modelers.Clear();
        _mesh.Clear();

        // Reconstruction
        AddModelers();
        MeshBuilder.Build(_mesh, _modelers);
    }

    void OnDestroy()
      => Util.DestroyObject(_mesh);

    #endregion
}

} // namespace Sketch4
