using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Dummy : MonoBehaviour
{
    [HideInInspector] public Player player;
    public Rigidbody rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Transform targetObject;
    [HideInInspector] public Vector3 dir;
    public Transform bulletParentTransform;
    public float fireRate;
    [HideInInspector] public float fireTimer;
    [HideInInspector] public int bulletNumber;
    public ParticleSystem bulletMuzzle;
    public float moveSpeed;

    public List<GameObject> weapons;
    
    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody>();
        rb.solverIterations = 1;
        anim = GetComponent<Animator>();
        for (int i = 0; i < player.nodeParent.childCount; i++)
        {
            if (player.nodeParent.GetChild(i).GetComponent<Node>().isEmpty)
            {
                player.nodeParent.GetChild(i).GetComponent<Node>().isEmpty = false;
                targetObject = player.nodeParent.GetChild(i);
                break;
            }
            
        }
        RefreshGun();
       
    }
    public void RefreshGun()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].SetActive(false);
        }
        weapons[player.gunLevel].SetActive(true);
    }
    private void FixedUpdate()
    {
        SoFar();
        if (player.fighting && !player.moving)
        {
            Fighting();

        }
        else
        {
            if (Vector3.Distance(transform.position, targetObject.position) > 0.1f)
            {
                Running();
            }
            else
            {
                if (player.fighting && !player.moving)
                {
                    Fighting();
                }
                else
                {
                    Waiting();
                }
            }
        }

    }
    public void Running()
    {
        if (player != null)
        {
            dir = (targetObject.position - transform.position).normalized * moveSpeed;
            rb.velocity = dir;
            transform.rotation = Quaternion.LookRotation(rb.velocity);
            rb.isKinematic = false;
            if (bulletParentTransform.gameObject.activeInHierarchy)
            {
                bulletParentTransform.gameObject.SetActive(false);
            }
            anim.SetBool("run", true);
            anim.SetBool("idle", false);
            anim.SetBool("shoot", false);
            anim.SetBool("stick", false);
        }
        else
        {
            Surrender();
        }
        
    }
    public void Surrender()
    {
        rb.isKinematic = true;
        anim.SetBool("surrender", true);
    }
    public void Fighting()
    {
        if (player != null)
        {
            if (player.closestObject != null)
            {
                if (player.currentWeapon.isStick)
                {
                    anim.SetBool("stick", true);
                }
                else
                {
                    anim.SetBool("shoot", true);
                }
                rb.isKinematic = true;
                anim.SetBool("run", false);
                anim.SetBool("idle", false);
                if (!player.isEnemyInRange)
                {
                    var lookPos = player.closestObject.position - transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 50);
                }
                else
                {
                    if (player.closestEnemy != null)
                    {
                        var lookPos = player.closestEnemy.position - transform.position;
                        lookPos.y = 0;
                        var rotation = Quaternion.LookRotation(lookPos);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 50);
                    }

                }

                Fire();
            }
        }
        else
        {
            Surrender();
        }

    }
    void Waiting()
    {
        if (player != null)
        {
            if (bulletParentTransform.gameObject.activeInHierarchy)
            {
                bulletParentTransform.gameObject.SetActive(false);
            }
            rb.isKinematic = true;
            anim.SetBool("shoot", false);
            anim.SetBool("run", false);
            anim.SetBool("idle", true);
            anim.SetBool("stick", false);
        }
        else
        {
            Surrender();
        }
        
    }
    public void Fire()
    {
        if (player.currentWeapon.isStick)
        {
            bulletParentTransform.gameObject.SetActive(false);
        }
        else
        {
            bulletParentTransform.gameObject.SetActive(true);
        }
       
        fireTimer += Time.deltaTime;
        if (fireTimer > fireRate)
        {
            if (!player.currentWeapon.isStick)
            {
                bulletMuzzle.Play();
            }
            fireTimer = 0;
            bulletParentTransform.GetChild(bulletNumber).transform.localPosition = Vector3.zero;
            bulletParentTransform.GetChild(bulletNumber).gameObject.SetActive(true);
            bulletParentTransform.GetChild(bulletNumber).GetComponent<Bullet>().BulletDisactive();
            if (!player.isEnemyInRange)
            {
                bulletParentTransform.GetChild(bulletNumber).DOMove(player.closestObject.position, 0.2f).SetEase(Ease.Linear);
            }
            else
            {
                if (player.closestEnemy != null)
                {
                    bulletParentTransform.GetChild(bulletNumber).DOMove(player.closestEnemy.position, 0.2f).SetEase(Ease.Linear);
                }
               
            }
            
            bulletNumber += 1;
            if (bulletNumber >= bulletParentTransform.childCount)
            {
                bulletNumber = 0;
            }
        }
    }
    public void SoFar()
    {
        if (Vector3.Distance(transform.position, targetObject.position) > 15f)
        {
            transform.position = new Vector3(targetObject.position.x + Random.Range(-10f, 10f), transform.position.y, targetObject.position.z);
        }
    }
}
