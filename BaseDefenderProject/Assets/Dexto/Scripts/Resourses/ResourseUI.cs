using UnityEngine;
using TMPro;

public class ResourseUI : MonoBehaviour
{
    [SerializeField] private ResourceType DisplayType;
    [SerializeField] private TMP_Text text;

    private void OnEnable() => ResourseManager.OnResourseAmountChanged += HandleChange;
    private void OnDisable() => ResourseManager.OnResourseAmountChanged -= HandleChange;


    private void HandleChange(ResourceType type, int Amount)
    {
        if(type == DisplayType)
        {
            text.text = Amount.ToString();
        }

    }
}
