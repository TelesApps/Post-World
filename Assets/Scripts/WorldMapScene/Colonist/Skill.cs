using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public enum SkillSlug { Construction, Scavanging, Farming, Lumber_Mill }
    public SkillSlug skillSlug { get; set; }
    public string Name { get; set; }
    public int skillLvl = 0;
    // #TODO come up with a sliding scale for experience and levels to reach master.
    public float experienceLimit = 500;
    public float currentExp { get; set; }

    public Skill(SkillSlug slug)
    {
        this.skillSlug = slug;
        this.Name = slug.ToString();
        this.currentExp = 0;
    }
}
