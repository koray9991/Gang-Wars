using UnityEngine;
using System.Collections;

public class AutoTransparent : MonoBehaviour
{
    private Shader[] m_OldShaders = null;
    private Color[] m_colors;
    private Texture[] m_textures;

    private Renderer m_renderer;

    private Coroutine coroutine;

    void Start()
    {
        //init
        this.m_renderer = this.GetComponent<Renderer>();
        this.m_OldShaders = new Shader[this.m_renderer.materials.Length];
        this.m_colors = new Color[this.m_renderer.materials.Length];
        this.m_textures = new Texture[this.m_renderer.materials.Length];


        for (int i = 0; i < this.m_renderer.materials.Length; i++)
        {

            if (this.m_renderer.materials[i].shader != null)
            {
                this.m_OldShaders[i] = this.m_renderer.materials[i].shader;
            }
            else
            {
                this.m_OldShaders[i] = null;
            }
            if (this.m_renderer.materials[i].color != null)
            {
                this.m_colors[i] = this.m_renderer.materials[i].color;
            }
            else
            {
                this.m_colors[i] = default;
            }

            if (this.m_renderer.materials[i].mainTexture != null)
            {
                this.m_textures[i] = this.m_renderer.materials[i].mainTexture;
            }
            else
            {
                this.m_textures[i] = null;
            }

        }
    }

    //reset gameobject
    private IEnumerator reset_gameobject()
    {
        yield return new WaitForSeconds(1f);
       
        //set the shader and layer back
        for (int i = 0; i < this.m_renderer.materials.Length; i++)
        {
            this.m_renderer.materials[i].shader = this.m_OldShaders[i];

            this.m_renderer.materials[i].color = this.m_colors[i];

            this.m_renderer.materials[i].mainTexture = this.m_textures[i];
        }

        this.gameObject.layer = 0;
    }

    //set the gameobject 
    public void set_obj_transparent_and_change_gamelayer(float alpha)
    {
        if (this.gameObject.layer != 1 && this.m_renderer != null)
        {

            for (int i = 0; i < this.m_renderer.materials.Length; i++)
            {
                //this.m_renderer.materials[i].shader = Shader.Find("EasyGameStudio/transparent");
                this.m_renderer.materials[i].shader = Shader.Find("Shader Graphs/TransparentShader");

                //Color C = this.m_renderer.materials[i].color;
                //C.a = alpha;
                //this.m_renderer.materials[i].SetTexture("main_texture", this.m_renderer.materials[i].GetTexture("_BaseMap"));
                //this.m_renderer.materials[i].SetColor("main_color", this.m_renderer.materials[i].GetColor("_BaseColor"));
                /***********/
                //this.m_renderer.materials[i].SetColor("main_color", this.m_colors[i]);
                //this.m_renderer.materials[i].SetTexture("main_texture", this.m_textures[i]);
                this.m_renderer.materials[i].SetColor("_Color", this.m_colors[i]);
                this.m_renderer.materials[i].SetTexture("_Texture", this.m_textures[i]);


                //this.m_renderer.materials[i].SetFloat("alpha", alpha);
                this.m_renderer.materials[i].SetFloat("_Opacity", alpha);
            }

            this.gameObject.layer = 1;
        }

        if (this.coroutine != null)
        {
            StopCoroutine(this.coroutine);
        }

        this.coroutine = StartCoroutine(this.reset_gameobject());
    }
}
