using System.Collections;
using UnityEngine;

public class Player_VFX : Entity_VFX
{
    [Header("Image Echo VFX")]
    [Range(0.01f,0.2f)]
    [SerializeField] private float imageEchoInterval = 0.05f;
    [SerializeField] private GameObject imageEchoPrefab;
    private Coroutine imageEchoCo;

    // 公开方法，用于启动图像回声效果
    public void DoImageEchoEffect(float duration)
    {
        if(imageEchoCo != null)// 如果已有的回声协程正在执行，则停止它
            StopCoroutine(imageEchoCo);

        imageEchoCo = StartCoroutine(ImageEchoEffectCo(duration)); // 启动一个新的回声效果协程
    }

    // 协程，负责执行图像回声效果的实际逻辑
    private IEnumerator ImageEchoEffectCo(float duration)
    {
        float timeTracker = 0;// 记录经过的时间

        while (timeTracker < duration) // 循环，直到经过的时间达到或超过设定的持续时间
        {
            CreateImageEcho(); // 创建一次图像回声效果
            
            yield return new WaitForSeconds(imageEchoInterval);// 等待一定的时间间隔后再进行下一次回声效果
            timeTracker = timeTracker + imageEchoInterval; // 更新已经经过的时间
        }
    }

    // 创建单个图像回声效果的实例
    private void CreateImageEcho()
    {
        GameObject imageEcho = Instantiate(imageEchoPrefab, transform.position, transform.rotation);// 实例化一个新的图像回声对象
        imageEcho.GetComponentInChildren<SpriteRenderer>().sprite = sr.sprite; // 设置回声图像的 Sprite，确保回声图像与原图像相同
    }

}
