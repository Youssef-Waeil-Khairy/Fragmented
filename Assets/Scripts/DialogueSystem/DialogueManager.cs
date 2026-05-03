using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Utility;
// ReSharper disable InconsistentNaming

namespace DialogueSystem
{
    /// <summary>
    /// Author: Jan Willem Goedvolk
    /// The component attached to a canvas for displaying dialogue
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        #region Variables
        [SerializeField][Expandable] private ScriptableDialogue Dialogue;
        [SerializeField] private ScriptableDialogue.DialogueSnippet CurrentSnippet;

        [SerializeField] private GameObject DialoguePanel;
        [SerializeField] private TMP_Text SpeakerName;
        [SerializeField] private Image SpeakerSprite;
        [SerializeField] private Image BackgroundSprite;
        [SerializeField] private TMP_Text Text;

        [SerializeField] private bool showOnLevelStart = false;
        [SerializeField] private int DialogueIndex;
        [InfoBox("These events will be called when their corresponding index snippet is shown. Please ensure the number of events exactly match the number of snippets in your dialogue.", EInfoBoxType.Warning)]
        [ValidateInput("ValidateDialogue", "You must have the same number of Snippet Events as Dialogue snippets. You currently do not")]
        [Foldout("Events")][SerializeField] private List<UnityEvent> SnippetEvents;
        [Foldout("Events")] public UnityEvent OnDialogueStart;
        [Foldout("Events")] public UnityEvent OnDialogueEnd;

        InputAction nextAction;
        #endregion

        #region Unity Functions
        private void OnEnable()
        {
            StartupLogger.LogEnable("Binding dialogue advance input", name);
            nextAction = InputSystem.actions.FindAction("NextSnippet");
            nextAction.performed += NextSnippet;

            DialogueIndex = 0;

            StartupLogger.LogEnable("Finished enabling successfully", name);
        }

        private void Start()
        {
            if (!showOnLevelStart)
            {
                StartupLogger.LogStart("Hiding dialogue on start up", name);

                HideDialogue();
            }
            else
            {
                StartupLogger.LogStart("Showing dialogue on start up", name);

                ShowDialogue();
            }
        }

        private void OnDisable()
        {
            StartupLogger.LogDisable("Unbinding dialogue advance input", name);
            nextAction.performed -= NextSnippet;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Ensures the dialogue events are the same as the number of dialogue snippets
        /// </summary>
        /// <returns></returns>
        private bool ValidateDialogue()
        {
            return Dialogue.DialogueSnippets.Count == SnippetEvents.Count;
        }

        /// <summary>
        /// New input system function wrapper for advancing dialogue
        /// </summary>
        /// <param name="callbackContext">Input system action to perform this</param>
        private void NextSnippet(InputAction.CallbackContext callbackContext)
        {
            AdvanceDialogue(); // This actually goes to the next snippet
        }

        /// <summary>
        /// Updates the dialogue UI with the current snippet's data
        /// </summary>
        private void UpdateUI()
        {
            SpeakerName.text = CurrentSnippet.SpeakerName;
            if (CurrentSnippet.HasSpeakerSprite && CurrentSnippet.SpeakerSprite != null)
            {
                SpeakerSprite.sprite = CurrentSnippet.SpeakerSprite;
            }
            if (CurrentSnippet.HasBackgroundSprite && CurrentSnippet.BackgroundSprite != null)
            {
                BackgroundSprite.sprite = CurrentSnippet.BackgroundSprite;
            }
            Text.text = CurrentSnippet.Text; // TODO: maybe make typewriter thing for this

            SnippetEvents[DialogueIndex]?.Invoke();
        }

        /// <summary>
        /// Shows the dialogue UI
        /// </summary>
        [Button]
        public void ShowDialogue()
        {
            DialoguePanel.SetActive(true);
            CurrentSnippet = Dialogue.DialogueSnippets[DialogueIndex];
            UpdateUI();
            OnDialogueStart?.Invoke();
        }

        /// <summary>
        /// Hides the dialogue UI
        /// </summary>
        [Button]
        public void HideDialogue()
        {
            DialoguePanel.SetActive(false);
        }

        /// <summary>
        /// Goes to the next snippet and updates the UI as needed
        /// </summary>
        [Button]
        public void AdvanceDialogue()
        {
            if (!DialoguePanel.activeSelf) return; // Only advance if dialogue is active

            DialogueIndex++;
            if (DialogueIndex > Dialogue.DialogueSnippets.Count - 1) // check if we are at the end
            {
                DialogueIndex = 0;
                OnDialogueEnd?.Invoke();
                HideDialogue();
                return;
            }

            CurrentSnippet = Dialogue.DialogueSnippets[DialogueIndex];
            UpdateUI();
        }

        /// <summary>
        /// Another way to make the dialogue move alond, will show the UI if it isn't already
        /// </summary>
        public void Activate()
        {
            if (DialoguePanel.activeSelf)
            {
                AdvanceDialogue();
            }
            else
            {
                ShowDialogue();
            }
        }

        /// <summary>
        /// Button functions for updating snippet data in the inspector
        /// </summary>
        [Button][UsedImplicitly]
        public void GetCurrentSnippet()
        {
            if (Dialogue == null)
            {
                return;
            }

            CurrentSnippet = Dialogue.DialogueSnippets[DialogueIndex];
            UpdateUI();
        }
        #endregion
    }
}