using UnityEngine;

public class Laser : MonoBehaviour
{
    public WeaponType type;
    public float damagePerSecond;
    public float maxDistance;
    public LayerMask hitLayers;
    public float currentLifetime;
    public float lifetime = 0.02f;

    private LineRenderer lineRenderer;
    private static float damageTimer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        currentLifetime += Time.deltaTime;
        if (currentLifetime >= lifetime)
        {
            Destroy(gameObject); // 销毁激光对象
            return;
        }
        // 激光起点（武器位置）
        Vector3 startPos = transform.position;
        // 激光终点（向上发射）
        Vector3 endPos = startPos + Vector3.up * maxDistance;

        // 射线检测
        RaycastHit hit;
        if (Physics.Raycast(startPos, Vector3.up, out hit, maxDistance, hitLayers))
        {
            endPos = hit.point;

            // 处理伤害
            damageTimer += Time.deltaTime;
            if (damageTimer >= 0.1f) // 每0.1秒造成一次伤害
            {
                damageTimer = 0;
                ApplyDamage(hit.collider.gameObject);
            }
        }

        // 更新激光视觉效果
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    void ApplyDamage(GameObject hitObject)
    {
        // 处理敌人部件伤害
        Enemy enemy = hitObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            // 对于Enemy_4及其子类
            if (enemy is Enemy_4)
            {
                Enemy_4 enemy4 = enemy as Enemy_4;
                Part part = enemy4.FindPart(hitObject);
                if (part != null)
                {
                    if (part.protectedBy != null)
                    {
                        foreach (string s in part.protectedBy)
                        {
                            if (!enemy4.Destoryed(s))
                            {
                                return;
                            }
                        }
                    }
                    part.health -= damagePerSecond * 0.1f; // 每次伤害
                    enemy4.ShowLocalizedDamage(part.mat);

                    if (part.health <= 0)
                    {
                        part.go.SetActive(false);
                        // 检查是否所有部件都被摧毁
                        bool allDestroyed = true;
                        foreach (Part p in enemy4.parts)
                        {
                            if (!enemy4.Destoryed(p))
                            {
                                allDestroyed = false;
                                break;
                            }
                        }
                        if (allDestroyed)
                        {
                            Main.S.ShipDestoryed(enemy4);
                            Destroy(enemy4.gameObject);
                        }
                    }
                }
            }
            else // 对于其他敌人
            {
                enemy.health -= damagePerSecond * 0.1f;
                enemy.ShowDamage();
                if (enemy.health <= 0)
                {
                    Main.S.ShipDestoryed(enemy);
                    Destroy(enemy.gameObject);
                }
            }
        }
    }
}