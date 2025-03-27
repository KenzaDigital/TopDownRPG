using UnityEngine;


public class ItemPickup : MonoBehaviour, IInteractable
{
    public Item item;
    public int quantity = 1;

    public string GetCustomName()
    {
        return item.itemName;
    }

    public string GetInteractionPrompt()
    {
        return "Appuyez sur E pour ramasser " + item.itemName;
    }

    public void Interact()
    {
        Pickup();
    }

    private void Pickup()
    {
        bool wasPickedUp = InventoryManager.Instance.AddItem(item, quantity);

        if (wasPickedUp)
        {
            // Jouer un son ou un effet de particule ici

            
            

            // Notifier le QuestManager que l'objet a été collecté
            if (QuestManager.Instance != null)
            {
                
                QuestManager.Instance.ItemCollected(item);
            }
            // Détruire l'objet dans le monde
            Destroy(gameObject);
        }
        else
        {
            // Afficher un message "Inventaire plein"
            Debug.Log("Votre inventaire est plein !");
        }

    }
}
