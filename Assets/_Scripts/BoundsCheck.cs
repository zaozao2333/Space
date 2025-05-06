using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /// <summary>
    /// 保持游戏对象在屏幕
    /// 只对于位于[0，0，0]的主正交摄像机有效
    /// </summary>
public class BoundsCheck : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Set in Inspector")]
    public float radius = 1f;

    [Header("Set Dynamically")]
    public float camWidth;
    public float camHeight;
    void Awake()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = transform.position;
        if(pos.x > camWidth - radius)
        {
            pos.x = camWidth - radius;
        }
        if (pos.x < -camWidth + radius)
        {
            pos.x = -camWidth + radius;
        }
        if (pos.y > camHeight - radius)
        {
            pos.y = camHeight - radius;
        }
        if (pos.y < -camHeight + radius)
        {
            pos.y = -camHeight + radius;
        }
        transform.position = pos;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Vector3 boundSize = new Vector3(camWidth * 2, camHeight * 2, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
}
