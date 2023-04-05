using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Transform m_Transform;
    private MapManager mapManager;
    private CameraMove cameraMove;

    private UIManager m_UIManager;

    private Color colorOne = new Color(134 / 255f, 113 / 255f, 173 / 255f);
    private Color colorTwo = new Color(146 / 255f, 120 / 255f, 180 / 255f);

    public int z = 3;
    private int x = 2;

    private bool life = true;
    private int gemCount = 0;
    private int scoreCount = 0;


    void Start()
    {
        gemCount = PlayerPrefs.GetInt("gem", 0);
        m_Transform = gameObject.GetComponent<Transform>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        cameraMove = GameObject.Find("Main Camera").GetComponent<CameraMove>();
        m_UIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    private void AddGemCount()
    {
        gemCount++;
        m_UIManager.UpdateData(scoreCount, gemCount);
        Debug.Log("宝石数：" + gemCount);
    }

    private void AddScoreCount()
    {
        scoreCount++;
        m_UIManager.UpdateData(scoreCount, gemCount);
        Debug.Log("分数：" + scoreCount);
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("gem", gemCount);
        if (scoreCount > PlayerPrefs.GetInt("score", 0))
        {
            PlayerPrefs.SetInt("score", scoreCount);
        }
    }

    /// <summary>
    /// 点击按钮/M键 → 开始游戏
    /// </summary>
    public void StartGame()
    {
        PlayerInit();
        cameraMove.startFollow = true;
        mapManager.StartTileDown();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartGame();
        }

        if (life)
        {
            PlayerControll();
        }

        CalcPosition();
    }

    /// <summary>
    /// 设置角色位置，蜗牛痕迹生成
    /// </summary>
    private void PlayerInit()
    {
        Transform playerPosition = mapManager.tileLists[z][x].GetComponent<Transform>();
        MeshRenderer normalColor = null;

        m_Transform.position = playerPosition.position + new Vector3(0, 0.245f / 2, 0);
        m_Transform.rotation = playerPosition.rotation;

        if (playerPosition.tag == "Tile")
        {
            normalColor = playerPosition.Find("normal_a2").GetComponent<MeshRenderer>();
        }
        else if (playerPosition.tag == "Spikes")
        {
            normalColor = playerPosition.Find("moving_spikes_a2").GetComponent<MeshRenderer>();
        }
        else if (playerPosition.tag == "Sky_Spikes")
        {
            normalColor = playerPosition.Find("smashing_spikes_a2").GetComponent<MeshRenderer>();
        }

        if (normalColor != null)
        {
            if (z % 2 == 0)
            {
                normalColor.material.color = colorOne;
            }
            else
            {
                normalColor.material.color = colorTwo;
            }
        }
        else
        {
            gameObject.AddComponent<Rigidbody>();
            StartCoroutine("GameOver", true);
        }
    }

    /// <summary>
    /// 移动
    /// </summary>
    private void PlayerControll()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayerLeft();
        }

        //右
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayerRight();
        }
    }

    public void PlayerLeft()
    {
        //左
        if (x != 0)
        {
            z++;
            AddScoreCount();
        }

        if (z % 2 == 1 && x != 0)
        {
            x--;
        }

        PlayerInit();
    }

    public void PlayerRight()
    {
        if (x != 4 || z % 2 != 1)
        {
            z++;
            AddScoreCount();
        }

        if (z % 2 == 0 && x != 4)
        {
            x++;
        }

        PlayerInit();
    }

    private void CalcPosition()
    {
        if (mapManager.tileLists.Count - z < 13)
        {
            mapManager.AddPR();
            float offsetZ =
                mapManager.tileLists[mapManager.tileLists.Count - 1][0].GetComponent<Transform>().position.z +
                mapManager.bottomLength / 2;
            mapManager.CreateTileWhite(offsetZ);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Spikes_Attack")
        {
            StartCoroutine("GameOver", false);
        }

        if (other.tag == "Gem")
        {
            GameObject.Destroy(other.gameObject.GetComponent<Transform>().parent.gameObject);
            AddGemCount();
        }
    }

    public IEnumerator GameOver(bool b)
    {
        if (b)
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (life)
        {
            Debug.Log("游戏结束");
            cameraMove.startFollow = false;
            life = false;
            SaveData();
            //UI相关交互.
            StartCoroutine("ResetGame");
        }
        // 时间暂停
        // Time.timeScale = 0;
    }

    /// <summary>
    /// 角色死亡后重置
    /// </summary>
    private void ResetPlayer()
    {
        //重置-取消角色刚体组件
        GameObject.Destroy(gameObject.GetComponent<Rigidbody>());
        //重置-角色出生点位置
        z = 3;
        x = 2;
        //重置life值（重生）
        life = true;
        //重置当前分数
        scoreCount = 0;
    }

    /// <summary>
    /// 开启新局重置
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(2);
        ResetPlayer();
        mapManager.ResetGameMap();
        cameraMove.ResetCamera();
        m_UIManager.ResetUI();
    }
}