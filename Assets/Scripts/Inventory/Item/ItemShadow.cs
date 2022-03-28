using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFarm.Inventory
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemShadow : MonoBehaviour
    {
        public SpriteRenderer itemRenderer;
        private SpriteRenderer shadowRenderer;

        private void Awake()
        {
            shadowRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            shadowRenderer.sprite = itemRenderer.sprite;
            shadowRenderer.color = new Color(0, 0, 0, 0.3f);
        }
    }
}