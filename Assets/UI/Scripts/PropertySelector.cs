using UnityEngine;

[System.Serializable]
public class PropertySelector
{
    [Header("Display")]
    public string propertyName;
    public float layoutRatio = 1f;

    [Header("Header & Label")]
    public string label;
    public bool centerHeader;

    [Header("Values")]
    public bool centerValues;
}