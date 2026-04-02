using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject panel;

    void Start()
    {
        foreach (var data in BuildingRegistry.Instance.GetAll())
            CreateButton(data);
    }

    void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
            panel.SetActive(!panel.activeSelf);

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            BuildingPlacer.Instance.CancelPlacing();
            panel.SetActive(false);
        }
    }

    void CreateButton(BuildingData data)
    {
        var go = Instantiate(buttonPrefab, buttonContainer);
        var button = go.GetComponent<Button>();
        var icon = go.transform.Find("Icon")?.GetComponent<Image>();
        var label = go.transform.Find("Label")?.GetComponent<TMP_Text>();

        if (icon != null && data.icon != null) icon.sprite = data.icon;
        if (label != null) label.text = data.displayName;

        button.onClick.AddListener(() =>
        {
            BuildingPlacer.Instance.StartPlacing(data);
            panel.SetActive(false);
        });
    }
}