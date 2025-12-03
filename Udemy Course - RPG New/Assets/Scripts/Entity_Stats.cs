using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat maxHealth;
    public Stat vitality;// each point gives +5 Hp
    

    public float GetMaxHealth()
    {
        float baseHp = maxHealth.GetValue();
        float bonusHp = vitality.GetValue() * 5;
        
        return baseHp + bonusHp;
    }
}
