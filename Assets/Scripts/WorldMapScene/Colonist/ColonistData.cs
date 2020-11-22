using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Skill;

public class ColonistData
{
    public enum ColonistStatus { idle, exploring_idle, exploring, building, deceased, scavanging, working }
    // ColonistData
    public ColonistStatus colonistStatus { get; set; }
    public string colStatusDesc = "";
    public string ColonistId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Colony ColonyResidence { get; set; }
    public WorkSlot AssignedWorkslot { get; set; }
    //#TODO Add Sickness Stats and Logic
    public float EnergyLvl = 1;
    public float BaseProduction = 0.5f;

    float Food = 1;
    float CoreHappiness = 0.5f;
    float HappinessModifers = 0;
    float Happiness = 0.01f;
    float Sadness = -0.01f;
    float TempHappiness = 1;
    public float M_Efficiency { get; set; }
    public float T_Efficiency { get; set; }

    public List<Skill> Skills = new List<Skill>();
    public ColonistData()
    {
        this.ColonistId = System.Guid.NewGuid().ToString();
        foreach (SkillSlug slug in System.Enum.GetValues(typeof(SkillSlug)))
        {
            Skills.Add(new Skill(slug));
        }
        this.colonistStatus = ColonistStatus.exploring_idle;
        // #TODO add logic for generating names;
        this.FirstName = "John";
        this.LastName = "Smith";
    }

    public void setColonistVariables()
    {
        EnergyLvl = 1 * Food;

        Sadness -= (.1f - Food);
        HappinessModifers = Random.Range(Sadness, Happiness);
        CoreHappiness = (CoreHappiness + HappinessModifers) * TempHappiness;
        M_Efficiency = 1 * (CoreHappiness + 0.5f);
    }

    public float getProduction(SkillSlug skill)
    {
        Skill inUse = Skills.Find(s => s.skillSlug == skill);
        float production = BaseProduction + (inUse.currentExp + T_Efficiency) * M_Efficiency * EnergyLvl;
        return production;
    }

}
