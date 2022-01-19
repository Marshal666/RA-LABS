using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{

    Rigidbody rig;

    CapsuleCollider capsule;

    public float Speed = 3f;

    public float JumpVelocity = 3f;

    public float JumpCooldownTime = 0.1f;

    public bool OnGround = false;

    public LayerMask GroundLayers;

    public KeyCode DigKey = KeyCode.Mouse0;

    public float DigSphereRadius = 0.75f;
    public float DigDistance = 3f;

    public Vector3 DigDirectionOffsetPoint = new Vector3(0, 0, 0.5f);

    public ObjectHolder BombsContainer;

    public Vector3 BombSpawnOffset = new Vector3(0.5f, 0, 0.25f);
    public Vector3 BombDropVelocity = new Vector3(0, 5f, 5f);

    public KeyCode DropBombKey = KeyCode.Mouse1;

    public KeyCode BuildWorldKey = KeyCode.Mouse2;
    public Vector3 BuildWorldOffset = new Vector3(0, -1.5f, 0);
    public float BuildWorldRadius = 1f;

    public Animator anim;

    PlayerCamera pcam;

    MarchingCubes mc;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        mc = MarchingCubes.Instance;
        pcam = PlayerCamera.Instance;
    }

    Vector3 input;
    bool jump = false;
    bool jumped = false;

    private void Update()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        anim.SetBool("Run", input.sqrMagnitude > 0.0001f);

        CheckOnGround();

        if (!jump && OnGround)
        {
            jump = Input.GetKey(KeyCode.Space);
        }

        if(Input.GetKeyDown(DigKey))
        {
            Vector3 p = rig.position + transform.TransformVector(DigDirectionOffsetPoint);
            Ray ray = new Ray(p, p - pcam.transform.position);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, DigDistance, GroundLayers))
            {
                mc.DrawInSphereLossy(hit.point, DigSphereRadius, mc.ValueMin);
            }
            anim.SetTrigger("Push");
        }

        if(Input.GetKeyDown(DropBombKey))
        {
            GameObject bomb = BombsContainer.GetObject();
            bomb.transform.position = transform.TransformPoint(BombSpawnOffset);
            Rigidbody r = bomb.GetComponent<Rigidbody>();
            r.velocity = Quaternion.Euler(0, pcam.dx, 0) * BombDropVelocity;
            anim.SetTrigger("Throw");
        }

        if(Input.GetKeyDown(BuildWorldKey))
        {
            mc.DrawInSphereLossy(transform.TransformPoint(BuildWorldOffset), BuildWorldRadius, mc.ValueMax);
        }

        transform.rotation = Quaternion.Euler(0, pcam.dx, 0);
    }

    IEnumerator JumpCooldownTimer()
    {
        yield return new WaitForSeconds(JumpCooldownTime);
        jumped = false;
    }

    void CheckOnGround()
    {
        Vector3 p1 = rig.position + capsule.center - Vector3.up * capsule.height / 2;
        Vector3 p2 = p1 + Vector3.up * capsule.height;
        OnGround = Physics.SphereCast(rig.position, capsule.radius * 0.999f, Vector3.down, out _, capsule.height / 3.95f, GroundLayers);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 velocity = input * Speed;
        velocity = Quaternion.Euler(0, pcam.dx, 0) * velocity;

        CheckOnGround();

        Debug.DrawLine(transform.position, transform.position + velocity);

        if(jump && OnGround && (!jumped || rig.velocity.y <= 0f))
        {
            rig.velocity += Vector3.up * JumpVelocity;
            jumped = true;
            StartCoroutine(JumpCooldownTimer());
        }

        transform.rotation = Quaternion.Euler(0, pcam.dx, 0);

        rig.velocity = new Vector3(velocity.x, rig.velocity.y, velocity.z);
        jump = false;
    }
}
