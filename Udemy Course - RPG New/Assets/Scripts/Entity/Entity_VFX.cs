using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    protected SpriteRenderer sr;// 声明SpriteRenderer变量，用于操作精灵的材质和渲染效果
    private Entity entity;
    
    [Header("On Taking Damage VFX")]
    [SerializeField] private Material onDamageMaterial;//在损坏材料上
    [SerializeField] private float onDamageVfxDuration = 0.2f;//伤害视觉效果持续时间
    private Material originalMaterial;//原始材料
    private Coroutine onDamageVfxCoroutine;//关于损坏视觉特效协程

    [Header("On Doing Damage VFX")]
    [SerializeField] private Color hitVfxColor = Color.white;
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject cirtHitVfx;
    
    [Header("Element Colors")]
    [SerializeField] private Color chillVfx = Color.cyan;
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color shockVfx = Color.yellow;
    private Color originalHitVfxColor;
    
    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();// 获取子物体上的SpriteRenderer组件
        originalMaterial = sr.material;// 记录原始材质
        originalHitVfxColor = hitVfxColor;
    }

    // 根据元素类型播放状态效果特效
    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        if (element == ElementType.Ice)// 如果元素类型是冰霜，启动冰霜状态效果的特效协程
            StartCoroutine(PlayStatusVfxCo(duration, chillVfx)); // 播放冰霜效果特效
        
        if(element == ElementType.Fire) // 如果元素类型是火焰，启动火焰状态效果的特效协程
            StartCoroutine(PlayStatusVfxCo(duration, burnVfx));
        
        if(element == ElementType.Lightning)// 如果元素类型是雷电，启动雷电状态效果的特效协程
            StartCoroutine(PlayStatusVfxCo(duration, shockVfx));
    }

    // 停止所有的视觉效果和特效
    public void StopAllVfx()
    {
        StopAllCoroutines();// 停止所有正在运行的协程
        sr.color = Color.white;// 将SpriteRenderer的颜色恢复为白色
        sr.material = originalMaterial;// 恢复原始的材质
    }

    // 播放状态特效的协程方法，周期性地改变颜色效果
    private IEnumerator PlayStatusVfxCo(float duration, Color effectColor)
    {
        float tickInterval = 0.25f;// 定义每次切换颜色的间隔时间（0.25秒）
        float timeHasPassed = 0; // 距离开始的时间

        Color lightColor = effectColor * 1.2f; // 亮色是输入颜色的 1.2 倍
        Color darkColor = effectColor * 0.8f; // 暗色是输入颜色的 0.8 倍

        bool toggle = false; // 切换标志，用于交替显示亮色和暗色

        while (timeHasPassed < duration) // 当经过的时间小于持续时间时，继续执行
        {
            sr.color = toggle ? lightColor : darkColor;// 根据 toggle 值切换显示亮色或暗色
            toggle = !toggle; // 每次切换颜色时反转 toggle 的值
            
            yield return new WaitForSeconds(tickInterval);  // 等待一个周期后再继续执行
            timeHasPassed = timeHasPassed + tickInterval; // 增加已过去的时间
        }

        sr.color = Color.white; // 恢复为默认的颜色
    }

    // 创建命中时的视觉特效（VFX）
    public void CreateOnHitVFX(Transform target,bool isCrit,ElementType element)
    {
        GameObject hitPrefab = isCrit ? cirtHitVfx : hitVfx;// 如果是暴击，使用暴击特效；否则使用普通命中特效
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);// 在目标位置生成特效，保持原始旋转
        vfx.GetComponentInChildren<SpriteRenderer>().color = GetElementColor(element);// 修改特效的颜色为预设颜色
        
        if(entity.facingDir == -1 && isCrit)
            vfx.transform.Rotate(0,180,0); // 如果敌人面朝左（`facingDir == -1`）且为暴击，旋转特效180度
    }

    // 更新攻击命中的颜色效果，根据元素类型改变效果颜色
    public Color GetElementColor(ElementType element)
    {
        switch (element)
        {
            case ElementType.Ice: return chillVfx;
            case ElementType.Fire: return burnVfx;
            case ElementType.Lightning: return shockVfx;
            
            default: return Color.white;
        }
    }

    public void PlayOnDamageVfx()
    {
        if(onDamageVfxCoroutine != null)
            StopCoroutine(onDamageVfxCoroutine);// 如果协程已经存在，停止当前协程  
        
        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo());// 启动新的协程，播放伤害视觉效果
    }

    private IEnumerator OnDamageVfxCo()
    {
        sr.material = onDamageMaterial;// 将精灵的材质更换为伤害材质

        yield return new WaitForSeconds(onDamageVfxDuration);// 等待指定的伤害效果持续时间
        sr.material = originalMaterial; // 恢复精灵的原始材质
    }
}
