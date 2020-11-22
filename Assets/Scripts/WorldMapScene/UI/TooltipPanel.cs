using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textGUI;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void useTooltip(bool isActive, Vector2 location, string message)
    {
        if(isActive)
        {
            gameObject.SetActive(true);
            transform.position = location;
            textGUI.text = message;
        }
        else
        {
            gameObject.SetActive(false);
            textGUI.text = "";
        }
    }
}
