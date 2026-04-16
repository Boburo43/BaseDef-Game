using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject panel;
    [SerializeField] private Button expandButton;


    private void OnEnable() => GameModeManager.OnModeChanged += OnGameModeChanged;
    private void OnDisable() => GameModeManager.OnModeChanged -= OnGameModeChanged;


    void Start()
    {
        foreach (var data in BuildingRegistry.Instance.GetAll())
            CreateButton(data);

        expandButton.onClick.AddListener(ToogleExpand);
    }

    private void OnGameModeChanged(GameMode newMode)
    {
        if (newMode == GameMode.Build)
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
            BuildingPlacer.Instance.CancelPlacing();
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
        });
    }

    void ToogleExpand()
    {
        GridExpansionManager.Instance.isExpanding = !GridExpansionManager.Instance.isExpanding;
        expandButton.image.color = GridExpansionManager.Instance.isExpanding ? Color.green : Color.white;
        Debug.Log("Grid Expansion " + (GridExpansionManager.Instance.isExpanding ? "Enabled" : "Disabled"));

    }
}