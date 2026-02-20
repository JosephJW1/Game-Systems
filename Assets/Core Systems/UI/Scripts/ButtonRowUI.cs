using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
public class ButtonRowUI : RowUI
{
    private Button button;
    private object item;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Initialize(UnityAction<object> action)
    {
        button.onClick.AddListener(() => action(item));
    }

    // NEW: Added isInteractable parameter
    public void UpdateItem(object item, bool isInteractable = true)
    {
        this.item = item;
        if (button != null)
        {
            button.interactable = isInteractable;
        }
    }
}