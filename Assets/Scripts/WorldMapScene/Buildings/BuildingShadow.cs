using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingShadow : MonoBehaviour
{
    [SerializeField] GameObject BuildingArea;
    [SerializeField] SpriteRenderer BuildingImg;
    [SerializeField] SpriteRenderer BuildingAreaImg;
    [SerializeField] Color32 canBuildColor;
    [SerializeField] Color32 cannotBuildColor;
    public bool canBuild = true;

    Colony Colony;
    BuildingData buildingData;
    Camera RegionCamera;
    public void onInit(Colony colony, BuildingData building)
    {
        this.Colony = colony;
        this.buildingData = building;
        this.BuildingImg.sprite = building.imgSprite;
        // #TODO Add logic to adjust the Building Area according to the size of the image as needed
    }

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("TileRegionCamera");
        RegionCamera = obj.GetComponent<Camera>();
    }

    private void Update()
    {
        Vector3 mousePosition = RegionCamera.ScreenToWorldPoint(Input.mousePosition);
        //Rigidbody2D rb = buildingShadow.GetComponent<Rigidbody2D>();
        //rb.MovePosition(mousePosition);
        gameObject.transform.position = mousePosition;
        // #TODO Add more logic to this, perhaps check again if there are enough resources etc
        if(Input.GetMouseButtonDown(0))
        {
            buildingData.worldLocation = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -1);
            this.Colony.addNewBuilding(buildingData);
            Destroy(this.gameObject);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(this.gameObject);
        }
    }

    public void setCanBuildIndicator(bool canBuild)
    {
        if(canBuild)
        {
            BuildingAreaImg.color = canBuildColor;
        } else
        {
            BuildingAreaImg.color = cannotBuildColor;
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.tag == "WaterTilemap" || collision.tag == "Buildings")
    //    {
    //        BuildingAreaImg.color = cannotBuildColor;
    //        canBuild = false;
    //    }
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "WaterTilemap" || collision.tag == "Buildings")
        {
            BuildingAreaImg.color = canBuildColor;
            canBuild = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "WaterTilemap" || collision.tag == "Buildings")
        {
            BuildingAreaImg.color = cannotBuildColor;
            canBuild = false;
        }
    }






}
