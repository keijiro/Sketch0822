using UnityEngine;
using System;

namespace Sketch4 {

[ExecuteInEditMode]
sealed class Sketch : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] SceneConfig _config = SceneConfig.Default();
    [SerializeField] Mesh _boardMesh = null;
    [SerializeField] Mesh _poleMesh = null;
    [SerializeField] uint _modelCapacity = 10000;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // Temporary mesh object
        _mesh = new Mesh();
        _mesh.hideFlags = HideFlags.DontSave;
        GetComponent<MeshFilter>().sharedMesh = _mesh;

        // Initial mesh construction
        ConstructMesh();
    }

    void OnValidate()
    {
        if (_mesh == null) return;

        // Clear
        _mesh.Clear();

        // Reconstruction
        ConstructMesh();
    }

    void OnDestroy()
      => Util.DestroyObject(_mesh);

    #endregion

    #region Private members

    Mesh _mesh;
    Modeler[] _modelers;

    void ConstructMesh()
    {
        if ((_modelers?.Length ?? 0) != _modelCapacity)
            _modelers = new Modeler[_modelCapacity];

        using var board = new GeometryCache(_boardMesh);
        using var pole = new GeometryCache(_poleMesh);

        var mcount = SceneBuilder.Build(_config, (board, pole), _modelers);

        MeshBuilder.Build(new Span<Modeler>(_modelers, 0, mcount), _mesh);
    }

    #endregion
}

} // namespace Sketch4
