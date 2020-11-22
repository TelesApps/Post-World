using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenTopUIPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ColonyNameTxt;
    [SerializeField] Transform HUDResourcePanel;
    [SerializeField] GameObject resourceUIPrefab;
    [SerializeField] GameObject openResourcesBtnPrefab;
    List<Resource> resourcesDisplaying = new List<Resource>();
    void Start()
    {
        // Default settings at the beginning of game
        resourcesDisplaying.Add(new Resource(ResourceDatabase.AllResources.Find(r => r.NameSlug == ResourceDatabase.ResourceSlug.wood)));
        resourcesDisplaying.Add(new Resource(ResourceDatabase.AllResources.Find(r => r.NameSlug == ResourceDatabase.ResourceSlug.berries)));
    }

    void Update()
    {
        
    }

    public void onColonySelected(ColonyData colonyData)
    {
        ColonyNameTxt.text = colonyData.ColonyName;
        // First destroy all children of HUDResourcePanel
        foreach (Transform child in HUDResourcePanel)
        {
            Destroy(child.gameObject);
        }
        // Instantiate Button at beginning of list;
        Instantiate(openResourcesBtnPrefab, HUDResourcePanel);
        // Instantiate every panel that is under resourceToDisplay
        foreach (Resource res in resourcesDisplaying)
        {
            GameObject obj = Instantiate(resourceUIPrefab, HUDResourcePanel);
            ResourceUI resUI = obj.GetComponent<ResourceUI>();
            Resource colRes = colonyData.AllResources.Find(r => r.NameSlug == res.NameSlug);
            if (colRes != null)
            {
                resUI.onInitResourcePanel(colRes);
            }
            else resUI.onInitResourcePanel(res);
        }
    }
}
