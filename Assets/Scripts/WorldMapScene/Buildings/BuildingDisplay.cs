using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingDisplay : MonoBehaviour
{
    Building building;
    [SerializeField] Sprite ConstructionImg;
    [SerializeField] SpriteRenderer DisplayingImg;
    Sprite BuildingImg;
    [SerializeField] Canvas UICanvas;
    [SerializeField] Slider ProgressSlider;
    [SerializeField] TextMeshProUGUI ProgressText;
    [SerializeField] GameObject IndicationIcon1;
    [SerializeField] GameObject IndicationIcon2;

    Camera RegionCamera;

    private void Start()
    {
        building = GetComponent<Building>();
        GameObject obj = GameObject.FindGameObjectWithTag("TileRegionCamera");
        RegionCamera = obj.GetComponent<Camera>();
        UICanvas.worldCamera = RegionCamera;
        ProgressSlider.value = building.bData.ConstructionRPCReceived;
        ProgressText.text = $"{System.Math.Round(building.bData.ConstructionRPCReceived, 1)} % Complete";
        DisplayingImg.sprite = ConstructionImg;
    }

    private void FixedUpdate()
    {
        if(!building.bData.isComplete)
        {
            float progressValue = getProgressBarValue(building.bData.ConstructionRPCReceived, building.bData.ConstructionRPCRequired);
            // MathfLerp makes the bar move more smoothly.
            ProgressSlider.value = Mathf.Lerp(ProgressSlider.value, progressValue, 8 * Time.deltaTime);
            ProgressText.text = $"{System.Math.Round(progressValue * 100, 2)}% Complete";
        }
    }

    public void CompleteConstruction()
    {
        IndicationIcon1.SetActive(false);
        DisplayingImg.sprite = BuildingImg;
        ProgressSlider.gameObject.SetActive(false);
    }

    public void setBuildingImg(Sprite img)
    {
        this.BuildingImg = img;
    }

    private float getProgressBarValue(float value, float max)
    {
        return (value / max);
    }

}
