using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DragAndDropContainer : MonoBehaviour
{
    public Transform ItemRoot;
    public int ItemMax;
    public bool SwapItemWhenFull;

    protected virtual void Awake()
    {
        if (ItemRoot == null)
            ItemRoot = transform;
    }

    public List<DragAndDropItem> GetAllItem()
    {
        List<DragAndDropItem> itemList = new List<DragAndDropItem>();

        Transform parent = ItemRoot;
        if (parent == null)
            parent = transform;

        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject child = parent.GetChild(i).gameObject;
            if (child.activeSelf)
            {
                DragAndDropItem item = child.GetComponent<DragAndDropItem>();
                if (item != null)
                    itemList.Add(item);
            }
        }

        return itemList;
    }

    public bool IsFull(out List<DragAndDropItem> itemList)
    {
        itemList = GetAllItem();

        if (ItemMax <= 0)
            return false;

        return itemList.Count >= ItemMax;
    }
}