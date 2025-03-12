using UnityEngine;
// Cette annotation permet de cr�er des Items directement depuis le menu de l'�diteur Unity
// Clic droit dans le Project Window -> Create -> Inventory -> Item
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Item Basic Info")]
    public string itemName; // Nom de l'objet qui sera affich� dans le jeu
    public Sprite icon; // Image de l'objet qui sera affich�e dans l'inventaire
    [TextArea(3, 10)] // Cette annotation cr�e une zone de texte plus grande dans l'inspecteur
    public string description; // Description d�taill�e de l'objet

    [Header("Item Properties")]
    public bool isStackable = true; // L'objet peut-il �tre empil� dans l'inventaire?
    public int maxStackSize = 99; // Combien d'objets identiques peuvent �tre empil�s au maximum

    [Header("Item Value")]
    public int buyPrice; // Prix d'achat de l'objet chez un marchand
    public int sellPrice; // Prix de vente de l'objet � un marchand

    // M�thode virtuelle qui d�finit ce qui se passe quand l'objet est utilis�
    // Le mot-cl� "virtual" permet aux classes d�riv�es de remplacer cette m�thode
    public virtual void Use()
    {
        // Comportement par d�faut: juste afficher un message dans la console
        // Cette m�thode sera remplac�e dans les classes d�riv�es pour des effets sp�cifiques
        // La m�thode de base que les classes d�riv�es surchargeront
        Debug.Log("Using item: " + itemName);
    }
}
