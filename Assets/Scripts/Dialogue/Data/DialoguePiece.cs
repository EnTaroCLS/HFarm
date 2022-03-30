using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HFarm.Dialogue
{
    [System.Serializable]
    public class DialoguePiece
    {
        [Header("∂‘ª∞œÍ«È")]
        public Sprite faceImage;
        public bool onLeft;
        public string npcName;
        [TextArea]
        public string dialogueText;
        public bool hasToPause;
        [HideInInspector]
        public bool isDone;
    }
}