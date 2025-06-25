using UnityEngine;
using System.Collections.Generic;

public static class ItemDatabase
{
    private static Dictionary<string, Item> itemDict;

    public static void Initialize()
    {
        itemDict = new Dictionary<string, Item>();
        var allItems = Resources.LoadAll<Item>("Items"); // todos os items devem estar em Resources/Items

        foreach (var item in allItems)
        {
            itemDict[item.name] = item; // ou item.itemId se tiver
        }
    }

    public static Item GetItemById(string id)
    {
        if (itemDict == null)
            Initialize();

        return itemDict.ContainsKey(id) ? itemDict[id] : null;
    }
}
