using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    public Light lightSource; // 光源
    public float threshold = 2.0f; // 光照强度阈值

    private MeshRenderer meshRenderer; // Mesh Renderer组件

    void Start()
    {
        // 获取当前GameObject的Mesh Renderer组件
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer == null)
        {
            Debug.LogError("MeshRenderer component not found on the object!");
        }
        else
        {
            // 根据初始光照强度设置Mesh Renderer的启用状态
            UpdateRendererState();
        }
    }

    void Update()
    {
        // 如果光源存在，更新Mesh Renderer的启用状态
        if (lightSource != null && meshRenderer != null)
        {
            UpdateRendererState();
        }
    }

    // 更新Mesh Renderer的启用状态，根据光照强度
    void UpdateRendererState()
    {
        meshRenderer.enabled = lightSource.intensity > threshold;
    }
}