using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private Transform m_Transform;
    private GameObject tile_white_Prefabs;
    private GameObject wall2_Prefabs;
    private GameObject moving_spikes_Prefabs;
    private GameObject smashing_spikes_Prefabs;
    private GameObject gem2_Prefabs;

    private PlayerController m_playerController;

    // 地图数据存储
    public List<GameObject[]> tileLists;

    private Color colorOne = new Color(124 / 255f, 155 / 255f, 230 / 255f);
    private Color colorTwo = new Color(125 / 255f, 169 / 255f, 233 / 255f);
    private Color colorWall = new Color(87 / 255f, 93 / 255f, 169 / 255f);

    //概率
    private int pr_hole = 0; //坑洞概率.
    private int pr_Spikes = 0; //地刺概率.
    private int pr_smashing = 0; //天上陷阱概率.
    private int pr_gem = 2;

    public float bottomLength = Mathf.Sqrt(2) * 0.254f;
    private int index = 0;

    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();

        tile_white_Prefabs = Resources.Load<GameObject>("tile_white");
        wall2_Prefabs = Resources.Load<GameObject>("wall2");
        moving_spikes_Prefabs = Resources.Load<GameObject>("moving_spikes");
        smashing_spikes_Prefabs = Resources.Load<GameObject>("smashing_spikes");
        gem2_Prefabs = Resources.Load<GameObject>("gem 2");

        m_playerController = GameObject.Find("cube_books").GetComponent<PlayerController>();

        tileLists = new List<GameObject[]>();
        CreateTileWhite(0);
    }

    /// <summary>
    /// 生成地砖
    /// </summary>
    public void CreateTileWhite(float offsetZ)
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject[] tileArray = new GameObject[6];
            for (int j = 0; j < 6; j++)
            {
                Vector3 pos = new Vector3(j * bottomLength, 0, i * bottomLength + offsetZ);
                Vector3 rot = new Vector3(-90, 45, 0);
                GameObject go = null;
                if (j == 0 || j == 5)
                {
                    go = GameObject.Instantiate<GameObject>(wall2_Prefabs, pos, Quaternion.Euler(rot), m_Transform);
                    go.GetComponent<MeshRenderer>().material.color = colorWall;
                }
                else
                {
                    int pr = CalcPR();
                    if (pr == 0) //正常地板
                    {
                        go = GameObject.Instantiate<GameObject>(tile_white_Prefabs, pos, Quaternion.Euler(rot),
                            m_Transform);
                        go.GetComponent<Transform>().Find("normal_a2").GetComponent<MeshRenderer>().material.color =
                            colorOne;
                        go.GetComponent<MeshRenderer>().material.color = colorOne;
                        int gemPr = CalcGem();
                        if (gemPr == 1) //生成宝石
                        {
                            GameObject gem = GameObject.Instantiate<GameObject>(gem2_Prefabs,
                                go.GetComponent<Transform>().position + new Vector3(0, 0.06f, 0), Quaternion.identity,
                                go.GetComponent<Transform>());
                        }
                    }
                    else if (pr == 1) //坑洞（空物体）
                    {
                        go = new GameObject();
                        go.GetComponent<Transform>().position = pos;
                        go.GetComponent<Transform>().rotation = Quaternion.Euler(rot);
                    }
                    else if (pr == 2) //地面陷阱
                    {
                        go = GameObject.Instantiate<GameObject>(moving_spikes_Prefabs, pos, Quaternion.Euler(rot),
                            m_Transform);
                    }
                    else if (pr == 3) //天空陷进
                    {
                        go = GameObject.Instantiate<GameObject>(smashing_spikes_Prefabs, pos, Quaternion.Euler(rot),
                            m_Transform);
                    }
                }

                tileArray[j] = go;
            }

            tileLists.Add(tileArray);
            GameObject[] whiteArray = new GameObject[5];
            for (int j = 0; j < 5; j++)
            {
                Vector3 pos = new Vector3(j * bottomLength + bottomLength / 2, 0,
                    i * bottomLength + bottomLength / 2 + offsetZ);
                Vector3 rot = new Vector3(-90, 45, 0);
                GameObject go = null;
                int pr = CalcPR();
                if (pr == 0)
                {
                    go = GameObject.Instantiate<GameObject>(tile_white_Prefabs, pos, Quaternion.Euler(rot),
                        m_Transform);
                    go.GetComponent<Transform>().Find("normal_a2").GetComponent<MeshRenderer>().material.color =
                        colorTwo;
                    go.GetComponent<MeshRenderer>().material.color = colorTwo;
                }
                else if (pr == 1) //坑洞（空物体）
                {
                    go = new GameObject();
                    go.GetComponent<Transform>().position = pos;
                    go.GetComponent<Transform>().rotation = Quaternion.Euler(rot);
                }
                else if (pr == 2) //地面陷阱
                {
                    go = GameObject.Instantiate<GameObject>(moving_spikes_Prefabs, pos, Quaternion.Euler(rot),
                        m_Transform);
                }
                else if (pr == 3)
                {
                    go = GameObject.Instantiate<GameObject>(smashing_spikes_Prefabs, pos, Quaternion.Euler(rot),
                        m_Transform);
                }

                // GameObject.Instantiate<GameObject>(tile_white_Prefabs, pos, Quaternion.Euler(rot), m_Transform);
                // go.GetComponent<MeshRenderer>().material.color = colorTwo;
                // go.GetComponent<Transform>().Find("normal_a2").GetComponent<MeshRenderer>().material.color = colorTwo;
                whiteArray[j] = go;
            }

            tileLists.Add(whiteArray);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string str = "";
            for (int i = 0; i < tileLists.Count; i++)
            {
                for (int j = 0; j < tileLists[i].Length; j++)
                {
                    str += tileLists[i][j].name;
                    tileLists[i][j].name = i + "---" + j;
                }

                str += "\n";
            }

            Debug.Log(str);
        }
    }

    public void StartTileDown()
    {
        StartCoroutine("TileDown");
    }

    private void StopTileDown()
    {
        StopCoroutine("TileDown");
    }

    /// <summary>
    /// 地面塌陷
    /// </summary>
    private IEnumerator TileDown()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < tileLists[index].Length; i++)
            {
                Rigidbody rb = tileLists[index][i].AddComponent<Rigidbody>();
                rb.angularVelocity =
                    new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)) *
                    Random.Range(1, 10);
                GameObject.Destroy(tileLists[index][i], 1.0f);
            }

            if (m_playerController.z == index)
            {
                StopTileDown();
                m_playerController.gameObject.AddComponent<Rigidbody>();
                m_playerController.StartCoroutine("GameOver", true);
            }

            index++;
        }
    }

    /// <summary>
    /// 计算陷进概率.
    /// 0:瓷砖
    /// 1:坑洞
    /// 2:地面陷阱
    /// 3.:天空陷阱
    /// </summary> 
    private int CalcPR()
    {
        int pr = Random.Range(0, 100);
        if (pr <= pr_hole)
        {
            return 1;
        }
        else if (31 < pr && pr < pr_Spikes + 30)
        {
            return 2;
        }
        else if (61 < pr && pr < pr_smashing + 60)
        {
            return 3;
        }

        return 0;
    }

    /// <summary>
    /// 计算宝石概率.
    /// </summary>
    /// <returns> 0:不生成 ， 1：生成 </returns>
    private int CalcGem()
    {
        int pr = Random.Range(1, 100);
        if (pr <= pr_gem)
        {
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// 增加概率
    /// </summary>
    public void AddPR()
    {
        pr_hole += 2;
        pr_Spikes += 1;
        pr_smashing += 1;
    }

    /// <summary>
    /// 新局重置游戏地图场景
    /// </summary>
    public void ResetGameMap()
    {
        //新局开始，清空上一局场景
        Transform[] sonTransform = m_Transform.GetComponentsInChildren<Transform>();
        for (int i = 1; i < sonTransform.Length; i++)
        {
            GameObject.Destroy(sonTransform[i].gameObject);
        }

        //概率清零
        pr_hole = 0;
        pr_Spikes = 0;
        pr_smashing = 0;
        pr_gem = 2;
        
        //重置塌陷角标 (表z轴 [z][x]，也就是[index][i] 
        index = 0;
        
        //清空list集合
        tileLists.Clear();
        
        //重新生成新地图
        CreateTileWhite(0);
    }
}