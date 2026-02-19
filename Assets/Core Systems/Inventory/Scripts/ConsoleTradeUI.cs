using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem; // <-- Required for the New Input System

public class ConsoleTradeUI : MonoBehaviour
{
    [SerializeField] private TransactionManager transactionManager;
    private bool isTrading = false;
    private List<TradeRecipe> currentRecipes;

    private void Start()
    {
        if (transactionManager == null) transactionManager = GetComponent<TransactionManager>();

        transactionManager.onTradeStarted.AddListener(OnTradeStarted);
        transactionManager.onTradeEnded.AddListener(OnTradeEnded);
    }

    private void OnTradeStarted(List<TradeRecipe> recipes)
    {
        isTrading = true;
        currentRecipes = recipes;
        PrintScreen();
    }

    private void OnTradeEnded()
    {
        isTrading = false;
        currentRecipes = null;
        Debug.Log("\n<color=red>--- TRADE CLOSED ---</color>\n");
    }

    private void Update()
    {
        // Safety check: Don't run if not trading or if no keyboard is connected
        if (!isTrading || Keyboard.current == null) return;

        // Spacebar to close
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            transactionManager.EndTrade();
            return;
        }

        // Check if Shift is held down
        bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;

        for (int i = 1; i <= 9; i++)
        {
            if (WasNumberKeyPressed(i))
            {
                int index = i - 1;

                if (currentRecipes != null && currentRecipes.Count > 0)
                {
                    if (index < currentRecipes.Count)
                    {
                        transactionManager.ExecuteRecipe(currentRecipes[index]);
                        PrintScreen();
                    }
                }
                else
                {
                    if (isShiftPressed)
                    {
                        var partnerSlots = transactionManager.CurrentPartnerInventory.slots;
                        if (index < partnerSlots.Count)
                            transactionManager.TakeItemFromPartner(partnerSlots[index], 1);
                    }
                    else
                    {
                        var playerSlots = transactionManager.myInventory.slots;
                        if (index < playerSlots.Count)
                            transactionManager.GiveItemToPartner(playerSlots[index], 1);
                    }
                    PrintScreen();
                }
            }
        }
    }

    // Helper method to translate the index to the new Input System keys
    private bool WasNumberKeyPressed(int number)
    {
        switch (number)
        {
            case 1: return Keyboard.current.digit1Key.wasPressedThisFrame;
            case 2: return Keyboard.current.digit2Key.wasPressedThisFrame;
            case 3: return Keyboard.current.digit3Key.wasPressedThisFrame;
            case 4: return Keyboard.current.digit4Key.wasPressedThisFrame;
            case 5: return Keyboard.current.digit5Key.wasPressedThisFrame;
            case 6: return Keyboard.current.digit6Key.wasPressedThisFrame;
            case 7: return Keyboard.current.digit7Key.wasPressedThisFrame;
            case 8: return Keyboard.current.digit8Key.wasPressedThisFrame;
            case 9: return Keyboard.current.digit9Key.wasPressedThisFrame;
            default: return false;
        }
    }

    private void PrintScreen()
    {
        string log = "\n========================================\n";

        if (currentRecipes != null && currentRecipes.Count > 0)
        {
            log += "<color=yellow>--- SHOP MODE ---</color>\n";
            for (int i = 0; i < currentRecipes.Count; i++)
            {
                var r = currentRecipes[i];
                log += $"[{i + 1}] Buy {r.productAmount}x {r.productItem.itemName} (Costs {r.currencyAmount}x {r.currencyItem.itemName})\n";
            }
        }
        else
        {
            log += "<color=yellow>--- CONTAINER MODE ---</color>\n";

            string giveText = transactionManager.CanDepositFreely ? "(1-9 to Give)" : "<color=red>(DEPOSIT LOCKED)</color>";
            log += $"<color=green>YOUR ITEMS {giveText}:</color>\n";

            var pSlots = transactionManager.myInventory.slots;
            for (int i = 0; i < pSlots.Count; i++) log += $"[{i + 1}] {pSlots[i].amount}x {pSlots[i].ItemData.itemName}\n";

            string takeText = transactionManager.CanTakeFreely ? "(Shift+1-9 to Take)" : "<color=red>(TAKE LOCKED)</color>";
            log += $"\n<color=cyan>PARTNER ITEMS {takeText}:</color>\n";

            var nSlots = transactionManager.CurrentPartnerInventory.slots;
            for (int i = 0; i < nSlots.Count; i++) log += $"[Shift+{i + 1}] {nSlots[i].amount}x {nSlots[i].ItemData.itemName}\n";
        }

        Debug.Log(log);
    }
}