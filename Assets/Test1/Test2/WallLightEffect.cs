using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WallLightEffect : MonoBehaviour
{
    public LightImpactTracker lightImpactTracker; // 引用 LightImpactTracker 脚本
    public Color gizmoColor = Color.red; // 光圈的颜色
    public int circleSegments = 36; // 光圈圆的分段数

    private void OnDrawGizmos()
    {
        if (lightImpactTracker != null && lightImpactTracker.isHitByLight)
        {
            // 设置 Gizmos 的颜色
            Gizmos.color = gizmoColor;

            // 绘制光圈范围（在墙面上）
            DrawCircleOnWall(
                lightImpactTracker.lightImpactPosition,
                lightImpactTracker.lightImpactNormal,
                lightImpactTracker.lightImpactRadius,
                circleSegments
            );
        }
    }

    // 在墙面上绘制光圈
    private void DrawCircleOnWall(Vector3 center, Vector3 normal, float radius, int segments)
    {
        // 计算圆的点
        Vector3[] points = new Vector3[segments + 1];
        Quaternion rotation = Quaternion.LookRotation(normal); // 确保圆在墙的平面上
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            Vector3 localPoint = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            points[i] = center + rotation * localPoint; // 将点转换到墙面坐标系
        }

        // 绘制圆线
        for (int i = 0; i < segments; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }
}