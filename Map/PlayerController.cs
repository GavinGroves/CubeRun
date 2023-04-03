using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Transform m_Transform;
    private MapManager mapManager;
    private CameraMove cameraMove;

    private Color colorOne = new Color(134 / 255f, 113 / 255f, 173 / 255f);
    private Color colorTwo = new Color(146 / 255f, 120 / 255f, 180 / 255f);

    public int z = 3;
    private int x = 2;

    private bool life = true;

    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        cameraMove = GameObject.Find("Main Camera").GetComponent<CameraMove>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayerInit();
            cameraMove.startFollow = true;
            mapManager.StartTileDown();
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

    private void PlayerControll()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (x != 0)
            {
                z++;
            }
            if (z % 2 == 1 && x != 0)
            {
                x--;
            }
            PlayerInit();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (x != 4 || z % 2 != 1)
            {
                z++;
            }
            if (z % 2 == 0 && x != 4)
            {
                x++;
            }
            PlayerInit();
        }
    }
    private void CalcPosition()
    {
        if (mapManager.tileLists.Count - z < 13)
        {
            mapManager.AddPR();
            float offsetZ = mapManager.tileLists[mapManager.tileLists.Count - 1][0].
                                                    GetComponent<Transform>().position.z + mapManager.bottomLength / 2;
            mapManager.CreateTileWhite(offsetZ);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Spikes_Attack")
        {
            StartCoroutine("GameOver", false);
        }
    }

    public IEnumerator GameOver(bool b)
    {
        if (b)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("游戏结束");
        life = false;
        // 时间暂停
        Time.timeScale = 0;
    }
}