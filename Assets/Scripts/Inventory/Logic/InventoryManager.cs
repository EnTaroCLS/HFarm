using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFarm.Inventory
{

    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;
        [Header("背包数据")]
        public InventoryBag_SO playerBag;

        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvevt;
        }

        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvevt;
        }

        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }

        private void OnDropItemEvevt(int ID, Vector3 pos, ItemType itemType)
        {
            RemoveItem(ID, 1);
        }

        /// <summary>
        /// 添加物品到背包
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="toDestroy">是否需要被销毁</param>
        public void AddItem(Item item, bool toDestroy)
        {
            int index = GetItemIndexInBag(item.itemID);
            AddItemAtIndex(item.itemID, index, 1);

            if (toDestroy)
            {
                Destroy(item.gameObject);
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 检查背包是否有空位
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 通过物品ID找到背包已有物品位置
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <returns>-1表示没有该物品否则返回序号</returns>
        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 在背包指定位置添加物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="index">背包位置</param>
        /// <param name="amount">物品数量</param>
        private void AddItemAtIndex(int ID, int index, int amount)
        {
            if (index == -1 && CheckBagCapacity())
            {
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if (playerBag.itemList[i].itemID == 0)
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
            }
            else
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };
                playerBag.itemList[index] = item;
            }
        }

        /// <summary>
        /// 交换背包指定位置的物品
        /// </summary>
        /// <param name="fromIndex">物品起始位置</param>
        /// <param name="toIndex">物品目标位置</param>
        public void SwapItem(int fromIndex, int toIndex)
        {
            InventoryItem currentItem = playerBag.itemList[fromIndex];
            InventoryItem targetItem = playerBag.itemList[toIndex];

            if (targetItem.itemID != 0)
            {
                playerBag.itemList[fromIndex] = targetItem;
                playerBag.itemList[toIndex] = currentItem;
            }
            else
            {
                playerBag.itemList[toIndex] = currentItem;
                playerBag.itemList[fromIndex] = new InventoryItem();
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 移除指定数量的背包物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="removeAmount">数量</param>
        public void RemoveItem(int ID, int removeAmount)
        {
            var index = GetItemIndexInBag(ID);
            if (playerBag.itemList[index].itemAmount > removeAmount)
            {
                var amount = playerBag.itemList[index].itemAmount - removeAmount;
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                playerBag.itemList[index] = item;
            }
            else if (playerBag.itemList[index].itemAmount == removeAmount)
            {
                var item = new InventoryItem();
                playerBag.itemList[index] = item;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
    }
}