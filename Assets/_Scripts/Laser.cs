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
            Destroy(gameObject); // ���ټ������
            return;
        }
        // ������㣨����λ�ã�
        Vector3 startPos = transform.position;
        // �����յ㣨���Ϸ��䣩
        Vector3 endPos = startPos + Vector3.up * maxDistance;

        // ���߼��
        RaycastHit hit;
        if (Physics.Raycast(startPos, Vector3.up, out hit, maxDistance, hitLayers))
        {
            endPos = hit.point;

            // �����˺�
            damageTimer += Time.deltaTime;
            if (damageTimer >= 0.1f) // ÿ0.1�����һ���˺�
            {
                damageTimer = 0;
                ApplyDamage(hit.collider.gameObject);
            }
        }

        // ���¼����Ӿ�Ч��
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    void ApplyDamage(GameObject hitObject)
    {
        // ������˲����˺�
        Enemy enemy = hitObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            // ����Enemy_4��������
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
                    part.health -= damagePerSecond * 0.1f; // ÿ���˺�
                    enemy4.ShowLocalizedDamage(part.mat);

                    if (part.health <= 0)
                    {
                        part.go.SetActive(false);
                        // ����Ƿ����в��������ݻ�
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
            else // ������������
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