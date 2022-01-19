using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    public MarchingCubes Terrain;

    public int MinFill = 32;

    public float widthFactor = 0.02f;
    public float depthFactor = 0.02f;
    
    // Start is called before the first frame update
    void Start()
    {
        int sx = (int)(Terrain.ValuesCount.x * widthFactor);
        int sz = (int)(Terrain.ValuesCount.z * widthFactor);
        for(int i = sx; i < Terrain.ValuesCount.x * (1 - widthFactor); i++)
        {
            for(int j = sz; j < Terrain.ValuesCount.z * (1 - depthFactor); j++)
            {
                for(int k = 0; k < MinFill; k++)
                {
                    Terrain.SetValue(i, k, j, Terrain.ValueMax);
                }
            }
        }
        Terrain.Redraw();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
