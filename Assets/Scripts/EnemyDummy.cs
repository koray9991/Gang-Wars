using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EnemyDummy : MonoBehaviour
{
    public Player player;
    public Ai leader;
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
    public List<SkinnedMeshRenderer> meshes;
    public Material[] mats;

    public List<GameObject> weapons;
    void Start()
    {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody>();
        rb.solverIterations = 1;
        anim = GetComponent<Animator>();
        for (int i = 0; i < leader.nodeParent.childCount; i++)
        {
            if (leader.nodeParent.GetChild(i).GetComponent<Node>().isEmpty)
            {
                leader.nodeParent.GetChild(i).GetComponent<Node>().isEmpty = false;
                targetObject = leader.nodeParent.GetChild(i);
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
        weapons[leader.gunLevel].SetActive(true);
    }
    private void FixedUpdate()
    {
        SoFar();
        if (leader.shootingTarget)
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
                Fighting();
            }
        }
        
       
    }

    public void Running()
    {
        if (leader != null)
        {
            dir = (targetObject.position - transform.position).normalized * moveSpeed;
            rb.velocity = dir;
            transform.rotation = Quaternion.LookRotation(rb.velocity);
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
    public void Fighting()
    {
        if (leader != null)
        {
            if (leader.closestObject != null)
            {
                if (leader.currentWeapon.isStick)
                {
                    anim.SetBool("stick", true);
                }
                else
                {
                    anim.SetBool("shoot", true);
                }
                anim.SetBool("run", false);
                if (!leader.attacked)
                {
                    var lookPos = leader.closestObject.position - transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 50);
                }
                else
                {
                    if (leader.player != null)
                    {
                        var lookPos = leader.player.transform.position - transform.position;
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
  public void Surrender()
    {
         rb.isKinematic = true;
        anim.SetBool("surrender", true);
        if (Vector3.Distance(transform.position, player.transform.position) < 5)
        {
            player.CapturedDummy(transform.position);
            Destroy(gameObject);

        }
        
        
    }
    public void Fire()
    {
        if (leader.currentWeapon.isStick)
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
            if (!leader.currentWeapon.isStick)
            {
                bulletMuzzle.Play();
            }
            fireTimer = 0;
            bulletParentTransform.GetChild(bulletNumber).transform.localPosition = Vector3.zero;
            bulletParentTransform.GetChild(bulletNumber).gameObject.SetActive(true);
            bulletParentTransform.GetChild(bulletNumber).GetComponent<Bullet>().BulletDisactive();

            if (!leader.attacked)
            {
                bulletParentTransform.GetChild(bulletNumber).DOMove(leader.closestObject.position, 0.2f).SetEase(Ease.Linear);
            }
            else
            {
                if (leader.player != null)
                {
                    bulletParentTransform.GetChild(bulletNumber).DOMove(leader.player.transform.position, 0.2f).SetEase(Ease.Linear);
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Dummy>())
        {
            player.CapturedDummy(transform.position);
            Destroy(gameObject);
        }
    }
}
