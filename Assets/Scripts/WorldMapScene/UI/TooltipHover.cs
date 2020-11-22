using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isResource;
    public string HoverMessage = "";
    UIController UI;
    TooltipPanel Tooltip;
    void Start()
    {
        UI = GameObject.Find("GameController").GetComponent<UIController>();
        Tooltip = UI.TooltipPanel.GetComponent<TooltipPanel>();
        if(isResource)
        {
            ResourceUI resourceUI = GetComponent<ResourceUI>();
            this.HoverMessage = resourceUI.getResource().Name;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 mouse = eventData.pointerCurrentRaycast.screenPosition;
        Tooltip.useTooltip(true, new Vector2(mouse.x, mouse.y + 40), HoverMessage);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Vector3 mouse = eventData.pointerCurrentRaycast.screenPosition;
        Tooltip.useTooltip(false, new Vector2(mouse.x, mouse.y + 20), HoverMessage);
    }
}
