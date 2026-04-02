// BuildingPlacer.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer Instance { get; private set; }

    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    private BuildingData _pending;
    private GameObject _ghost;

    void Awake() => Instance = this;

    public void StartPlacing(BuildingData data)
    {
        CancelPlacing();
        _pending = data;
        _ghost = Instantiate(data.prefab);
        DisableGhostComponents(_ghost);
        SetGhostMaterial(true);
    }

    void Update()
    {
        if (_pending == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelPlacing();
            return;
        }

        var cell = GetCellUnderMouse();
        _ghost.transform.position = GridSystem.Instance.CellToWorld(cell);

        bool valid = CanPlace(cell);
        SetGhostMaterial(valid);

        if (Mouse.current.leftButton.wasPressedThisFrame && valid)
            PlaceBuilding(cell);
    }

    private bool CanPlace(Vector2Int cell) =>
        GridSystem.Instance.IsAreaUnlocked(cell, _pending.size) &&
        GridSystem.Instance.IsAreaFree(cell, _pending.size);

    private void PlaceBuilding(Vector2Int cell)
    {
        var go = Instantiate(
            _pending.prefab,
            GridSystem.Instance.CellToWorld(cell),
            Quaternion.identity);

        var b = go.GetComponent<Building>();
        b.Initialize(_pending, cell);
        GridSystem.Instance.Occupy(cell, _pending.size, b);

        CancelPlacing();
    }

    public void CancelPlacing()
    {
        if (_ghost != null) Destroy(_ghost);
        _ghost = null;
        _pending = null;
    }

    private Vector2Int GetCellUnderMouse()
    {
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out var hit))
            return GridSystem.Instance.WorldToCell(hit.point);
        return Vector2Int.zero;
    }

    private void SetGhostMaterial(bool valid)
    {
        foreach (var r in _ghost.GetComponentsInChildren<Renderer>())
            r.material = valid ? validMaterial : invalidMaterial;
    }

    private void DisableGhostComponents(GameObject go)
    {
        foreach (var c in go.GetComponentsInChildren<Collider>())
            c.enabled = false;
    }
}