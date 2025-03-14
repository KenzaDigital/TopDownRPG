using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class InventoryManager : MonoBehaviour
{
    [Header("Input Settings")]
    public InputActionReference inventoryAction; // L'action pour ouvrir/fermer l'inventaire

    [Header("Inventory Settings")]
    public int inventorySize = 20;  // Nombre d'emplacements dans l'inventaire
    public List<InventorySlot> slots = new List<InventorySlot>(); // Liste de tous les emplacements
                                                                  // Dictionnaire pour acc�der rapidement aux objets par leur ID (peu utilis� dans cette version)
    Dictionary<string, InventorySlot> itemsById = new Dictionary<string, InventorySlot>();

    [Header("UI References")]
    public GameObject inventoryPanel;  // Le panel principal de l'inventaire
    public Transform slotsGrid; // Le parent o� les slots seront instanci�s
    public GameObject slotPrefab; // Le prefab pour un emplacement d'inventaire
    public GameObject backgroundOverlay; // Fond semi-transparent
    public Button closeButton; // Bouton de fermeture

    private bool isInventoryOpen = false; // L'inventaire est-il actuellement ouvert?

    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        // Initialiser les slots d'inventaire
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new InventorySlot());
        }

        // Fermer l'inventaire au d�marrage
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
        if (backgroundOverlay != null)
        {
            backgroundOverlay.SetActive(false);
        }

        // Configurer l'action d'input pour ouvrir l'inventaire
        if (inventoryAction != null)
        {
            inventoryAction.action.started += OnInventoryInput;
        }

        // Configurer le bouton de fermeture
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseInventory);
        }
    }

    private void Start()
    {
        // Cr�er les slots UI
        RefreshInventoryUI();
    }

    private void OnEnable()
    {
        // Activer l'action d'input
        if (inventoryAction != null)
        {
            inventoryAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        // D�sactiver l'action d'input
        if (inventoryAction != null)
        {
            inventoryAction.action.Disable();
        }
    }

    private void OnDestroy()
    {
        // Nettoyer les abonnements d'�v�nements
        if (inventoryAction != null)
        {
            inventoryAction.action.started -= OnInventoryInput;
        }
    }

    private void OnInventoryInput(InputAction.CallbackContext obj)
    {
        ToggleInventory();
    }

    // M�thode pour ouvrir ou fermer l'inventaire
    public void ToggleInventory()
    {
        // Inverser l'�tat actuel
        isInventoryOpen = !isInventoryOpen;

        // Appliquer ce changement � l'interface
        if (inventoryPanel)
        {
            inventoryPanel.SetActive(isInventoryOpen);
        }
        if (backgroundOverlay)
        {
            backgroundOverlay.SetActive(isInventoryOpen);
        }
    }

    // M�thode pour fermer l'inventaire
    public void CloseInventory()
    {
        isInventoryOpen = false;
        if (inventoryPanel)
        {
            inventoryPanel.SetActive(false);
        }
        if (backgroundOverlay)
        {
            backgroundOverlay.SetActive(false);
        }
    }

    // M�thode pour ajouter un objet � l'inventaire
    // Retourne true si l'objet a �t� ajout� avec succ�s, false sinon
    public bool AddItem(Item item, int quantity = 1)
    {
        // V�rifier si l'item est empilable
        if (item.isStackable)
        {
            // Chercher un slot existant avec le m�me item
            for (int i = 0; i < slots.Count; i++)
            {
                // V�rifier si le slot contient le m�me objet et n'est pas plein
                if (slots[i].item == item && slots[i].quantity < item.maxStackSize)
                {
                    // Essayer d'ajouter l'objet � ce slot
                    quantity = slots[i].AddItem(item, quantity);

                    // Si tout a �t� ajout�, mettre � jour l'UI et retourner true
                    if (quantity <= 0)
                    {
                        RefreshInventoryUI();
                        return true; // Tout a �t� ajout�
                    }
                }
            }
        }

        // Chercher un slot vide
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty())
            {
                // Essayer d'ajouter l'objet � cet emplacement vide
                quantity = slots[i].AddItem(item, quantity);

                // Si tout a �t� ajout�, mettre � jour l'UI et retourner true
                if (quantity <= 0)
                {
                    RefreshInventoryUI();
                    return true; // Tout a �t� ajout�
                }
            }
        }

        // --- INVENTAIRE PLEIN ---
        // Si on arrive ici, c'est que l'objet n'a pas pu �tre ajout� compl�tement
        Debug.Log("Inventaire plein, impossible d'ajouter " + item.itemName);
        RefreshInventoryUI();
        return false;
    }

    // M�thode pour retirer un objet de l'inventaire
    public void RemoveItem(Item item, int quantity = 1)
    {
        // Parcourir tous les slots pour trouver le premier contenant cet objet
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == item)
            {
                // Retirer l'objet du slot trouv�
                slots[i].RemoveItem(quantity);
                RefreshInventoryUI();
                return;
            }
        }
    }

    // M�thode pour utiliser un objet � partir de son index dans l'inventaire
    public void UseItem(int slotIndex)
    {
        // V�rifier que l'index est valide
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            // V�rifier que le slot n'est pas vide
            if (!slots[slotIndex].IsEmpty())
            {
                // Utiliser l'objet et mettre � jour l'UI
                slots[slotIndex].item.Use();
                RefreshInventoryUI();
            }
        }
    }

    // M�thode priv�e pour mettre � jour l'interface utilisateur de l'inventaire
    private void RefreshInventoryUI()
    {
        // Si l'UI n'est pas encore initialis�e, retourner
        if (slotsGrid == null || slotPrefab == null)
        {
            return;
        }

        // --- NETTOYER L'UI EXISTANTE ---
        // Supprimer tous les anciens slots visuels
        foreach (Transform child in slotsGrid)
        {
            Destroy(child.gameObject);
        }

        // --- CR�ER LES NOUVEAUX SLOTS VISUELS ---
        // Cr�er un slot visuel pour chaque slot de donn�es
        for (int i = 0; i < slots.Count; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotsGrid);
            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();

            if (slotUI != null)
            {
                slotUI.SetupSlot(i, slots[i]);
            }
        }
    }
}
