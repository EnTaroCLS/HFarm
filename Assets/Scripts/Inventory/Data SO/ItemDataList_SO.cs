using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataList_SO", menuName = "Inventory/ItemList")]
public class ItemDataList_SO : ScriptableObject
{
    public List<ItemDetails> itemDetailsList;
}
