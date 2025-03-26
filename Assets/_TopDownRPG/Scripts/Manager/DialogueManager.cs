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
    public TextMeshProUGUI dialogueText; // Le texte qui affiche le dialogue lui-m�me
    public GameObject continueIndicator; // Un indicateur visuel (comme une fl�che) qui montre qu'on peut continuer

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.05f; // Temps entre l'affichage de chaque caract�re (en secondes)

    [Header("Input Settings")]
    public InputActionReference continueDialogueAction; // L'action d'input pour continuer le dialogue

    private string[] currentLines; // Tableau contenant toutes les lignes du dialogue actuel
    private int currentLineIndex; // Index de la ligne actuellement affich�e
    private bool isTyping; // Est-ce que le texte est en train d'�tre tap�?
    private Coroutine typingCoroutine; // R�f�rence � la coroutine qui affiche le texte caract�re par caract�re
    private bool isDialogueActive; // Est-ce qu'un dialogue est actif?

    // Ajout de l'�v�nement Unity pour la fin du dialogue
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

        // Initialiser l'�v�nement Unity
        if (onDialogueEnded == null)
        {
            onDialogueEnded = new UnityEvent();
        }

        // S'assurer que le dialogue est d�sactiv� au d�marrage
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
        // D�sactiver l'action d'input
        if (continueDialogueAction != null)
        {
            continueDialogueAction.action.Disable();
        }
    }

    private void OnDestroy()
    {
        // Nettoyer les abonnements d'�v�nements
        if (continueDialogueAction != null)
        {
            continueDialogueAction.action.started -= OnContinueDialogueInput;
        }
    }

    // Cette m�thode est appel�e quand l'action de continuer le dialogue est d�clench�e
    private void OnContinueDialogueInput(InputAction.CallbackContext context)
    {
        if (!isDialogueActive)
        {
            return;
        }

        if (isTyping)
        {
            // Si le texte est en train d'�tre tap�, le compl�ter imm�diatement
            CompleteTyping();
        }
        else
        {
            // Sinon, passer � la ligne suivante
            DisplayNextLine();
        }
    }

    public void StartDialogue(string speakerName, string[] lines)
    {
        // R�initialiser les variables de dialogue
        currentLines = lines;
        currentLineIndex = 0;
        isDialogueActive = true;

        // Activer le panel et d�finir le nom du personnage
        dialoguePanel.SetActive(true);
        nameText.text = speakerName;

        // Afficher la premi�re ligne
        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        // Si on est � la fin du dialogue, terminer
        if (currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        // D�sactiver l'indicateur de continuation
        if (continueIndicator)
        {
            continueIndicator.SetActive(false);
        }

        // Commencer � taper la ligne
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

        // Invoquer l'�v�nement de fin de dialogue
        onDialogueEnded.Invoke();
    }

    // M�thode publique pour v�rifier si un dialogue est en cours
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}
