using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class custom : MonoBehaviour
{
    // Start is called before the first frame update
    public Material material;
    public Texture[] textures;
    int count;
    public void Enter()
    {
        
        material.SetFloat("_EmissiveExposureWeight", 0.7f);
    }
    public void exit()
    {
        
        material.SetFloat("_EmissiveExposureWeight", 1f);
    }
    public void click()
    {
        material.mainTexture= textures[count];
        count++;
        if(count>=textures.Length)
        {
            count= 0;
        }
    }
}
