using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyPanel : MonoBehaviour
{
    [SerializeField] RawImage img1;
    [SerializeField] RawImage img2;
    [SerializeField] RawImage img3;
    [SerializeField] Button viewAllBtn;
    Slider currentActivitySlider;
    [SerializeField] TextMeshProUGUI currentActivityTxt;
    UIController UI;

    Party Party;
    List<Colonist> PartyMembers = new List<Colonist>();
    void Start()
    {
        currentActivitySlider = GetComponentInChildren<Slider>();
        currentActivitySlider.gameObject.SetActive(false);
        GameObject GC = GameObject.Find("GameController");
        UI = GC.GetComponent<UIController>();
    }

    void Update()
    {
        if(PartyMembers.Count > 0)
        {
            // #TODO Calculate Party Stats here
            if(PartyMembers[0] != null)
            {
                this.img1.gameObject.SetActive(true);
                this.img1.texture = PartyMembers[0].getRenderTexture();
            }
            if (PartyMembers.Count > 1 && PartyMembers[1] != null)
            {
                this.img2.gameObject.SetActive(true);
                this.img2.texture = PartyMembers[1].getRenderTexture();
            }
            if (PartyMembers.Count > 2 && PartyMembers[2] != null)
            {
                this.img3.gameObject.SetActive(true);
                this.img3.texture = PartyMembers[2].getRenderTexture();
            }
        }
        if (PartyMembers.Count > 3) viewAllBtn.gameObject.SetActive(true);
        if(Party.partyStatus == Party.PartyStatus.Exploring)
        {
            currentActivitySlider.maxValue = 100;
            currentActivitySlider.value = Party.partyLocation.getTileData().exploredPercent;
        }
    }

    public void setColonists(Party party)
    {
        this.Party = party;
        this.PartyMembers = party.PartyMembers;
    }

    public void onSettleParty()
    {
        UI.onSettleParty(this.Party);
    }

    public void OnSetPartyToExplore()
    {
        Party.setPartyToExplore();
        currentActivityTxt.text = "Exploring";
        currentActivitySlider.gameObject.SetActive(true);
    }
}
