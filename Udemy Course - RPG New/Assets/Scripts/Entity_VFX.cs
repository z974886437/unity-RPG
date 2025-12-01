using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    private SpriteRenderer sr;// 声明SpriteRenderer变量，用于操作精灵的材质和渲染效果
    
    [Header("On Damage VFX")]
    [SerializeField] private Material onDamageMaterial;//在损坏材料上
    [SerializeField] private float onDamageVfxDuration = 0.2f;//伤害视觉效果持续时间
    private Material originalMaterial;//原始材料
    private Coroutine onDamageVfxCoroutine;//关于损坏视觉特效协程

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();// 获取子物体上的SpriteRenderer组件
        originalMaterial = sr.material;// 记录原始材质
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
