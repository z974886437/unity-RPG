using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [SerializeField] private Transform background;//背景
    [SerializeField] private float parallaxMultiplier;//视差乘数
    [SerializeField] private float imageWidthOffset = 10;//图像宽度偏移

    private float imageFullWidth;// 图像的完整宽度
    private float imageHalfWidth; // 图像的半宽度

    // 计算背景图像的宽度，获取 SpriteRenderer 的边界宽度
    public void CalculateImageWidth()
    {
        imageFullWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;// 获取背景图像的完整宽度
        imageHalfWidth = imageFullWidth / 2;// 计算图像的半宽度
    }

    // 移动背景层，根据相机的移动量来平移背景
    public void Move(float distanceToMove)
    {
        //background.position = background.position + new Vector3(distanceToMove * parallaxMultiplier, 0);
        background.position += Vector3.right * (distanceToMove * parallaxMultiplier);// 根据视差乘数平移背景层，distanceToMove 是摄像机移动的距离
    }

    // 循环背景图像，如果背景图像的边界超出了摄像机视野的范围，则将其位置重置，达到无缝循环的效果
    public void LoopBackground(float cameraLefteEdge, float cameraRightEdge)
    {
        float imageRightEdge = (background.position.x + imageHalfWidth) - imageWidthOffset; // 获取背景图像的右边界
        float imageLeftEdge = (background.position.x - imageHalfWidth) + imageWidthOffset; // 获取背景图像的左边界

        if (imageRightEdge < cameraLefteEdge) // 如果背景图像的右边界超出了摄像机的左边界，背景需要重新定位到屏幕右边
            background.position += Vector3.right * imageFullWidth;
        else if(imageLeftEdge > cameraRightEdge) // 如果背景图像的左边界超出了摄像机的右边界，背景需要重新定位到屏幕左边
            background.position += Vector3.right * -imageFullWidth;
    }
}
