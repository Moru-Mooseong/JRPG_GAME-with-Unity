using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AutoMapGenerator : MonoBehaviour
{
    public Texture2D texture;

    public Texture2D RGBTexture;

    public MeshRenderer Testmesh;
    public RawImage raw;

    private void Start()
    {
        TextureRead();
    }


    private void TextureRead()
    {
        var ref_ = texture.Size();
        Debug.Log($"텍스쳐 사이즈 : {ref_}");
        Texture2D renewTexture = texture;
        RGBTexture = renewTexture;
        for (int x = 0; x < 256; x++)
        {
            for (int y = 0; y < 256; y++)
            {
                var colorData = texture.GetPixel(x, y);
                var gammar = texture.GetPixel(x, y).grayscale;
                //colorData.r = gammar;
                //colorData.g = gammar;
                //colorData.b = gammar;

                RGBTexture.SetPixel(x, y, colorData);
            }
        }
        Testmesh.material.SetTexture(0, RGBTexture);
        raw.texture = RGBTexture;
        //raw.texture = texture;
    }
}
