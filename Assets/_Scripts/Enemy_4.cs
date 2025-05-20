using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part
{
    public string name;
    public float health;
    public string[] protectedBy;

    [HideInInspector]
    public GameObject go;
    public Material mat;
}
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts;

    public Vector3 p0, p1;
    public float timeStart;
    public float duration = 4;
    // Start is called before the first frame update
    void Start()
    {

        Transform t;
        foreach(Part part in parts)
        {
            t = transform.Find(part.name);
            if(t != null)
            {
                part.go = t.gameObject;
                part.mat = part.go.GetComponent<Renderer>().material;
                part.go.tag = "EnemyPart";
                part.go.layer = LayerMask.NameToLayer("EnemyPart");
            }
        }
    }

    void InitMovement()
    {
        p0 = p1;
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);
        timeStart = Time.time;  
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;
        if( u >= 1)
        {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2);
        pos = (1 - u) * p0 + u * p1;
    }

    public Part FindPart(string name)
    {
        foreach (Part part in parts)
        {
            if (part.name == name)
                return part;
        }
        return null;
    }

    public Part FindPart(GameObject go)
    {
        foreach (Part part in parts)
        {
            if (part.go == go) return part;
        }
        return null;
    }

    public bool Destoryed(Part part)
    {
        if (part == null) return true;
        return (part.health <= 0); 
    }

    public bool Destoryed(string name)
    {
        return Destoryed(FindPart(name));
    }

    public bool Destoryed(GameObject go)
    {
        return Destoryed(FindPart(go));
    }

    public void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                if (!bndCheck.isOnScreen)
                {
                    Destoryed(other);
                    break;
                }
                GameObject goHit = collision.contacts[0].thisCollider.gameObject;
                Part partHit = FindPart(goHit);
                if (partHit == null)
                {
                    goHit = collision.contacts[0].otherCollider.gameObject;
                    partHit = FindPart(goHit);
                }

                if (partHit.protectedBy != null)
                {
                    foreach (string s in partHit.protectedBy)
                    {
                        if (!Destoryed(s))
                        {
                            Destoryed(other);
                            return;
                        }
                    }
                }
                partHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                ShowLocalizedDamage(partHit.mat);
                if(partHit.health <= 0)
                {
                    partHit.go.SetActive(false);
                    bool allDestoryed = true;
                    foreach (Part part in parts)
                    {
                        if(!Destoryed(part))
                            allDestoryed = false;
                        break;
                    }
                    if (allDestoryed)
                    {
                        Main.S.ShipDestoryed(this);
                        Destoryed(this.gameObject);
                    }
                }
                Destoryed(other);
                break;
        }
    }
}
