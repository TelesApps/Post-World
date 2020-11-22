using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ColonistData;
using static Skill;

public class Colonist : MonoBehaviour
{
    Camera cam;
    // RenderTexture is used to display GameObject as a UI Raw Image.
    RenderTexture RT;
    SpriteRenderer backGround;
    ColonistData CData = new ColonistData();
    Animator Animation;

    private void Awake()
    {
        Animation = GetComponent<Animator>();
        List<Transform> list = new List<Transform>(gameObject.GetComponentsInChildren<Transform>());
        Transform obj = list.Find(j => j.gameObject.name == "BackgroundImg");
        backGround = obj.GetComponent<SpriteRenderer>();
        cam = GetComponentInChildren<Camera>();
        // The RenderTexture size has to be the same dimetion as the Raw Image Rect Size
        RT = new RenderTexture(100, 125, 1);
        cam.targetTexture = RT;
    }
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public RenderTexture getRenderTexture()
    {
        return RT;
    }

    public ColonistData getColonistData()
    {
        return this.CData;
    }

    public void onSettleInColony(Colony colony)
    {
        //#TODO Add carrying Items Feature to Colonists which he would bring to colony uppon setteling
        CData.ColonyResidence = colony;
        CData.colonistStatus = ColonistData.ColonistStatus.idle;
    } 

    public float getProduction(SkillSlug skill)
    {
        return this.CData.getProduction(skill);
    }

    public void addToColonistSkill(SkillSlug slug, float amount = 0.01f)
    {
        Skill skill = CData.Skills.Find(s => s.skillSlug == slug);
        skill.currentExp += amount;
    }

    public void setColonistStatus(ColonistStatus status, string desc)
    {
        this.CData.colonistStatus = status;
        this.CData.colStatusDesc = desc;
    }

    //#TODO Maybe move this to its own script calle ColonistDisplay
    public void setAnimation(string animation)
    {
        Animation.Play(animation, 0);
    }



}
