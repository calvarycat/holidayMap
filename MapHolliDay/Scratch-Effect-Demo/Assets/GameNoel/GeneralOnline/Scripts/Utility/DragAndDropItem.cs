using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DragAndDropItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private const float TimeMove = 0.2f;

    public DragAndDropContainer ParentContainer;
    public Canvas MyCanvas;

    private bool _isDown;

    public void Init(Canvas canvas, DragAndDropContainer container)
    {
        if (canvas != null)
            MyCanvas = canvas;

        if (MyCanvas == null)
            MyCanvas = FindObjectOfType<Canvas>();

        ParentContainer = container;

        if (ParentContainer != null)
        {
            transform.SetParent(ParentContainer.ItemRoot);
            transform.position = ParentContainer.ItemRoot.position;
        }
    }

    private void Update()
    {
        if (_isDown)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(MyCanvas.transform as RectTransform,
                Input.mousePosition, MyCanvas.worldCamera, out pos);
            transform.position = MyCanvas.transform.TransformPoint(pos);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDown = true;
        transform.SetParent(MyCanvas.transform);
        transform.SetAsLastSibling();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDown = false;
        DragAndDropContainer container = null;
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, raycastResults);
        if (raycastResults.Count > 0)
        {
            for (int i = 0; i < raycastResults.Count; i++)
            {
                container = raycastResults[i].gameObject.GetComponent<DragAndDropContainer>();
                if (container != null)
                {
                    break;
                }
            }
        }

        if (container == null || container.transform == transform.parent)
        {
            transform.SetParent(ParentContainer.ItemRoot);
            iTween.MoveTo(gameObject, transform.parent.position, TimeMove);
        }
        else
        {
            List<DragAndDropItem> itemList = null;

            if (container.IsFull(out itemList))
            {
                if (container.SwapItemWhenFull)
                {
                    DragAndDropItem otherItem = null;

                    if (itemList.Count > 0)
                        otherItem = itemList[itemList.Count - 1];

                    if (otherItem != null)
                        otherItem.ChangeContainer(ParentContainer);

                    ChangeContainer(container);
                }
                else
                {
                    transform.SetParent(ParentContainer.ItemRoot);
                    iTween.MoveTo(gameObject, ParentContainer.ItemRoot.position, TimeMove);
                }
            }
            else
            {
                ChangeContainer(container);
            }
        }
    }

    private void ChangeContainer(DragAndDropContainer container)
    {
        ParentContainer = container;
        if (ParentContainer != null)
        {
            transform.SetParent(ParentContainer.ItemRoot);
            iTween.MoveTo(gameObject, ParentContainer.ItemRoot.position, TimeMove);
        }
    }
}