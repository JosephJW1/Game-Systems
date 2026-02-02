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

    public void UpdateItem(object item)
    {
        this.item = item;
    }
}