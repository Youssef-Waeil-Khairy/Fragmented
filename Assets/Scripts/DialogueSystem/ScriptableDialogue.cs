using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace DialogueSystem
{
    /// <summary>
    /// Author: Jan Willem Goedvolk
    /// Scriptable object containing a piece of dialogue or texts
    /// </summary>
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue System/New Dialogue", order = 0)]
    public class ScriptableDialogue : ScriptableObject
    {
        /// <summary>
        /// A single text of dialogue
        /// </summary>
        [Serializable]
        public struct DialogueSnippet
        {
            public string SpeakerName;
            public bool HasSpeakerSprite;
            [AllowNesting][ShowIf("HasSpeakerSprite")] public Sprite SpeakerSprite;
            public bool HasBackgroundSprite;
            [AllowNesting][ShowIf("HasBackgroundSprite")] public Sprite BackgroundSprite;
            [TextArea(1, 4)] public string Text;
        }

        /// <summary>
        /// The actual full dialogue
        /// </summary>
        public List<DialogueSnippet> DialogueSnippets;
    }
}