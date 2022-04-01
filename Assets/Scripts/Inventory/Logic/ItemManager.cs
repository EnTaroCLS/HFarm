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

        // ��¼����Item
        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();
        // ��¼�����Ҿ�
        private Dictionary<string, List<SceneFurniture>> sceneFurnitureDict = new Dictionary<string, List<SceneFurniture>>();

        private void OnEnable()
        {
            EventHandler.InstantiateItemInSence += OnInstantiateItemInSence;
            EventHandler.DropItemEvent += OnDropItemEvevt;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneUnloadEvent;
            EventHandler.BulidFurnitureEvent += OnBulidFureiturnEvent;
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInSence -= OnInstantiateItemInSence;
            EventHandler.DropItemEvent -= OnDropItemEvevt;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneUnloadEvent;
            EventHandler.BulidFurnitureEvent -= OnBulidFureiturnEvent;
        }

        private void OnBulidFureiturnEvent(int ID, Vector3 mousePos)
        {
            BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(ID);
            var bulidItem = Instantiate(bluePrint.buildPrefab, mousePos, Quaternion.identity, itemParent);
            if (bulidItem.GetComponent<Box>())
            {
                bulidItem.GetComponent<Box>().index = InventoryManager.Instance.BoxDataAmount;
                bulidItem.GetComponent<Box>().InitBox(bulidItem.GetComponent<Box>().index);
            }
        }

        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
            GetAllSceneFurniture();
        }

        private void OnAfterSceneUnloadEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreateAllItems();
            RebuildFurniture();
        }

        /// <summary>
        /// ��ָ������λ��������Ʒ
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <param name="pos">��������</param>
        private void OnInstantiateItemInSence(int ID, Vector3 pos)
        {
            var item = Instantiate(bounceItemPrefab, pos, Quaternion.identity, itemParent);
            item.itemID = ID;
            item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);
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
        /// ��õ�ǰ����������Ʒ
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
        /// �ؽ���ǰ������������Ʒ
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

        /// <summary>
        /// ��ó������мҾ�
        /// </summary>
        private void GetAllSceneFurniture()
        {
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();

            foreach (var item in FindObjectsOfType<Furniture>())
            {
                SceneFurniture sceneFurniture = new SceneFurniture
                {
                    itemID = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };
                if (item.GetComponent<Box>() != null)
                    sceneFurniture.boxIndex = item.GetComponent<Box>().index;

                currentSceneFurniture.Add(sceneFurniture);
            }

            if (sceneFurnitureDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                //�ҵ����ݾ͸���item�����б�
                sceneFurnitureDict[SceneManager.GetActiveScene().name] = currentSceneFurniture;
            }
            else    //������³���
            {
                sceneFurnitureDict.Add(SceneManager.GetActiveScene().name, currentSceneFurniture);
            }
        }

        /// <summary>
        /// �ؽ���ǰ�����Ҿ�
        /// </summary>
        private void RebuildFurniture()
        {
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();

            if (sceneFurnitureDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneFurniture))
            {
                if (currentSceneFurniture != null)
                {
                    foreach (SceneFurniture sceneFurniture in currentSceneFurniture)
                    {
                        BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(sceneFurniture.itemID);
                        var bulidItem = Instantiate(bluePrint.buildPrefab, sceneFurniture.position.ToVector3(), Quaternion.identity, itemParent);
                        if (bulidItem.GetComponent<Box>())
                        {
                            bulidItem.GetComponent<Box>().InitBox(sceneFurniture.boxIndex);
                        }
                    }
                }
            }
        }
    }
}
