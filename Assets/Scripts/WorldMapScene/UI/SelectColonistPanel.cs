using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Skill;

public class SelectColonistPanel : MonoBehaviour
{
    [SerializeField] Transform ScrollViewContent;
    [SerializeField] GameObject ColonistUISelectPanelPrefab;
    [SerializeField] Toggle showIdle;
    public delegate void selectColonist(Colonist colonist);
    public static event selectColonist onSelected;
    List<Colonist> colonists;
    SkillSlug skillToUse;

    public void onStartSelection(List<Colonist> colonists, SkillSlug skill)
    {
        this.colonists = colonists;
        this.skillToUse = skill;
        resetContentView();
        this.populateSelectionList();
    }

    public void populateSelectionList()
    {
        List<Colonist> colonistToDisplay = this.colonists;
        // #TODO Add logic to order it and filter it according to skill level.
        // First delete all Panels
        foreach (Transform child in ScrollViewContent)
        {
            Destroy(child.gameObject);
        }
        if (this.showIdle.isOn)
        {
            List<Colonist> idleColonists = this.colonists.FindAll(c => c.getColonistData().colonistStatus == ColonistData.ColonistStatus.idle);
            colonistToDisplay = idleColonists;
        }
        // Repopulate the scrollview
        foreach (Colonist colonist in colonistToDisplay)
        {
            GameObject objPanel = Instantiate(ColonistUISelectPanelPrefab, ScrollViewContent);
            setColonistPanelData(objPanel, colonist, skillToUse);
            Button selectButton = objPanel.GetComponent<Button>();
            selectButton.onClick.AddListener(() => onColonistSelected(colonist));
        }
    }

    public void onCancelSelection()
    {
        if (onSelected != null)
            onSelected(null);
        this.gameObject.SetActive(false);
        this.showIdle.isOn = true;
    }
    public void onColonistSelected(Colonist colonist)
    {
        if (onSelected != null)
            onSelected(colonist);
        this.gameObject.SetActive(false);
        this.showIdle.isOn = true;
    }

    void resetContentView()
    {
        foreach (Transform child in ScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void setColonistPanelData(GameObject panel, Colonist colonist, SkillSlug skill)
    {
        ColonistData data = colonist.getColonistData();
        List<TextMeshProUGUI> panelTexts = new List<TextMeshProUGUI>(panel.GetComponentsInChildren<TextMeshProUGUI>());
        Skill colonistSkill = data.Skills.Find(sk => sk.skillSlug == skill);
        RawImage panelImg = panel.GetComponentInChildren<RawImage>();
        Slider skillLvlSlider = panel.GetComponentInChildren<Slider>();
        TextMeshProUGUI SkillLvlText = panelTexts.Find(p => p.name == "SkillLvlText");
        TextMeshProUGUI nameTxt = panelTexts.Find(p => p.name == "colonistName");
        TextMeshProUGUI currentTaskTxt = panelTexts.Find(p => p.name == "currentTaskTxt");

        panelImg.texture = colonist.getRenderTexture();
        skillLvlSlider.value = colonistSkill.currentExp / colonistSkill.experienceLimit;
        SkillLvlText.text = System.Math.Round(colonistSkill.currentExp, 2).ToString();
        nameTxt.text = $"{data.FirstName} {data.LastName}";
        currentTaskTxt.text = data.colStatusDesc;
    }
}
