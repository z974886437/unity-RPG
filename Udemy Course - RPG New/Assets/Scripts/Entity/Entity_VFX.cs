using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    private SpriteRenderer sr;// 声明SpriteRenderer变量，用于操作精灵的材质和渲染效果
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
    
    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();// 获取子物体上的SpriteRenderer组件
        originalMaterial = sr.material;// 记录原始材质
    }

    // 创建命中时的视觉特效（VFX）
    public void CreateOnHitVFX(Transform target,bool isCrit)
    {
        GameObject hitPrefab = isCrit ? cirtHitVfx : hitVfx;// 如果是暴击，使用暴击特效；否则使用普通命中特效
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);// 在目标位置生成特效，保持原始旋转
        vfx.GetComponentInChildren<SpriteRenderer>().color = hitVfxColor;// 修改特效的颜色为预设颜色
        
        if(entity.facingDir == -1 && isCrit)
            vfx.transform.Rotate(0,180,0); // 如果敌人面朝左（`facingDir == -1`）且为暴击，旋转特效180度
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
