using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_5 : Enemy_4
{
    // Start is called before the first frame update
    void Start()
    {
        Transform t;
        foreach (Part part in parts)
        {
            t = transform.Find(part.name);
            if (t != null)
            {
                part.go = t.gameObject;
                part.mat = part.go.GetComponent<Renderer>().material;
                part.go.tag = "EnemyPart";
                part.go.layer = LayerMask.NameToLayer("EnemyPart");
            }
        }
    }

    public override void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.tag == "ProjectileHero")
        {
            Destroy(other.gameObject); // 显式销毁子弹
            base.OnCollisionEnter(collision); // 调用父类逻辑
        }
    }
}
