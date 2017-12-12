using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class AdaptingEventSystemDragThreshold : MonoBehaviour
{
    private int _target;
    private bool _isNeedToCheck;

    private void Awake()
    {
        int defaultValue = EventSystem.current.pixelDragThreshold;
        _target = Mathf.Max(defaultValue, (int)(defaultValue * Screen.dpi / 160f));
        EventSystem.current.pixelDragThreshold = _target;
        _isNeedToCheck = true;
    }

    private void Update()
    {
        if (_isNeedToCheck)
        {
            if (EventSystem.current.pixelDragThreshold != _target)
            {
                EventSystem.current.pixelDragThreshold = _target;
            }
            else
            {
                _isNeedToCheck = false;
            }
        }
    }
}