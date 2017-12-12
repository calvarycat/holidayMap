using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoGridCellSize : MonoBehaviour
{
    public RectTransform ViewPort;
    public GridLayoutGroup Grid;
    public bool EffectHorizontal;
    public bool EffectVertical;
    public Vector2 Offset;

    private bool _isDone;

    private void Awake()
    {
        if (Grid != null && ViewPort != null)
        {
            _isDone = false;
        }
        else
        {
            _isDone = true;
        }
    }

    private void Update()
    {
        if (!_isDone)
        {
            if (ViewPort.rect.width != 0 && ViewPort.rect.height != 0)
            {
                Vector2 target = Vector2.zero;

                if (EffectHorizontal)
                {
                    target.x = ViewPort.rect.width + Offset.x;
                }
                else
                {
                    target.x = Grid.cellSize.x;
                }

                if (EffectVertical)
                {
                    target.y = ViewPort.rect.height + Offset.y;
                }
                else
                {
                    target.y = Grid.cellSize.y;
                }

                Grid.cellSize = target;
                _isDone = true;
            }
        }
    }
}