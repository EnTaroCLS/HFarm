using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HFarm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public ItemToolTip itemToolTip;
        [Header("ÍÏ×§Í¼Æ¬")]
        public Image dragItem;
        [Header("Íæ¼Ò±³°üUI")]
        [SerializeField] private GameObject bagUI;
        private bool bagOpened;
        
        [SerializeField] private SlotUI[] playerSlots;

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        }

        private void Start()
        {
            for (int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }
            bagOpened = bagUI.activeInHierarchy;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                OpenBagUI();
            }
        }

        private void OnBeforeSceneUnloadEvent()
        {
            UpdateSlotHightlight(-1);
        }

        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> items)
        {
            switch (location)
            {
                case InventoryLocation.Player:
                    for (int i = 0;i < playerSlots.Length;i++)
                    {
                        if (items[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(items[i].itemID);
                            playerSlots[i].UpdateSlot(item, items[i].itemAmount);
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    
                    break;
                case InventoryLocation.Box:
                    break;
                default:
                    break;
            }
        }

        public void OpenBagUI()
        {
            bagOpened = !bagOpened;
            bagUI.SetActive(bagOpened);
        }

        public void UpdateSlotHightlight(int index)
        {
            foreach (var slot in playerSlots)
            {
                if (slot.isSelected && slot.slotIndex == index)
                    slot.slotHightlight.gameObject.SetActive(true);
                else
                {
                    slot.isSelected = false;
                    slot.slotHightlight.gameObject.SetActive(false);
                }
            }
        }
    }
}
