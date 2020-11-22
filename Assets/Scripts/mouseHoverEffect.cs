using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseHoverEffect : MonoBehaviour
{
    SpriteRenderer sprite;
    Color32 defaultColor = new Color32(255, 255, 255, 255);
    Color32 hoverColor = new Color32(200, 200, 200, 255);
    Tile tile;
    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        tile = GetComponent<Tile>();
    }

    void OnMouseOver()
    {
        // Check if Mouse is over an UI Panel, if its not then perform click action
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (tile == null)
            {
                sprite.color = this.hoverColor;
            }
            else
            {
                if (tile.getTileData().isVisible)
                {
                    sprite.color = this.hoverColor;
                }
            }
        }

    }

    void OnMouseExit()
    {
        if (tile == null)
        {
            sprite.color = this.defaultColor;
        } else
        {
            if (tile.getTileData().isVisible)
            {
                sprite.color = this.defaultColor;
            }
        }
    }
}
