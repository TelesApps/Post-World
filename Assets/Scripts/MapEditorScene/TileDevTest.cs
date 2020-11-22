using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileDevTest : MonoBehaviour
{
    [SerializeField] Tile tile;
    [SerializeField] TextMeshProUGUI dicNameTxt;
    [SerializeField] TextMeshProUGUI neighborTxt0;
    [SerializeField] TextMeshProUGUI neighborTxt1;
    [SerializeField] TextMeshProUGUI neighborTxt2;
    [SerializeField] TextMeshProUGUI neighborTxt3;
    [SerializeField] TextMeshProUGUI neighborTxt4;
    [SerializeField] TextMeshProUGUI neighborTxt5;
    Camera WorldCamera;
    TileData data;
    string text = "";

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = GameObject.Find("Main Camera");
        WorldCamera = obj.GetComponent<Camera>();
        Canvas canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = WorldCamera;
        this.data = tile.getTileData();

        neighborTxt0.gameObject.SetActive(false);
        neighborTxt1.gameObject.SetActive(false);
        neighborTxt2.gameObject.SetActive(false);
        neighborTxt3.gameObject.SetActive(false);
        neighborTxt4.gameObject.SetActive(false);
        neighborTxt5.gameObject.SetActive(false);
        if (data.riverNumber != -1)
            text = "R";
    }

    // Update is called once per frame
    void Update()
    {
        dicNameTxt.text = text;
        //(int, int)[] neighbors = tile.getTileData().neighborHexes;
        //neighborTxt0.text = neighbors[0].Item1.ToString()+ "," + neighbors[0].Item2.ToString();
        //neighborTxt1.text = neighbors[1].Item1.ToString() + "," + neighbors[1].Item2.ToString();
        //neighborTxt2.text = neighbors[2].Item1.ToString() + "," + neighbors[2].Item2.ToString();
        //neighborTxt3.text = neighbors[3].Item1.ToString() + "," + neighbors[3].Item2.ToString();
        //neighborTxt4.text = neighbors[4].Item1.ToString() + "," + neighbors[4].Item2.ToString();
        //neighborTxt5.text = neighbors[5].Item1.ToString() + "," + neighbors[5].Item2.ToString();

    }

}
