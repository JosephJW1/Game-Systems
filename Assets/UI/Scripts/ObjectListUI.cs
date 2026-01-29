using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Linq.Expressions;

public class ObjectListUI : MonoBehaviour
{
    private class CachedProperty
    {
        public Func<object, object> Getter;
        public Type PropertyType;
    }

    [Header("Data Source")]
    [SerializeField] MonoBehaviour targetScript;
    [SerializeField] string listName;
    [SerializeField] List<PropertySelector> propertiesToLog;

    [Header("UI Prefabs")]
    [SerializeField] GameObject rowPrefab;
    [SerializeField] GameObject buttonRowPrefab;
    [SerializeField] GameObject textCellPrefab;
    [SerializeField] GameObject imageCellPrefab;
    [SerializeField] Transform rowContainer;

    [Header("Row Action")]
    [SerializeField] private UnityEvent<object> onRowClicked;

    private BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private List<RowUI> rowPool = new List<RowUI>();
    private IList sourceList;

    private List<CachedProperty> cachedProperties = new List<CachedProperty>();

    private bool isButtonRow;
    private GameObject activePrefab;

    void Start()
    {
        isButtonRow = onRowClicked.GetPersistentEventCount() > 0;

        if (isButtonRow)
        {
            if (buttonRowPrefab == null) { Debug.LogError("onRowClicked event is assigned, but Button Row Prefab is null!", this); this.enabled = false; return; }
            activePrefab = buttonRowPrefab;
        }
        else
        {
            activePrefab = rowPrefab;
        }

        if (!InitializeDataSource())
        {
            this.enabled = false;
            return;
        }

        CreateHeaderRow();
    }

    private bool InitializeDataSource()
    {
        if (targetScript == null) { Debug.LogError("Target Script is not assigned!", this); return false; }
        if (string.IsNullOrEmpty(listName)) { Debug.LogError("List Name is not specified!", this); return false; }

        Type scriptType = targetScript.GetType();
        object listObject = null;

        PropertyInfo listPropertyInfo = scriptType.GetProperty(listName, bindingFlags);
        FieldInfo listFieldInfo = scriptType.GetField(listName, bindingFlags);

        if (listPropertyInfo != null) { listObject = listPropertyInfo.GetValue(targetScript); }
        else if (listFieldInfo != null) { listObject = listFieldInfo.GetValue(targetScript); }
        else { Debug.LogError($"Could not find a property or field named '{listName}' on script '{scriptType.Name}'.", this); return false; }

        sourceList = listObject as IList;
        if (sourceList == null) { Debug.LogError($"The member '{listName}' is not a list (it does not implement IList).", this); return false; }

        Type itemType = null;
        Type listType = listObject.GetType();
        if (listType.IsGenericType)
        {
            itemType = listType.GetGenericArguments()[0];
        }
        else if (sourceList.Count > 0 && sourceList[0] != null)
        {
            itemType = sourceList[0].GetType();
        }
        else
        {
            Debug.LogWarning("List is empty and not a generic List<T>, cannot cache property paths yet. Will try again on Refresh.", this);
            return true;
        }

        CacheAllPaths(itemType);
        return true;
    }

    private CachedProperty BuildPropertyAccessors(Type itemType, string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return new CachedProperty { Getter = (item) => null, PropertyType = typeof(object) };
        }

