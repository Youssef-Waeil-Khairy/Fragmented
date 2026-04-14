using System;
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
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private GameObject DialoguePanel;
        // Speaker name
        [SerializeField] private TMP_Text SpeakerName;
        // Speaker Sprite
        [SerializeField] private Image SpeakerSprite;
        // Background Sprite
        [SerializeField] private Image BackgroundSprite;
        // Text
        [SerializeField] private TMP_Text Text;
        [SerializeField] private bool showOnLevelStart = false;
        [SerializeField][Expandable] private ScriptableDialogue Dialogue;
        [SerializeField] private ScriptableDialogue.DialogueSnippet CurrentSnippet;
        [SerializeField] private int DialogueIndex;
        [InfoBox("These events will be called when their corresponding index snippet is shown. Please ensure the number of events exactly match the number of snippets in your dialogue.", EInfoBoxType.Warning)]
        [ValidateInput("ValidateDialogue", "You must have the same number of Snippet Events as Dialogue snippets. You currently do not")]
        [Foldout("Events")][SerializeField] private List<UnityEvent> SnippetEvents;
        [Foldout("Events")] public UnityEvent OnDialogueStart;
        [Foldout("Events")] public UnityEvent OnDialogueEnd;

        InputAction nextAction;

        private void OnEnable()
        {
            StartupLogger.LogEnable("Binding dialogue advance input", name);
            nextAction = InputSystem.actions.FindAction("NextSnippet");
            nextAction.performed += NextSnippet;
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
                while (PlayerController.PLAYERCONTROLLERSTARTED == false)
                {
                    StartupLogger.LogStart($"Waiting for Player Controller to start. currently: {PlayerController.PLAYERCONTROLLERSTARTED}", name);
                }

                ShowDialogue();
            }
        }

        // private void Start()
        // {
        //     if (!showOnLevelStart)
        //     {
        //         HideDialogue();
        //     }
        //     else
        //     {
        //         ShowDialogue();
        //     }
        // }

        private void OnDisable()
        {
            StartupLogger.LogDisable("Unbinding dialogue advance input", name);
            nextAction.performed -= NextSnippet;
        }

        private bool ValidateDialogue()
        {
            return Dialogue.DialogueSnippets.Count == SnippetEvents.Count;
        }

        private void NextSnippet(InputAction.CallbackContext callbackContext)
        {
            AdvanceDialogue();
        }

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

        [Button]
        public void ShowDialogue()
        {
            DialoguePanel.SetActive(true);
            CurrentSnippet = Dialogue.DialogueSnippets[DialogueIndex];
            UpdateUI();
            OnDialogueStart?.Invoke();
        }

        [Button]
        public void HideDialogue()
        {
            DialoguePanel.SetActive(false);
        }

        [Button]
        public void AdvanceDialogue()
        {
            if (!DialoguePanel.activeSelf) return;

            DialogueIndex++;
            if (DialogueIndex > Dialogue.DialogueSnippets.Count - 1)
            {
                DialogueIndex = 0;
                OnDialogueEnd?.Invoke();
                HideDialogue();
                return;
            }
            CurrentSnippet = Dialogue.DialogueSnippets[DialogueIndex];
            UpdateUI();
        }

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
    }
}