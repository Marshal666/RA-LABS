using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{

    public Particles ParticleSystem;

    public Vector3 StartVelocity;

    float startTime = 0;

    MeshRenderer Renderer;

    Vector3 originalScale;

    private void Awake()
    {
        Renderer = GetComponent<MeshRenderer>();
        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        startTime = Time.time;
        Renderer.material.color = ParticleSystem.ColorOverLifetime.Evaluate(0);
    }

    // Update is called once per frame
    void Update()
    {
        float t = (Time.time - startTime) / ParticleSystem.Duration;
        //print(t);
        if(t >= 1f)
        {
            gameObject.SetActive(false);
            ParticleSystem.Holder.ReturnObject(gameObject);
            return;
        }
        transform.position += (new Vector3(ParticleSystem.XVelocity.Evaluate(t), ParticleSystem.YVelocity.Evaluate(t), ParticleSystem.ZVelocity.Evaluate(t)) + StartVelocity) * Time.deltaTime;
        Renderer.material.color = ParticleSystem.ColorOverLifetime.Evaluate(t);
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        transform.localScale = originalScale * ParticleSystem.SizeOverLiftime.Evaluate(t);
    }

    public void Init()
    {
        Renderer.material.color = ParticleSystem.ColorOverLifetime.Evaluate(0);
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        transform.localScale = originalScale * ParticleSystem.SizeOverLiftime.Evaluate(0);
    }
}
