// PlacementDebug.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementDebug : MonoBehaviour
{
    [SerializeField] private BuildingData[] buildings;

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame && buildings.Length > 0)
            BuildingPlacer.Instance.StartPlacing(buildings[0]);

        if (Keyboard.current.digit2Key.wasPressedThisFrame && buildings.Length > 1)
            BuildingPlacer.Instance.StartPlacing(buildings[1]);

        if (Keyboard.current.digit3Key.wasPressedThisFrame && buildings.Length > 2)
            BuildingPlacer.Instance.StartPlacing(buildings[2]);
    }
}