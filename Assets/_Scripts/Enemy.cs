using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;
    public float showDamageDuration = 0.1f;
    public float powerUpDropChance = 1f;

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials;
    public bool showingDamage = false;
    public float damageDoneTime;
    public bool notifiedOfDestruction = false;

    protected BoundsCheck bndCheck;
    public Vector3 pos
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }

    void Update()
    {
        Move();
        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }
        if(bndCheck != null && bndCheck.offDown)
        {
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();

        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject otherGO = collision.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                if (!bndCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                ShowDamage();
                if(health <= 0)
                {
                    if (!notifiedOfDestruction)
                    {
                        Main.S.ShipDestoryed(this);
                    }
                    Destroy(this.gameObject);
                }
                Destroy(otherGO);
                break;
            default:
                break;
        }
    }

    public void ShowDamage()
    {
        foreach (Material mat in materials)
        {
            mat.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration; 
    }

    void UnShowDamage()
    {
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        ShowDamage();
        if (health <= 0)
        {
            if (!notifiedOfDestruction)
            {
                Main.S.ShipDestoryed(this);
            }
            Destroy(this.gameObject);
        }
    }
}
