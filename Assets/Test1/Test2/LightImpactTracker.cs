using UnityEngine;

public class LightImpactTracker : MonoBehaviour
{
    public Vector3 lightImpactPosition; // 光圈中心位置（墙面上的实际照射位置）
    public float lightImpactRadius; // 光圈在墙面上的半径
    public Vector3 lightImpactNormal; // 墙面的法线
    public bool isHitByLight = false; // 是否被光源照射
    public float distanceToCharacter; // 碰撞点到人物的距离

    public GameObject character; // 用于指定人物的 GameObject

    private void Update()
    {
        // 初始化状态
        isHitByLight = false;
        lightImpactRadius = 0;
        distanceToCharacter = 0;

        // 获取所有场景中的光源
        Light[] lights = FindObjectsOfType<Light>();
        foreach (Light light in lights)
        {
            if (light.type == LightType.Spot || light.type == LightType.Point)
            {
                // 计算光源是否照射到墙面
                if (CalculateLightImpact(light, out Vector3 hitPoint, out Vector3 normal, out float radius))
                {
                    lightImpactPosition = hitPoint;
                    lightImpactNormal = normal;
                    lightImpactRadius = radius;
                    isHitByLight = true;

                    // 计算碰撞点到人物的距离
                    if (character != null)
                    {
                        distanceToCharacter = Vector3.Distance(hitPoint, character.transform.position);
                    }
                }
            }
        }
    }

    // 计算光源对墙面的影响
    private bool CalculateLightImpact(Light light, out Vector3 hitPoint, out Vector3 normal, out float radius)
    {
        hitPoint = Vector3.zero;
        normal = Vector3.zero;
        radius = 0;

        // 从光源方向向墙面发射射线
        RaycastHit hit;
        Vector3 lightDirection = light.transform.forward; // 光源的方向
        if (Physics.Raycast(light.transform.position, lightDirection, out hit))
        {
            // 检查是否击中任何对象
            hitPoint = hit.point; // 获取碰撞点（光圈中心位置）
            normal = hit.normal; // 获取碰撞点的法线

            // 计算光圈半径（投影范围）
            if (light.type == LightType.Spot)
            {
                float distanceToWall = Vector3.Distance(light.transform.position, hit.point);
                radius = Mathf.Tan(light.spotAngle * 0.5f * Mathf.Deg2Rad) * distanceToWall; // 投影半径
            }
            else if (light.type == LightType.Point)
            {
                radius = 1f; // 点光源的光圈范围假设为一个固定值（可根据需求调整）
            }

            return true;
        }

        return false;
    }
}