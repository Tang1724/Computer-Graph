using UnityEngine;
using LibCSG;
using System.Collections.Generic;

public class BooleanOperationExample : MonoBehaviour
{
    public GameObject objectA; // 用户从 Inspector 选择的第一个物体
    public List<GameObject> objectsB; // 用户从 Inspector 选择的一系列物体 B
    public GameObject resultObject; // 用于显示布尔运算结果的物体
    public Operation operationType = Operation.OPERATION_SUBTRACTION; // 用户选择布尔运算类型 (默认差集)

    private CSGBrushOperation csgOperation; // 用于执行布尔运算
    private CSGBrush resultBrush;           // 存储布尔运算结果

    private CSGBrush brushA; // Brush 对象 A
    private List<CSGBrush> brushesB; // Brush 对象 B 的集合

    void Start()
    {
        // 初始化布尔操作工具
        csgOperation = new CSGBrushOperation();

        // 检查结果对象是否存在
        if (resultObject == null)
        {
            Debug.LogError("结果物体 (Result Object) 未设置，请在 Inspector 中指定！");
            return;
        }

        // 创建用于存储结果的 CSGBrush
        resultBrush = new CSGBrush(resultObject);

        // 初始化 B 的 Brush 列表
        brushesB = new List<CSGBrush>();
    }

    void Update()
    {
        // 检查输入物体是否设置
        if (objectA == null || objectsB == null || objectsB.Count == 0)
        {
            Debug.LogWarning("请在 Inspector 中指定物体 A 和至少一个物体 B！");
            return;
        }

        // 如果输入物体发生变化或需要重新计算
        PerformBooleanOperation();
    }

    void PerformBooleanOperation()
    {
        // 初始化 Brush A
        if (brushA == null)
        {
            brushA = new CSGBrush(objectA);
            brushA.build_from_mesh(objectA.GetComponent<MeshFilter>().mesh);
        }

        // 初始化 Brushes B
        brushesB.Clear();
        foreach (GameObject objB in objectsB)
        {
            if (objB != null)
            {
                CSGBrush brushB = new CSGBrush(objB);
                brushB.build_from_mesh(objB.GetComponent<MeshFilter>().mesh);
                brushesB.Add(brushB);
            }
        }

        // 开始执行布尔运算：A 与所有 B
        CSGBrush currentResult = brushA;
        foreach (var brushB in brushesB)
        {
            // 创建一个临时结果 Brush
            CSGBrush tempResult = new CSGBrush(resultObject);
            csgOperation.merge_brushes(operationType, currentResult, brushB, ref tempResult);
            currentResult = tempResult; // 更新当前结果
        }

        // 将最终结果赋值给 resultBrush
        resultBrush = currentResult;

        // 更新结果物体的 Mesh
        Mesh resultMesh = resultObject.GetComponent<MeshFilter>().mesh;
        resultMesh.Clear();
        resultBrush.getMesh(resultMesh);

        // 确保法线正确
        resultMesh.RecalculateNormals();
        resultMesh.RecalculateBounds();

        // 添加或更新 MeshCollider
        UpdateMeshCollider(resultMesh);
    }

    void UpdateMeshCollider(Mesh resultMesh)
    {
        // 检查是否已经有 MeshCollider
        MeshCollider meshCollider = resultObject.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            // 如果没有，添加一个
            meshCollider = resultObject.AddComponent<MeshCollider>();
        }

        // 更新 MeshCollider 的 Mesh
        meshCollider.sharedMesh = resultMesh;

        // 如果需要凸形碰撞体，可以启用以下代码（需根据需求修改）
    }
}