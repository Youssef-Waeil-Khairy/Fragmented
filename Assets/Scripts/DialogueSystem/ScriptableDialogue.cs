using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue System/New Dialogue", order = 0)]
    public class ScriptableDialogue : ScriptableObject
    {
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

        public List<DialogueSnippet> DialogueSnippets;
    }
}