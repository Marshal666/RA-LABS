using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticlesCS : MonoBehaviour
{

    //public int MaxCount = 1000;
    public float SpawnRate = 20;
    public float Duration = 5;
    public float StartSpeed = 2f;

    public Mesh SpawnMesh;
    public Vector3 SpawnMeshOffset;
    public Vector3 SpawnMeshRotation;
    public Vector3 SpawnMeshScale = Vector3.one;

    public Mesh ParticleMesh;
    public Material ParticleMaterial;

    public AnimationCurve XVelocity;
    public AnimationCurve YVelocity;
    public AnimationCurve ZVelocity;

    public Gradient ColorOverLifetime;

    public AnimationCurve SizeOverLiftime = AnimationCurve.Constant(0, 1, 1);

    public enum PlaybackMode
    {
        Loop,
        Once
    }

    public int seed;

    public PlaybackMode Mode;

    public struct Particle
    {
        public Matrix4x4 Transform;
        public Vector3 StartVelocity;
        public Vector3 InputVelocity;
    }

    //[HideInInspector]
    //public ObjectHolder Holder;
    System.Random random;
    Weighted_Randomizer.StaticWeightedRandomizer<int> TrianglePicker;

    // Start is called before the first frame update
    void Start()
    {
        //normals are per vertex
        random = new System.Random(seed);
        TrianglePicker = new Weighted_Randomizer.StaticWeightedRandomizer<int>(seed);
        int tric = SpawnMesh.triangles.Length;
        float[] areas = new float[tric / 3];
        for (int i = 0; i < tric; i += 3)
        {
            areas[i / 3] = Vector3.Cross(
                SpawnMesh.vertices[SpawnMesh.triangles[i + 1]] - SpawnMesh.vertices[SpawnMesh.triangles[i]],
                SpawnMesh.vertices[SpawnMesh.triangles[i + 2]] - SpawnMesh.vertices[SpawnMesh.triangles[i]]).magnitude * 0.5f;
            //print(areas[i / 3]);
        }
        float arsum = areas.Sum();
        for (int i = 0; i < tric; i += 3)
        {
            TrianglePicker.Add(i, Mathf.CeilToInt((areas[i / 3] / arsum) * 1000f));
        }

        //GameObject particleInstance = new GameObject("ParticleInstance");
        //particleInstance.AddComponent<MeshFilter>().mesh = ParticleMesh;
        //particleInstance.AddComponent<MeshRenderer>().sharedMaterial = ParticleMaterial;
        //particleInstance.transform.parent = transform;
        //particleInstance.SetActive(false);
        //particleInstance.AddComponent<Particle>().ParticleSystem = this;

        //Holder = gameObject.AddComponent<ObjectHolder>();
        //Holder.Object = particleInstance;
        //Holder.Size = Mathf.CeilToInt(SpawnRate * Duration);

    }

    (Vector3 pos, Vector3 dir) GetSpawnPosition()
    {
        int tri = TrianglePicker.NextWithReplacement();
        Vector3 A = SpawnMesh.vertices[SpawnMesh.triangles[tri]];
        Vector3 B = SpawnMesh.vertices[SpawnMesh.triangles[tri + 1]];
        Vector3 C = SpawnMesh.vertices[SpawnMesh.triangles[tri + 2]];

        Vector3 nA = SpawnMesh.normals[SpawnMesh.triangles[tri]];
        Vector3 nB = SpawnMesh.normals[SpawnMesh.triangles[tri + 1]];
        Vector3 nC = SpawnMesh.normals[SpawnMesh.triangles[tri + 2]];


        Vector3 a = B - A;
        Vector3 b = C - A;
        float n1 = (float)random.NextDouble();
        float n2 = (float)random.NextDouble();
        if (n1 + n2 > 1f)
            (n1, n2) = (1f - n1, 1f - n2);
        Vector3 olp = transform.position;
        Quaternion oldR = transform.rotation;
        Vector3 olds = transform.localScale;
        transform.position += SpawnMeshOffset;
        transform.rotation = Quaternion.Euler(SpawnMeshRotation);
        transform.localScale = SpawnMeshScale;
        Vector3 spawnPosition = transform.TransformPoint(SpawnMesh.vertices[SpawnMesh.triangles[tri]] + a * n1 + b * n2);
        Vector3 Normal = (Vector3.Lerp(nA, nB, n1) + Vector3.Lerp(nA, nC, n2)) / 2f;
        var ret = (spawnPosition, transform.TransformVector(Normal).normalized);
        transform.position = olp;
        transform.rotation = oldR;
        transform.localScale = olds;
        return ret;
    }

    IEnumerator Cycle()
    {
        float time = 0;
        float spawnInterval = 1f / SpawnRate;
        float spawnTime = 0f;
        while (time < Duration)
        {
            if (spawnTime >= spawnInterval)
            {
                spawnTime = 0f;
                //GameObject po = Holder.GetObject();
                //po.SetActive(true);
                //Particle p = po.GetComponent<Particle>();
                var pn = GetSpawnPosition();
                //po.transform.position = pn.pos;
                //p.StartVelocity = pn.dir * StartSpeed;
                //p.Init();
            }
            spawnTime += Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }
        systemCoroutine = null;
        yield break;
    }

    //List<(Vector3, Vector3)> pts = new List<(Vector3, Vector3)>();

    Coroutine systemCoroutine = null;

    bool played = false;

    // Update is called once per frame
    void Update()
    {
        switch (Mode)
        {
            case PlaybackMode.Loop:
                if (systemCoroutine == null)
                {
                    systemCoroutine = StartCoroutine(Cycle());
                }
                break;
            case PlaybackMode.Once:
                if (!played)
                {
                    systemCoroutine = StartCoroutine(Cycle());
                    played = true;
                }
                break;
            default:
                break;
        }

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    pts.Clear();
        //    for (int i = 0; i < MaxCount; i++)
        //    {
        //        var s = GetSpawnPosition();
        //        print(s);
        //        pts.Add(s);
        //    }
        //}
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (SpawnMesh != null)
            Gizmos.DrawWireMesh(SpawnMesh, transform.position + SpawnMeshOffset, Quaternion.Euler(SpawnMeshRotation), SpawnMeshScale);
        //if (pts != null && pts.Count > 0)
        //{
        //    for (int i = 0; i < pts.Count; i++)
        //    {
        //        Gizmos.DrawCube(pts[i].Item1, Vector3.one * 0.01f);
        //        Gizmos.DrawLine(pts[i].Item1, pts[i].Item1 + pts[i].Item2);
        //    }
        //}
    }


}
