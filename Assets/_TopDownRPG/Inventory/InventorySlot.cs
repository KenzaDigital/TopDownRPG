using UnityEngine;


// L'attribut [System.Serializable] permet � Unity d'afficher cette classe
// dans l'inspecteur et de la sauvegarder, m�me si ce n'est pas un MonoBehaviour
[System.Serializable]
public class InventorySlot
{
    public Item item; // L'objet contenu dans ce slot (null si vide)
    public int quantity; // La quantit� de cet objet (0 si vide)

    // Constructeur par d�faut - cr�e un slot vide
    public InventorySlot()
    {
        item = null;
        quantity = 0;
    }

    // Constructeur avec param�tres - cr�e un slot avec un objet sp�cifique et une quantit�
    public InventorySlot(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    // V�rifie si le slot est vide (pas d'objet ou quantit� nulle)
    public bool IsEmpty()
    {
        return item == null || quantity <= 0;
    }

    // Vide compl�tement le slot
    public void Clear()
    {
        item = null;
        quantity = 0;
    }

    // V�rifie si un objet sp�cifique peut �tre ajout� � ce slot
    // Retourne vrai si le slot est vide OU si c'est le m�me objet, qu'il est empilable
    // et que la pile n'est pas d�j� pleine
    public bool CanAddItem(Item itemToAdd)
    {
        return item == null || (item == itemToAdd && itemToAdd.isStackable && quantity < itemToAdd.maxStackSize);
    }

    // Tente d'ajouter une quantit� d'un objet � ce slot
    // Retourne la quantit� restante qui n'a pas pu �tre ajout�e (0 si tout a �t� ajout�)
    public int AddItem(Item itemToAdd, int quantityToAdd)
    {
        // Si le slot est vide, on peut simplement y mettre l'objet
        if (item == null)
        {
            item = itemToAdd;
            quantity = quantityToAdd;
            return 0; // Tout a �t� ajout�, donc 0 reste
        }

        // Si c'est le m�me objet et qu'il est empilable, on essaie d'en ajouter
        if (item == itemToAdd && itemToAdd.isStackable)
        {
            // Calculer combien on peut encore ajouter
            int spaceLeft = itemToAdd.maxStackSize - quantity;
            // On ajoute soit tout, soit ce qu'il reste de place
            int added = Mathf.Min(quantityToAdd, spaceLeft);

            // Augmenter la quantit�
            quantity += added;
            return quantityToAdd - added; // Retourne la quantit� restante
        }

        return quantityToAdd;// Si on arrive ici, c'est qu'on n'a rien pu ajouter
    }

    // Retire une certaine quantit� de l'objet du slot
    // Si la quantit� tombe � 0 ou moins, le slot est vid�
    public void RemoveItem(int quantityToRemove)
    {
        quantity -= quantityToRemove;

        if (quantity <= 0)
        {
            Clear();
        }
    }
}
