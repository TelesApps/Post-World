using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    Image img;
    TextMeshProUGUI amountTxt;
    Resource Res;

    private void Awake()
    {

        Image[] images = GetComponentsInChildren<Image>();
        this.img = images[1];
        amountTxt = GetComponentInChildren<TextMeshProUGUI>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        if(Res != null)
        {
            amountTxt.text = Mathf.RoundToInt(this.Res.Amount).ToString();
        }
    }

    public void onInitResourcePanel(Resource res)
    {
        this.img.sprite = res.imgSprite;
        this.Res = res;
    }

    public Resource getResource()
    {
        return this.Res;
    }

    public TextMeshProUGUI getTextMeshProRef()
    {
        return amountTxt;
    }
}
