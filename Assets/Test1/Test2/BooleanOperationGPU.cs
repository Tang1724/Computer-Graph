using UnityEngine;

public class BooleanOperationGPU : MonoBehaviour
{
    public GameObject objectA; // 输入物体 A
    public GameObject objectB; // 输入物体 B
    public GameObject resultObject; // 用于显示布尔运算结果的物体

    public ComputeShader booleanComputeShader; // Compute Shader

    public int operationType = 0; // 布尔运算类型 (0 = Union, 1 = Intersection, 2 = Subtraction)

    private ComputeBuffer verticesABuffer;
    private ComputeBuffer verticesBBuffer;
    private ComputeBuffer resultBuffer;

    void Start()
    {
        PerformBooleanOperation();
    }

    void PerformBooleanOperation()
    {
        // 检查对象是否设置
        if (objectA == null || objectB == null || resultObject == null || booleanComputeShader == null)
        {
            Debug.LogError("请确保 ObjectA、ObjectB、ResultObject 和 ComputeShader 都已设置！");
            return;
        }

        // 获取输入的 Mesh
        Mesh meshA = objectA.GetComponent<MeshFilter>().mesh;
        Mesh meshB = objectB.GetComponent<MeshFilter>().mesh;

        if (meshA == null || meshB == null)
        {
            Debug.LogError("ObjectA 或 ObjectB 没有 MeshFilter 或未赋值 Mesh！");
            return;
        }

        // 获取顶点数据
        Vector3[] verticesA = meshA.vertices;
        Vector3[] verticesB = meshB.vertices;

        Debug.Log("Mesh A Vertices Count: " + verticesA.Length);
        Debug.Log("Mesh B Vertices Count: " + verticesB.Length);

        // 初始化 Compute Buffers
        verticesABuffer = new ComputeBuffer(verticesA.Length, sizeof(float) * 3);
        verticesBBuffer = new ComputeBuffer(verticesB.Length, sizeof(float) * 3);
        resultBuffer = new ComputeBuffer(verticesA.Length, sizeof(float) * 3); // 假设结果顶点数量与 A 相同

        verticesABuffer.SetData(verticesA);
        verticesBBuffer.SetData(verticesB);

        // 设置 Compute Shader 参数
        int kernel = booleanComputeShader.FindKernel("BooleanOperation");
        booleanComputeShader.SetBuffer(kernel, "verticesA", verticesABuffer);
        booleanComputeShader.SetBuffer(kernel, "verticesB", verticesBBuffer);
        booleanComputeShader.SetBuffer(kernel, "resultVertices", resultBuffer);
        booleanComputeShader.SetInt("operationType", operationType);

        // 执行 Compute Shader
        int threadGroups = Mathf.CeilToInt((float)verticesA.Length / 64);
        booleanComputeShader.Dispatch(kernel, threadGroups, 1, 1);

        // 获取结果数据
        Vector3[] resultVertices = new Vector3[verticesA.Length];
        resultBuffer.GetData(resultVertices);

        Debug.Log("Result Vertices Count: " + resultVertices.Length);
        Debug.Log("First Result Vertex: " + resultVertices[0]);

        // 创建结果 Mesh
        Mesh resultMesh = new Mesh();
        resultMesh.vertices = resultVertices;
        resultMesh.triangles = meshA.triangles; // 使用 A 的三角形结构作为示例
        resultMesh.RecalculateNormals();
        resultMesh.RecalculateBounds();

        // 应用到结果物体
        resultObject.GetComponent<MeshFilter>().mesh = resultMesh;

        // 清理 Compute Buffers
        verticesABuffer.Release();
        verticesBBuffer.Release();
        resultBuffer.Release();
    }
}