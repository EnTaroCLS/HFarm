using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HFarm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public ItemToolTip itemToolTip;
        [Header("拖拽图片")]
        public Image dragItem;

        [Header("玩家背包UI")]
        [SerializeField] private GameObject bagUI;
        private bool bagOpened;

        [Header("通用背包")]
        [SerializeField] private GameObject baseBag;
        public GameObject shopSlotPrefab;
        public GameObject boxSlotPrefab;

        [Header("交易UI")]
        public TradeUI tradeUI;
        public TextMeshProUGUI playerMoneyText;
        
        [SerializeField] private SlotUI[] playerSlots;
        [SerializeField] private List<SlotUI> baseBagSlots;

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI += OnShowTradeUI;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI -= OnShowTradeUI;
        }

        private void Start()
        {
            for (int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }
            bagOpened = bagUI.activeInHierarchy;
            playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                OpenBagUI();
            }
        }

        private void OnShowTradeUI(ItemDetails item, bool isSell)
        {
            tradeUI.gameObject.SetActive(true);
            tradeUI.SetupTradeUI(item, isSell);
        }

        private void OnBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            baseBag.SetActive(false);
            itemToolTip.gameObject.SetActive(false);
            UpdateSlotHightlight(-1);

            foreach (var slot in baseBagSlots)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();

            if (slotType == SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                bagUI.SetActive(false);
                bagOpened = false;
            }
        }

        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            // TODO:通用箱子Prefab
            GameObject prefab = slotType switch
            {
                SlotType.Shop => shopSlotPrefab,
                SlotType.Box => boxSlotPrefab,
                _ => null,
            };

            //生成背包UI
            baseBag.SetActive(true);
            baseBagSlots = new List<SlotUI>();

            for (int i = 0; i < bagData.itemList.Count; i++)
            {
                var slot = Instantiate(prefab, baseBag.transform.GetChild(0)).GetComponent<SlotUI>();
                slot.slotIndex = i;
                baseBagSlots.Add(slot);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());
            
            if (slotType == SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(-1, 0.5f);
                bagUI.SetActive(true);
                bagOpened = true;
            }
            
            //更新UI显示
            OnUpdateInventoryUI(InventoryLocation.Box, bagData.itemList);
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
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {
                        if (items[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(items[i].itemID);
                            baseBagSlots[i].UpdateSlot(item, items[i].itemAmount);
                        }
                        else
                        {
                            baseBagSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
            playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
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
