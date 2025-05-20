using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    none,
    blaster,
    spread,
    phaser,
    missile,
    laser,
    shield
}

[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter;
    public Color color = Color.white;
    public GameObject projectilePrefab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;
    public float continuousDamage = 0;
    public float delayBetweenShots = 0;
    public float velocity = 20;
}
public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.blaster;
    public WeaponDefinition definition;
    public GameObject collar;
    public float lastShot;
    private Renderer collarRend;
    // Start is called before the first frame update
    void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();  

        SetType(_type);
        if(PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_Projectile_Anchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    public WeaponType type
    {
        get { return _type; }
        set { SetType(value); }
    }
    public void SetType(WeaponType type)
    {
        _type = type;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        definition = Main.GetWeaponDefinition(_type);
        collarRend.material.color = definition.color;
        lastShot = 0;
    }

    public void Fire()
    {
        if (!gameObject.activeInHierarchy) return;
        if (Time.time - lastShot < definition.delayBetweenShots) return;
        Projectile p;
        Laser l;
        Vector3 vel = Vector3.up * definition.velocity;
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }

        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = vel;
                break;
            case WeaponType.spread:
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = vel;

                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.GetComponent<Rigidbody>().velocity = p.transform.rotation * vel;

                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.GetComponent<Rigidbody>().velocity = p.transform.rotation * vel;
                break;
            case WeaponType.laser:
                l = MakeLaser();
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(definition.projectilePrefab);
        if(transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position =collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShot = Time.time;
        return p;
    }

    public Laser MakeLaser()
    {
        GameObject go = new GameObject("Laser");
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);

        // 添加LineRenderer组件
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = 0.5f;
        lr.endWidth = 0.5f;
        lr.startColor = definition.projectileColor;
        lr.endColor = definition.projectileColor;

        // 添加Laser组件
        Laser laser = go.AddComponent<Laser>();
        laser.type = type;
        laser.damagePerSecond = definition.continuousDamage;
        laser.maxDistance = 100f; // 可根据需要调整
        laser.hitLayers = LayerMask.GetMask("Enemy", "EnemyPart");

        lastShot = Time.time;
        return laser;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
