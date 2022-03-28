using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace HFarm.Inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("获取组件")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
        public Image slotHightlight;
        [SerializeField] Button button;

        [Header("格子类型")]
        public SlotType slotType;
        public bool isSelected;
        public int slotIndex;

        public ItemDetails itemDetails;
        public int itemAmount;

        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        private void Start()
        {
            isSelected = false;
            if (itemDetails == null)
                UpdateEmptySlot();
        }

        public void UpdateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            slotImage.sprite = item.itemIcon;
            itemAmount = amount;
            amountText.text = amount.ToString();
            slotImage.enabled = true;
            button.interactable = true;
        }

        /// <summary>
        /// 将Slot更新为空
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
                inventoryUI.UpdateSlotHightlight(-1);
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }

            itemDetails = null;
            slotImage.enabled = false;
            amountText.text = "";
            button.interactable = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails == null)
                return;
            isSelected = !isSelected;

            inventoryUI.UpdateSlotHightlight(slotIndex);

            if (slotType == SlotType.Bag)
            {
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemAmount != 0)
            {
                inventoryUI.dragItem.enabled = true;
                inventoryUI.dragItem.sprite = slotImage.sprite;
                inventoryUI.dragItem.SetNativeSize();
                isSelected = true;
                inventoryUI.UpdateSlotHightlight(slotIndex);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.enabled = false;

            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                    return;
                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex = targetSlot.slotIndex;

                if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }

                inventoryUI.UpdateSlotHightlight(-1);
            }
            /*else
            {
                if (itemDetails.canDropped)
                {
                    var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                    EventHandler.CallInstantiateItemInSence(itemDetails.itemID, pos);
                }
            }*/
        }
    }
}
