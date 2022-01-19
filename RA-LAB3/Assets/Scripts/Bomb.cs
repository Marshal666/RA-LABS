using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bomb : MonoBehaviour
{

    MarchingCubes mc;
    Rigidbody rb;

    public float Timeout = 15f;

    public ObjectHolder Holder;

    bool HasTracerParticleEffect = true;
    public ObjectHolder TracerEffects;

    bool HasExplosionParticleEffect = true;
    public ObjectHolder ParticleEffects;

    public float ExplosionRadius = 2f;

    // Start is called before the first frame update
    void Awake()
    {
        mc = MarchingCubes.Instance;
        rb = GetComponent<Rigidbody>();

        if(!TracerEffects)
            HasTracerParticleEffect = false;

        if(!ParticleEffects)
            HasExplosionParticleEffect = false;

    }

    private void OnEnable()
    {
        if (HasTracerParticleEffect)
        {
            var g = TracerEffects.GetObject();
            g.transform.position = transform.position;
            g.transform.SetParent(transform);
        }
        StartCoroutine(TimeoutM());
    }

    IEnumerator TimeoutM()
    {
        yield return new WaitForSeconds(Timeout);
        if (HasExplosionParticleEffect)
        {
            GameObject exp = ParticleEffects.GetObject();
            exp.transform.position = transform.position;
        }

        mc.DrawInSphereLossy(transform.position, ExplosionRadius, mc.ValueMin);
        rb.velocity = Vector3.zero;

        GameObject g = ParticleEffects.GetObject();
        g.transform.position = transform.position;
        Holder.ReturnObject(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(HasExplosionParticleEffect)
        {
            GameObject exp = ParticleEffects.GetObject();
            exp.transform.position = transform.position;
        }

        mc.DrawInSphereLossy(transform.position, ExplosionRadius, mc.ValueMin);
        rb.velocity = Vector3.zero;

        GameObject g = ParticleEffects.GetObject();
        g.transform.position = transform.position;
        Holder.ReturnObject(gameObject);
        StopAllCoroutines();
    }
}
