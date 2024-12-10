using UnityEngine;
using UnityEngine.ProBuilder;
using LibCSG;

public class BooleanOperation : MonoBehaviour
{
    public GameObject objectA; // 需要布尔操作的第一个物体（例如 Cube）
    public GameObject objectB; // 需要布尔操作的第二个物体（例如 Sphere）
    public Operation operation = Operation.OPERATION_SUBTRACTION; // 布尔操作类型

    void Start()
    {
        // 检查两个物体是否存在
        if (objectA == null || objectB == null)
        {
            Debug.LogError("请将两个 GameObject （objectA 和 objectB）拖入脚本的 Inspector 面板！");
            return;
        }

        // 获取两个物体的 Mesh 数据
        Mesh meshA = GetMeshFromGameObject(objectA);
        Mesh meshB = GetMeshFromGameObject(objectB);

        if (meshA == null || meshB == null)
        {
            Debug.LogError("确保 objectA 和 objectB 上都存在 MeshFilter 组件及有效的 Mesh 数据！");
            return;
        }

        // 创建两个 CSGBrush 对象
        CSGBrush brushA = new CSGBrush(objectA);
        brushA.build_from_mesh(meshA);

        CSGBrush brushB = new CSGBrush(objectB);
        brushB.build_from_mesh(meshB);

        // 创建用于存储结果的 CSGBrush
        CSGBrush resultBrush = new CSGBrush();

        // 创建 CSGBrushOperation 并执行布尔操作
        CSGBrushOperation brushOperation = new CSGBrushOperation();
        brushOperation.merge_brushes(operation, brushA, brushB, ref resultBrush);

        // 将结果转换为 Mesh
        Mesh resultMesh = resultBrush.getMesh();

        // 将结果应用到新的 GameObject 上
        GameObject resultObject = CreateResultObject(resultMesh);

        // 设置结果物体的位置和旋转
        resultObject.transform.position = objectA.transform.position;
        resultObject.transform.rotation = objectA.transform.rotation;

        Debug.Log("布尔运算完成！结果物体已经生成。");
    }

    /// <summary>
    /// 从 GameObject 中获取 Mesh
    /// </summary>
    /// <param name="go">GameObject</param>
    /// <returns>Mesh</returns>
    private Mesh GetMeshFromGameObject(GameObject go)
    {
        MeshFilter meshFilter = go.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            return meshFilter.sharedMesh;
        }
        else
        {
            Debug.LogError($"GameObject {go.name} 没有 MeshFilter 组件！");
            return null;
        }
    }

    /// <summary>
    /// 创建用于显示结果的 GameObject
    /// </summary>
    /// <param name="mesh">结果 Mesh</param>
    /// <returns>新的 GameObject</returns>
    private GameObject CreateResultObject(Mesh mesh)
    {
        GameObject resultObject = new GameObject("ResultObject");
        MeshFilter meshFilter = resultObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = resultObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = mesh;
        meshRenderer.material = new Material(Shader.Find("Standard")); // 使用默认材质

        return resultObject;
    }
}