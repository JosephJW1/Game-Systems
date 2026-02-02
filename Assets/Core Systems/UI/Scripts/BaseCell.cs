using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
public abstract class BaseCell : MonoBehaviour
{
    protected LayoutElement layoutElement;

    protected virtual void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
    }

    public abstract void SetValue(object value);

    public virtual void SetAlignment(TextAlignmentOptions alignment) { }

    public void ConfigureLayout(float layoutRatio)
    {
        if (layoutElement == null) layoutElement = GetComponent<LayoutElement>();
        layoutElement.minWidth = 0;
        layoutElement.preferredWidth = 0;
        layoutElement.flexibleWidth = (layoutRatio > 0) ? layoutRatio : 1f;
    }
}