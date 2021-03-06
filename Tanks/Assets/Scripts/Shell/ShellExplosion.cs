using UnityEngine;
using Mirror;

public class ShellExplosion : NetworkBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;
    [SyncVar]
    public float m_CurrentLaunchForce = 15f;

    private void Start()
    {
        gameObject.GetComponent<Rigidbody>().velocity = m_CurrentLaunchForce * gameObject.transform.forward;
        Destroy(gameObject, m_MaxLifeTime);
    }

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            if (!targetHealth)
                continue;

            float damage = CalculateDamage(targetRigidbody.position);
            targetHealth.TakeDamage(damage);
        }

        PlayShellExplosion();
    }

    [ClientRpc]
    private void PlayShellExplosion()
    {
        m_ExplosionParticles.transform.parent = null;
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
        Destroy(m_ExplosionParticles.gameObject, mainModule.duration);

        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        float damage = relativeDistance * m_MaxDamage;
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}