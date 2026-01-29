using UnityEngine;
using TMPro;

public class TextCell : BaseCell
{
    [SerializeField] private TextMeshProUGUI textLabel;

    public override void SetValue(object value)
    {
        if (textLabel != null)
        {
            textLabel.text = value != null ? value.ToString() : "null";
        }
    }

    public override void SetAlignment(TextAlignmentOptions alignment)
    {
        if (textLabel != null)
        {
            textLabel.alignment = alignment;
        }
    }
}