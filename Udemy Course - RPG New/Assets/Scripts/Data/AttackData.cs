using System;
using UnityEngine;

[Serializable]
public class AttackData
{
    public float phyiscalDamage;
    public float elementalDamage;
    public bool isCrit;
    public ElementType element;

    public ElementalEffectData effectData;



    // 构造函数：初始化攻击数据，计算物理和元素伤害
    public AttackData(Entity_Stats entityStats, DamageScaleData scaleData)
    {
        phyiscalDamage = entityStats.GetPhyiscalDamage(out isCrit, scaleData.phyiscal); // 计算物理伤害，判断是否暴击，依据scaleData.phyiscal进行加成
        elementalDamage = entityStats.GetElementalDamage(out element, scaleData.elemental);// 计算元素伤害，确定元素类型，依据scaleData.elemental进行加成
        
        effectData = new ElementalEffectData(entityStats, scaleData);// 初始化元素效果数据，传入角色属性和伤害加成数据
    }
}
