using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // 导入处理场景的命名空间

public class GameManage : MonoBehaviour
{
    void Update()
    {
        // 检测玩家是否按下了 'R' 键
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCurrentScene();
        }
    }

    // 重置当前场景的方法
    void ResetCurrentScene()
    {
        // 获取当前活跃场景的名称
        string sceneName = SceneManager.GetActiveScene().name;
        // 重新加载当前场景
        SceneManager.LoadScene(sceneName);
    }
}
