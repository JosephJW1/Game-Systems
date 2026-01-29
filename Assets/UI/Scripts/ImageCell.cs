using UnityEngine;
using UnityEngine.UI;

public class ImageCell : BaseCell
{
    [SerializeField] private Image imageComponent;

    public override void SetValue(object value)
    {
        if (imageComponent != null)
        {
            Sprite spriteValue = value as Sprite;
            imageComponent.sprite = spriteValue;
            imageComponent.enabled = (spriteValue != null);
        }
    }
}