using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI quantityText;
    public Image slotBackground;

    private int slotIndex;
    private InventorySlot slotData;

    public void SetupSlot(int index, InventorySlot data)
    {
        slotIndex = index;
        slotData = data;

        // Mettre à jour l'UI en fonction des données du slot
        if (data.IsEmpty())
        {
            itemIcon.enabled = false;
            quantityText.gameObject.SetActive(false);
        }
        else
        {
            itemIcon.sprite = data.item.icon;
            itemIcon.enabled = true;

            if (data.quantity > 1)
            {
                quantityText.text = data.quantity.ToString();
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Clic gauche - Utiliser l'item
            if (!slotData.IsEmpty())
            {
                InventoryManager.Instance.UseItem(slotIndex);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Clic droit - Jeter l'item ou afficher des options
            if (!slotData.IsEmpty())
            {
                // Pour l'instant, simplement jeter un item
                InventoryManager.Instance.RemoveItem(slotData.item, 1);
            }
        }
    }
}
