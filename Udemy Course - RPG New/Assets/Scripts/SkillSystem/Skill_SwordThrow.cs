using UnityEngine;

public class Skill_SwordThrow : Skill_Base
{
    private SkillObject_Sword currentSword;
   
    [Header("Regular Sword Upgrade")]
    [SerializeField] private GameObject swordPrefab;
    [Range(0,10)]
    [SerializeField] private float throwPower = 5;
    
    
    [Header("Trajectory prediction")]
    [SerializeField] private GameObject predictionDot;//预测点
    [SerializeField] private int numberOfDots = 20;//点数
    [SerializeField] private float spaceBetweenDots = 0.05f;//点之间的空间
    private float swordGravity;//剑的重力
    private Transform[] dots;
    private Vector2 confirmedDirection;//确认方向
    
    // Unity 生命周期函数：在对象被创建时最先调用，用于初始化必要数据
    protected override void Awake()
    {
        base.Awake();
        swordGravity = swordPrefab.GetComponent<Rigidbody2D>().gravityScale;
        dots = GenerateDots();// 生成并缓存轨迹预测用的小圆点，用于后续实时更新位置
    }

    // 检查技能是否可以使用（重写基类方法）
    public override bool CanUseSkill()
    {
        if (currentSword != null)// 如果当前场上已经有一把飞剑存在
        {
            currentSword.GetSwordBackToPlayer();// 告诉现有飞剑回到玩家手中，而不是生成新的飞剑
            return false;// 返回 false，表示本次技能无法再次释放
        }
        
        return base.CanUseSkill(); // 否则调用基类判断逻辑，继续判断技能是否可用（如冷却、魔力等）
    }

    // 对外公开的方法：用于真正执行“丢剑”行为
    public void ThrowSword()
    {
        GameObject newSword = Instantiate(swordPrefab, dots[1].position, Quaternion.identity);

        currentSword = newSword.GetComponent<SkillObject_Sword>();
        currentSword.SetupSword(this,GetThrowPower());
    }

    private Vector2 GetThrowPower() => confirmedDirection * (throwPower * 10);
    
    // 根据输入方向预测剑的飞行轨迹，并更新所有预测点的位置
    public void PredictTrajectory(Vector2 direction)
    {
        for (int i = 0; i < dots.Length; i++) // 遍历所有轨迹点
        {
            dots[i].position = GetTrajectoryPoint(direction, i * spaceBetweenDots);// 根据当前点的时间参数，计算该点在轨迹中的预测位置
        }
    }

    // 根据方向和时间，计算某一时刻剑所在的世界坐标位置
    private Vector2 GetTrajectoryPoint(Vector2 direction, float t)
    {
        float scaledThrowPower = throwPower * 10;// 对投掷力度进行放大，避免数值过小导致轨迹不明显
        
        // This gives us the initial velocity - the starting speed and direction of the throw 这给出了初始速度——投掷的起始速度和方向
        Vector2 initialVelocity = direction * scaledThrowPower;// 计算初始速度：方向 × 投掷力度，决定起飞方向和初速度大小

        // Gravity pulls the sword down over time. The longer it's in the air,the more it drops.重力会随着时间拉下剑。它在空中停留的时间越长，掉落得越多。
        Vector2 gravityEffect = 0.5f * Physics2D.gravity * swordGravity * (t * t);// 计算重力影响：0.5gt²，用于模拟物体随时间下坠的效果

        // We calculate how far the sword will travel after time "t"我们计算剑在时间“t”之后能行多远
        // by combining the initial throw direction with the gravity pull, 通过将初始抛掷方向与引力拉力结合，
        Vector2 predictedPoint = (initialVelocity * t) + gravityEffect;// 将初速度位移和重力位移叠加，得到在时间 t 后的相对位移

        Vector2 playerPosition = transform.root.position;// 获取玩家的世界坐标，作为轨迹计算的起点

        return playerPosition + predictedPoint;// 返回最终世界坐标下的预测位置
    }
    
    // 记录玩家最终确认的投掷方向，用于真正丢剑时使用
    public void ConfirmTrajectory(Vector2 direction) => confirmedDirection = direction;
    
    // 控制所有轨迹预测点的显示或隐藏
    public void EnableDots(bool enable)
    {
        foreach (Transform t in dots)// 遍历所有预测点
            t.gameObject.SetActive(enable);// 根据参数统一设置激活状态
    }
    
    // 生成用于轨迹预测的小圆点，并返回其 Transform 数组
    private Transform[] GenerateDots()
    {
        Transform[] newDots = new Transform[numberOfDots];// 创建一个数组，用于存储所有预测点
    
        for (int i = 0; i < numberOfDots; i++)// 根据数量循环生成预测点
        {
            newDots[i] = Instantiate(predictionDot, transform.position, Quaternion.identity, transform).transform; // 实例化预测点，并设置父物体为当前对象，方便统一管理
            newDots[i].gameObject.SetActive(false);// 初始状态隐藏预测点，避免一开始就显示
        }
    
        return newDots;// 返回生成好的预测点数组
    }
}
