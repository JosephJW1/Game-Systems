using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class ItemActionPanel : MonoBehaviour
{
    // A simple struct to define an action in the Inspector
    [System.Serializable]
    public struct ActionDefinition
    {
        public string buttonLabel;
        // This event passes the ItemObject to your TransactionManager
        public UnityEvent<object> actionFunction;
    }

    [Header("Settings")]
    [SerializeField] private GameObject actionButtonPrefab; // Prefab with Button & TextMeshProUGUI
    [SerializeField] private Transform buttonContainer;

    [Header("Actions")]
    // The list of functions you wanted!
    [SerializeField] private List<ActionDefinition> availableActions;

    private object currentItem;
    private List<GameObject> spawnedButtons = new List<GameObject>();

    // --- PUBLIC ENTRY POINT ---
    // Connect ObjectListUI's "OnRowClicked" to this function!
    public void OpenPanel(object item)
    {
        this.currentItem = item;
        gameObject.SetActive(true);
        RefreshButtons();
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
        this.currentItem = null;
    }

    private void RefreshButtons()
    {
        // 1. Clear old buttons
        foreach (var btn in spawnedButtons) Destroy(btn);
        spawnedButtons.Clear();

        // 2. Create new buttons for each action
        foreach (var actionDef in availableActions)
        {
            GameObject btnObj = Instantiate(actionButtonPrefab, buttonContainer);
            spawnedButtons.Add(btnObj);

            // Setup Label
            TextMeshProUGUI label = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = actionDef.buttonLabel;

            // Setup Click Event
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    // Call the function, passing the current item
                    actionDef.actionFunction.Invoke(currentItem);

                    // Optional: Close panel after action?
                    // ClosePanel(); 
                });
            }
        }
    }
}