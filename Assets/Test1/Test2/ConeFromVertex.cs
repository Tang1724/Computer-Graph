using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class ConeFromVertex : MonoBehaviour
{
    public Light spotLight; // 绑定一个SpotLight
    public Transform startPoint; // 圆锥的顶点位置（绑定的点）
    public int segments = 36; // 圆锥底面的分段数（越多越圆滑）

    private ProBuilderMesh coneMesh; // ProBuilder 网格
    private MeshCollider coneCollider; // 圆锥的碰撞体

    private float previousRange; // 用于记录 SpotLight 的 range
    private float previousAngle; // 用于记录 SpotLight 的 spotAngle

    void Start()
    {
        if (spotLight == null || spotLight.type != LightType.Spot)
        {
            Debug.LogError("请绑定一个SpotLight并确保其类型为Spot！");
            return;
        }

        if (startPoint == null)
        {
            Debug.LogError("请绑定一个点作为圆锥的起点！");
            return;
        }

        // 初始化圆锥
        GenerateCone();
        previousRange = spotLight.range;
        previousAngle = spotLight.spotAngle;
    }

    void Update()
    {
        // 检查 SpotLight 参数是否发生了变化
        if (Mathf.Abs(spotLight.range - previousRange) > 0.01f || Mathf.Abs(spotLight.spotAngle - previousAngle) > 0.01f)
        {
            UpdateCone(); // 更新圆锥
            previousRange = spotLight.range; // 记录当前 range
            previousAngle = spotLight.spotAngle; // 记录当前 spotAngle
        }
    }

    private void GenerateCone()
    {
        // 使用 ShapeGenerator 创建锥体，并将其附加到当前物体
        coneMesh = ShapeGenerator.CreateShape(ShapeType.Cone);
        coneMesh.transform.parent = null; // 移除原来的父物体
        coneMesh.gameObject.name = "ConeMesh"; // 重命名为 "ConeMesh"

        // 将生成的网格附加到当前物体
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mf.sharedMesh = coneMesh.GetComponent<MeshFilter>().sharedMesh;
        mr.material = new Material(Shader.Find("Standard"));

        // 添加 MeshCollider 并初始化
        coneCollider = gameObject.AddComponent<MeshCollider>();
        coneCollider.convex = true; // 设置为凸形碰撞体，适用于大多数情况

        // 销毁原始 ProBuilder 网格对象
        Destroy(coneMesh.gameObject);

        // 初次更新圆锥的大小和方向
        UpdateCone();
    }

    private void UpdateCone()
    {
        // 获取 SpotLight 参数
        float range = Mathf.Max(spotLight.range - 2f, 0.1f); // 圆锥的高度
        float angle = spotLight.spotAngle * 0.5f; // SpotLight 的半角
        float radius = Mathf.Tan(Mathf.Deg2Rad * angle) * range + 2f; // 根据公式计算底面半径

        // 保留父物体的位置
        Vector3 parentPosition = transform.parent != null ? transform.parent.position : Vector3.zero; // 父物体的位置
        transform.position = parentPosition; // 保留父物体的位置

        // 计算局部位置和旋转
        Vector3 localPosition = transform.InverseTransformPoint(startPoint.position); // 顶点在父物体的局部空间位置
        Quaternion localRotation = Quaternion.Inverse(transform.rotation) * spotLight.transform.rotation; // 保留父物体旋转的局部旋转

        // 更新当前物体的局部位置和旋转
        transform.localPosition = localPosition; // 设置局部位置
        transform.localRotation = localRotation; // 设置局部旋转

        // 获取父物体的全局缩放
        Vector3 parentScale = transform.lossyScale;

        // 设置圆锥的缩放，抵消父物体缩放的影响
        transform.localScale = new Vector3(
            radius / parentScale.x, // 消除父物体在 X 轴上的缩放影响
            range / parentScale.y,  // 消除父物体在 Y 轴上的缩放影响
            radius / parentScale.z  // 消除父物体在 Z 轴上的缩放影响
        );

        // 刷新 MeshCollider 的碰撞体
        coneCollider.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
    }
}