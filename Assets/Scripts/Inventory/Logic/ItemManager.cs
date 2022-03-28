using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HFarm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        public Item bounceItemPrefab;
        private Transform itemParent;

        private Transform PlayerTransform => FindObjectOfType<Player>().transform;

        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();

        private void OnEnable()
        {
            EventHandler.InstantiateItemInSence += OnInstantiateItemInSence;
            EventHandler.DropItemEvent += OnDropItemEvevt;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneUnloadEvent;
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInSence -= OnInstantiateItemInSence;
            EventHandler.DropItemEvent -= OnDropItemEvevt;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneUnloadEvent;
        }

        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
        }

        private void OnAfterSceneUnloadEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreateAllItems();
        }

        /// <summary>
        /// 在指定坐标位置生成物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="pos">世界坐标</param>
        private void OnInstantiateItemInSence(int ID, Vector3 pos)
        {
            var item = Instantiate(itemPrefab, pos, Quaternion.identity, itemParent);
            item.itemID = ID;
        }

        private void OnDropItemEvevt(int ID, Vector3 mousePos, ItemType itemType)
        {
            if (itemType == ItemType.Seed) return;
            var item = Instantiate(bounceItemPrefab, PlayerTransform.position, Quaternion.identity, itemParent);
            item.itemID = ID;
            var dir = (mousePos - PlayerTransform.position).normalized;
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos, dir);
        }

        /// <summary>
        /// 获得当前场景所有物品
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();

            foreach (var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem()
                {
                    itemID = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };

                currentSceneItems.Add(sceneItem);
            }

            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItems;
            }
            else
            {
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItems);
            }
        }

        /// <summary>
        /// 重建当前场景中所以物品
        /// </summary>
        private void RecreateAllItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems))
            {
                if (currentSceneItems != null)
                {
                    foreach (var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }

                    foreach (var item in currentSceneItems)
                    {
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.itemID);
                    }
                }
            }
        }
    }
}
