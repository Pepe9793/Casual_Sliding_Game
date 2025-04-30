using UnityEngine;

public class BG_Scroll : MonoBehaviour
{
    public float _scrollspeed = 0.3f;
    
    private MeshRenderer _meshrenderer;

    private void Awake()
    {
        _meshrenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        Scroll();
    }

    void Scroll()
    {
        Vector2 offset = _meshrenderer.sharedMaterial.GetTextureOffset("_MainTex");
        offset.y += _scrollspeed * Time.deltaTime;

        _meshrenderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}
