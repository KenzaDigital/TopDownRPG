using UnityEngine;


// L'attribut [System.Serializable] permet à Unity d'afficher cette classe
// dans l'inspecteur et de la sauvegarder, même si ce n'est pas un MonoBehaviour
[System.Serializable]
public class InventorySlot
{
    public Item item; // L'objet contenu dans ce slot (null si vide)
    public int quantity; // La quantité de cet objet (0 si vide)

    // Constructeur par défaut - crée un slot vide
    public InventorySlot()
    {
        item = null;
        quantity = 0;
    }

    // Constructeur avec paramètres - crée un slot avec un objet spécifique et une quantité
    public InventorySlot(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    // Vérifie si le slot est vide (pas d'objet ou quantité nulle)
    public bool IsEmpty()
    {
        return item == null || quantity <= 0;
    }

    // Vide complètement le slot
    public void Clear()
    {
        item = null;
        quantity = 0;
    }

    // Vérifie si un objet spécifique peut être ajouté à ce slot
    // Retourne vrai si le slot est vide OU si c'est le même objet, qu'il est empilable
    // et que la pile n'est pas déjà pleine
    public bool CanAddItem(Item itemToAdd)
    {
        return item == null || (item == itemToAdd && itemToAdd.isStackable && quantity < itemToAdd.maxStackSize);
    }

    // Tente d'ajouter une quantité d'un objet à ce slot
    // Retourne la quantité restante qui n'a pas pu être ajoutée (0 si tout a été ajouté)
    public int AddItem(Item itemToAdd, int quantityToAdd)
    {
        // Si le slot est vide, on peut simplement y mettre l'objet
        if (item == null)
        {
            item = itemToAdd;
            quantity = quantityToAdd;
            return 0; // Tout a été ajouté, donc 0 reste
        }

        // Si c'est le même objet et qu'il est empilable, on essaie d'en ajouter
        if (item == itemToAdd && itemToAdd.isStackable)
        {
            // Calculer combien on peut encore ajouter
            int spaceLeft = itemToAdd.maxStackSize - quantity;
            // On ajoute soit tout, soit ce qu'il reste de place
            int added = Mathf.Min(quantityToAdd, spaceLeft);

            // Augmenter la quantité
            quantity += added;
            return quantityToAdd - added; // Retourne la quantité restante
        }

        return quantityToAdd;// Si on arrive ici, c'est qu'on n'a rien pu ajouter
    }

    // Retire une certaine quantité de l'objet du slot
    // Si la quantité tombe à 0 ou moins, le slot est vidé
    public void RemoveItem(int quantityToRemove)
    {
        quantity -= quantityToRemove;

        if (quantity <= 0)
        {
            Clear();
        }
    }
}