        try
        {
            ParameterExpression param = Expression.Parameter(typeof(object), "item");
            Expression body = Expression.Convert(param, itemType);

            foreach (string part in path.Split('.'))
            {
                body = Expression.PropertyOrField(body, part);
            }

            Type finalType = body.Type;
            body = Expression.Convert(body, typeof(object));
            Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(body, param);

            return new CachedProperty
            {
                Getter = lambda.Compile(),
                PropertyType = finalType
            };
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to build getter for path '{path}' on type '{itemType.Name}'. Does the property exist? Error: {ex.Message}", this);
            return new CachedProperty { Getter = (item) => "<ERROR>", PropertyType = typeof(string) };
        }
    }

    private void CacheAllPaths(Type itemType)
    {
        cachedProperties.Clear();
        foreach (PropertySelector propSelector in propertiesToLog)
        {
            cachedProperties.Add(BuildPropertyAccessors(itemType, propSelector.propertyName));
        }
    }

    private void CreateHeaderRow()
    {
        if (propertiesToLog == null || propertiesToLog.Count == 0) return;
        if (rowPrefab == null || textCellPrefab == null) { Debug.LogWarning("Cannot create header row. Prefabs are not assigned.", this); return; }

        Transform container = (rowContainer != null) ? rowContainer : transform;
        GameObject headerRowInstance = Instantiate(rowPrefab, container);

        foreach (PropertySelector propSelector in propertiesToLog)
        {
            GameObject cellInstance = Instantiate(textCellPrefab, headerRowInstance.transform);
            TextCell textCell = cellInstance.GetComponent<TextCell>();

            if (textCell != null)
            {
                textCell.SetValue(propSelector.label);
                if (propSelector.centerHeader)
                {
                    textCell.SetAlignment(TextAlignmentOptions.Center);
                }
            }
            else { Debug.LogError("Text Cell Prefab does not have a TextCell component.", cellInstance); }

            BaseCell baseCell = cellInstance.GetComponent<BaseCell>();
            if (baseCell != null) { baseCell.ConfigureLayout(propSelector.layoutRatio); }
            else { Debug.LogError("Text Cell Prefab is missing the BaseCell component!", cellInstance); }
        }
    }

    public void RefreshUI()
    {
        if (sourceList == null) { return; }

        if (cachedProperties.Count != propertiesToLog.Count && sourceList.Count > 0 && sourceList[0] != null)
        {
            CacheAllPaths(sourceList[0].GetType());
        }
        if (cachedProperties.Count != propertiesToLog.Count) { return; }

        Transform container = (rowContainer != null) ? rowContainer : transform;

        for (int i = 0; i < sourceList.Count; i++)
        {
            object item = sourceList[i];
            RowUI rowInstance;

            if (i < rowPool.Count)
            {
                rowInstance = rowPool[i];
                rowInstance.gameObject.SetActive(true);
            }
            else
            {
                GameObject newRowGO = Instantiate(activePrefab, container);
                rowInstance = newRowGO.GetComponent<RowUI>();
                if (rowInstance == null) { Debug.LogError("Row Prefab is missing the RowUI component!", newRowGO); return; }

                if (isButtonRow)
                {
                    ((ButtonRowUI)rowInstance).Initialize(onRowClicked.Invoke);
                }

                for (int j = 0; j < propertiesToLog.Count; j++)
                {
                    BaseCell newCell = CreateCell(rowInstance.transform, cachedProperties[j], propertiesToLog[j].layoutRatio);
                    rowInstance.cells.Add(newCell);
                }
                rowPool.Add(rowInstance);
            }

            if (item == null)
            {
                rowInstance.gameObject.SetActive(false);
                continue;
            }

            if (isButtonRow)
            {
                ((ButtonRowUI)rowInstance).UpdateItem(item);
            }

            for (int j = 0; j < propertiesToLog.Count; j++)
            {
                PropertySelector propSelector = propertiesToLog[j];
                BaseCell cellToUpdate = rowInstance.cells[j];

                object displayValue = cachedProperties[j].Getter(item);
                cellToUpdate.SetValue(displayValue);

                if (cellToUpdate is TextCell)
                {
                    TextAlignmentOptions alignment = propSelector.centerValues ? TextAlignmentOptions.Center : TextAlignmentOptions.Left;
                    cellToUpdate.SetAlignment(alignment);
                }
            }
        }

        for (int i = sourceList.Count; i < rowPool.Count; i++)
        {
            rowPool[i].gameObject.SetActive(false);
        }
    }

    private BaseCell CreateCell(Transform rowTransform, CachedProperty property, float layoutRatio)
    {
        Type declaredType = property.PropertyType;
        GameObject cellInstance;

        if (typeof(Sprite).IsAssignableFrom(declaredType))
        {
            if (imageCellPrefab == null) { Debug.LogError("Image Cell Prefab is not assigned!"); return null; }
            cellInstance = Instantiate(imageCellPrefab, rowTransform);
        }
        else
        {
            if (textCellPrefab == null) { Debug.LogError("Text Cell Prefab is not assigned!"); return null; }
            cellInstance = Instantiate(textCellPrefab, rowTransform);
        }

        BaseCell cellComponent = cellInstance.GetComponent<BaseCell>();
        cellComponent.ConfigureLayout(layoutRatio);
        return cellComponent;
    }
}