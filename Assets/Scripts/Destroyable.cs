using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public int health;

    [Header("Fracture")]
    public bool fractured;
    public Transform meshParent;

    [Header("Hit")]
    public bool hitParticleBool;
    public ParticleSystem hitParticle;

    [Header("DestroyExplosion")]
    public bool destroyExplosion;
    public ParticleSystem destroyParticle;

    [Header("Force")]
    public bool force;
    public float x,y,z;

    [Header("Trash")]
    public bool goingToTrash;

    [Header("Layer")]
    public bool ignoreLayer;

    [Header("IsDisactive")]
    public bool fracturedObjectsIsDisactive;

    [Header("OutlineMesh")]
    public GameObject outlineMesh;

    [Header("IsCar")]
    public bool isCar;
    public float maxHealth;
    public GameObject[] deformMeshes;
    public GameObject outlineMesh2, outlineMesh3;

    [Header("IsHome")]
    public bool isHome;

    private void Start()
    {
        if (isCar)
        {
            maxHealth = health;
            for (int i = 0; i < deformMeshes.Length; i++)
            {
                deformMeshes[i].gameObject.SetActive(false);
            }
            deformMeshes[0].gameObject.SetActive(true);
        }
       
    }
    private void Update()
    {
        if (isCar)
        {
            if (health > 0 && deformMeshes.Length==3)
            {
                if (health < maxHealth * (0.66f) && health > maxHealth * (0.33f) && !deformMeshes[1].gameObject.activeInHierarchy)
                {
                    for (int i = 0; i < deformMeshes.Length; i++)
                    {
                        deformMeshes[i].gameObject.SetActive(false);
                    }
                    deformMeshes[1].gameObject.SetActive(true);
                }
                if (health < maxHealth * (0.33f) && !deformMeshes[2].gameObject.activeInHierarchy)
                {
                    for (int i = 0; i < deformMeshes.Length; i++)
                    {
                        deformMeshes[i].gameObject.SetActive(false);
                    }
                    deformMeshes[2].gameObject.SetActive(true);
                }
            }
            
        }
    }



    public void Hit() 
    {
        if (hitParticleBool)
        {
            hitParticle.Play();
        }
    }

    public void Destroyed()
    {
        if (fractured)
        {
            for (int i = meshParent.childCount - 1; i >= 0; i--)
            {
                meshParent.GetChild(i).gameObject.AddComponent<Rigidbody>();
                meshParent.GetChild(i).gameObject.AddComponent<BoxCollider>();
                if (fracturedObjectsIsDisactive)
                {
                    meshParent.GetChild(i).gameObject.SetActive(true);
                }
                if (ignoreLayer)
                {
                    meshParent.GetChild(i).gameObject.layer = 7;
                }
                if (goingToTrash)
                {
                    meshParent.GetChild(i).gameObject.AddComponent<Trash>();
                }
                if (force)
                {
                    meshParent.GetChild(i).GetComponent<Rigidbody>().AddForce(Random.Range(-x, x), y, Random.Range(-z, z));
                }
                if (meshParent.GetChild(i).GetComponent<Outline>())
                {
                    meshParent.GetChild(i).GetComponent<Outline>().enabled = false;
                }
                meshParent.GetChild(i).parent = GameObject.FindGameObjectWithTag("Trash").transform;
              
            }
        }
        if (destroyExplosion)
        {
            destroyParticle.transform.parent = GameObject.FindGameObjectWithTag("Trash").transform;
            destroyParticle.Play();
        }
        Destroy(gameObject);
    }

}



