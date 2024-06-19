using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] string defaultTooltipText = "Tooltip Text";
    private TextMeshProUGUI tooltipText;
    private string initialText;

    private void Start()
    {
        tooltipText = GetComponent<TextMeshProUGUI>();
        initialText = tooltipText.text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        initialText = tooltipText.text;
        tooltipText.text = defaultTooltipText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipText.text = initialText;
    }
}
