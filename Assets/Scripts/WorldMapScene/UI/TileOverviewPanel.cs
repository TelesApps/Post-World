using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileOverviewPanel : MonoBehaviour
{
    Tile SelectedTile;
    Colony SelectedColony;

    [SerializeField] GameController GC;
    [SerializeField] Image TileImg;
    [SerializeField] TextMeshProUGUI colonyName;
    [SerializeField] TextMeshProUGUI BuildingsNumberTxt;
    [SerializeField] TextMeshProUGUI ColonistsNumberTxt;
    [SerializeField] TextMeshProUGUI PartiesNumberTxt;
    [SerializeField] TextMeshProUGUI ViewBtnTxt;
    Slider exploredSlider;
    [SerializeField] TextMeshProUGUI exploredTxt;
    List<Resource> wildResources;
    // Start is called before the first frame update
    void Start()
    {
        exploredSlider = GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        //#TODO for performance this could probably be put into setSelectedTile Function
        if(GC.isSummerSeason)
            TileImg.sprite = SelectedTile.getTileData().summerImg;
        else TileImg.sprite = SelectedTile.getTileData().summerImg;
        colonyName.text = this.SelectedColony.getColonyData().ColonyName;
        BuildingsNumberTxt.text = this.SelectedColony.getColonyData().AllBuildings.Count.ToString();
        ColonistsNumberTxt.text = this.SelectedColony.getColonyData().AllColonists.Count.ToString();
        PartiesNumberTxt.text = this.SelectedTile.getOnTilePartiesCount().ToString();
        if (SelectedColony.getColonyData().IsColonized)
            ViewBtnTxt.text = "View Colony";
        else ViewBtnTxt.text = "View Region";
        float percentExplored = SelectedTile.getTileData().exploredPercent;
        exploredSlider.value = percentExplored;
        exploredTxt.text = $"{System.Math.Round(percentExplored, 1).ToString()}% Explored";
    }

    public void setSelectedTile(Tile tile)
    {
        this.SelectedTile = tile;
        this.SelectedColony = tile.getColony();
        TileImg.sprite = tile.getTileData().summerImg;
    }
}
