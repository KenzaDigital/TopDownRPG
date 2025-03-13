using UnityEngine;
// Cette annotation permet de créer des Items directement depuis le menu de l'éditeur Unity
// Clic droit dans le Project Window -> Create -> Inventory -> Item
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Item Basic Info")]
    public string itemName; // Nom de l'objet qui sera affiché dans le jeu
    public Sprite icon; // Image de l'objet qui sera affichée dans l'inventaire
    [TextArea(3, 10)] // Cette annotation crée une zone de texte plus grande dans l'inspecteur
    public string description; // Description détaillée de l'objet

    [Header("Item Properties")]
    public bool isStackable = true; // L'objet peut-il être empilé dans l'inventaire?
    public int maxStackSize = 99; // Combien d'objets identiques peuvent être empilés au maximum

    [Header("Item Value")]
    public int buyPrice; // Prix d'achat de l'objet chez un marchand
    public int sellPrice; // Prix de vente de l'objet à un marchand

    // Méthode virtuelle qui définit ce qui se passe quand l'objet est utilisé
    // Le mot-clé "virtual" permet aux classes dérivées de remplacer cette méthode
    public virtual void Use()
    {
        // Comportement par défaut: juste afficher un message dans la console
        // Cette méthode sera remplacée dans les classes dérivées pour des effets spécifiques
        // La méthode de base que les classes dérivées surchargeront
        Debug.Log("Using item: " + itemName);
    }
}
