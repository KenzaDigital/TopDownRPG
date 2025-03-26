using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Events;   

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel; // Le panel qui contient tout l'UI de dialogue
    public TextMeshProUGUI nameText; // Le texte qui affiche le nom du personnage qui parle
    public TextMeshProUGUI dialogueText; // Le texte qui affiche le dialogue lui-même
    public GameObject continueIndicator; // Un indicateur visuel (comme une flèche) qui montre qu'on peut continuer

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.05f; // Temps entre l'affichage de chaque caractère (en secondes)

    [Header("Input Settings")]
    public InputActionReference continueDialogueAction; // L'action d'input pour continuer le dialogue

    private string[] currentLines; // Tableau contenant toutes les lignes du dialogue actuel
    private int currentLineIndex; // Index de la ligne actuellement affichée
    private bool isTyping; // Est-ce que le texte est en train d'être tapé?
    private Coroutine typingCoroutine; // Référence à la coroutine qui affiche le texte caractère par caractère
    private bool isDialogueActive; // Est-ce qu'un dialogue est actif?

    // Ajout de l'événement Unity pour la fin du dialogue
    public UnityEvent onDialogueEnded;

    // Singleton pattern
    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        // Initialiser l'événement Unity
        if (onDialogueEnded == null)
        {
            onDialogueEnded = new UnityEvent();
        }

        // S'assurer que le dialogue est désactivé au démarrage
        dialoguePanel.SetActive(false);
        if (continueIndicator != null)
            continueIndicator.SetActive(false);

        // Configurer l'action d'input pour continuer le dialogue
        if (continueDialogueAction != null)
        {
            continueDialogueAction.action.started += OnContinueDialogueInput;
        }
    }

    private void OnEnable()
    {
        // Activer l'action d'input
        if (continueDialogueAction != null)
        {
            continueDialogueAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        // Désactiver l'action d'input
        if (continueDialogueAction != null)
        {
            continueDialogueAction.action.Disable();
        }
    }

    private void OnDestroy()
    {
        // Nettoyer les abonnements d'événements
        if (continueDialogueAction != null)
        {
            continueDialogueAction.action.started -= OnContinueDialogueInput;
        }
    }

    // Cette méthode est appelée quand l'action de continuer le dialogue est déclenchée
    private void OnContinueDialogueInput(InputAction.CallbackContext context)
    {
        if (!isDialogueActive)
        {
            return;
        }

        if (isTyping)
        {
            // Si le texte est en train d'être tapé, le compléter immédiatement
            CompleteTyping();
        }
        else
        {
            // Sinon, passer à la ligne suivante
            DisplayNextLine();
        }
    }

    public void StartDialogue(string speakerName, string[] lines)
    {
        // Réinitialiser les variables de dialogue
        currentLines = lines;
        currentLineIndex = 0;
        isDialogueActive = true;

        // Activer le panel et définir le nom du personnage
        dialoguePanel.SetActive(true);
        nameText.text = speakerName;

        // Afficher la première ligne
        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        // Si on est à la fin du dialogue, terminer
        if (currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        // Désactiver l'indicateur de continuation
        if (continueIndicator)
        {
            continueIndicator.SetActive(false);
        }

        // Commencer à taper la ligne
        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentLineIndex]));
        currentLineIndex++;
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        // Activer l'indicateur de continuation
        if (continueIndicator)
        {
            continueIndicator.SetActive(true);
        }
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = currentLines[currentLineIndex - 1];
        isTyping = false;

        // Activer l'indicateur de continuation
        if (continueIndicator)
        {
            continueIndicator.SetActive(true);
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);

        // Invoquer l'événement de fin de dialogue
        onDialogueEnded.Invoke();
    }

    // Méthode publique pour vérifier si un dialogue est en cours
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}
