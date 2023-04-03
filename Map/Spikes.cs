using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private Transform m_Transform;
    private Transform son_Transform;

    private Vector3 normalPos;//默认位置
    private Vector3 targetPos;//目标位置

    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        son_Transform = m_Transform.Find("moving_spikes_b").GetComponent<Transform>();

        normalPos = son_Transform.position;
        targetPos = son_Transform.position + new Vector3(0,0.15f,0);
        StartCoroutine("UpAndDown");
    }

    private IEnumerator UpAndDown()
    {
        while(true)
        {
            StopCoroutine("Down");
            StartCoroutine("Up");
            yield return new WaitForSeconds(2.0f);
            StopCoroutine("Up");
            StartCoroutine("Down");
            yield return new WaitForSeconds(2.0f);
        }
    }

    private IEnumerator Up()
    {
        while(true)
        {
            son_Transform.position = Vector3.Lerp(son_Transform.position,targetPos,Time.deltaTime * 25);
            yield return null;
        }    
    }

    private IEnumerator Down()
    {
        while(true)
        {
            son_Transform.position = Vector3.Lerp(son_Transform.position,normalPos,Time.deltaTime * 25);
            yield return null;
        }    
    }
}
