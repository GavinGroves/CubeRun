using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Transform m_Transform;
    private Transform player_Transform;
    public bool startFollow = false;
    private Vector3 normalPos; //存放摄像机初始位置
    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        normalPos = m_Transform.position;
        player_Transform = GameObject.Find("cube_books").GetComponent<Transform>();
    }

    void Update()
    {
        CameraFollow();
    }

    private void CameraFollow()
    {
        if (startFollow)
        {
            Vector3 pos =
            new Vector3(m_Transform.position.x, player_Transform.position.y + 1.08f, player_Transform.position.z);

            m_Transform.position = Vector3.Lerp(m_Transform.position, pos, Time.deltaTime);
        }
    }
    
    /// <summary>
    /// 重置摄像机位置
    /// </summary>
    public void ResetCamera()
    {
        m_Transform.position = normalPos;
    }
}