using System;
using UnityEngine;


[CreateAssetMenu(menuName = "RPG Setup/Skill Data",fileName = "Skill data -")]// 创建 ScriptableObject 以便在 Unity 编辑器中创建技能数据
public class Skill_DataSO : ScriptableObject
{
    public int cost;// 技能消耗
    public bool unlockedByDefault;
    public SkillType skillType;
    public UpgradeData upgradeData;
    
    [Header("Skill description")]
    public string displayName;// 技能名称
    [TextArea]// 在 Unity 编辑器中显示为多行文本框
    public string description;// 技能描述
    public Sprite icon;// 技能图标
}

[Serializable]
public class UpgradeData
{
    public SkillUpgradeType upgradeType;
    public float cooldown;
    public DamageScaleData damageScaleData;
}