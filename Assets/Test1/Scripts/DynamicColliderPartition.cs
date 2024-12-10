using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicColliderPartition : MonoBehaviour
{
    public Vector3 subColliderSize = new Vector3(1, 1, 1); // 每个小Collider的尺寸

    void Start()
    {
        SplitCollider();
    }

    void SplitCollider()
    {
        Collider originalCollider = GetComponent<Collider>();

        if (originalCollider == null)
        {
            Debug.LogError("No collider attached to the object!");
            return;
        }

        Bounds bounds = originalCollider.bounds;
        Vector3 originalSize = bounds.size;

        // 计算每个方向需要多少个collider
        int countX = Mathf.CeilToInt(originalSize.x / subColliderSize.x);
        int countY = Mathf.CeilToInt(originalSize.y / subColliderSize.y);
        int countZ = Mathf.CeilToInt(originalSize.z / subColliderSize.z);

        // 计算新的collider的实际尺寸（如果不能整除）
        Vector3 newSize = new Vector3(originalSize.x / countX, originalSize.y / countY, originalSize.z / countZ);

        // 移除原始的Collider
        Destroy(originalCollider);

        // 在物体上添加新的colliders
        for (int x = 0; x < countX; x++)
        {
            for (int y = 0; y < countY; y++)
            {
                for (int z = 0; z < countZ; z++)
                {
                    Vector3 center = new Vector3(
                        (x + 0.5f) * newSize.x - originalSize.x / 2,
                        (y + 0.5f) * newSize.y - originalSize.y / 2,
                        (z + 0.5f) * newSize.z - originalSize.z / 2
                    );

                    BoxCollider box = gameObject.AddComponent<BoxCollider>();
                    box.size = newSize;
                    box.center = center;
                }
            }
        }
    }
}