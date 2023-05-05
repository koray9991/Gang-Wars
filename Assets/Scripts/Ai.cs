using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
public class Ai : MonoBehaviour
{
    [HideInInspector] public Player player;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public NavMeshAgent navMesh;
    public Transform closestObject;
   public bool movingTarget;
    public bool shootingTarget;
    [HideInInspector] public float movingTargetTimer;
    public float range;
    public Transform nodeParent;
    public Transform rangeObject;

    public Transform bulletParentTransform;
    public float fireRate;
    [HideInInspector] public float fireTimer;
    [HideInInspector] public int bulletNumber;
    public ParticleSystem bulletMuzzle;

    public GameObject dummy;
    public Transform dummyParent;
    [HideInInspector] public int dummyCount;
    [HideInInspector] public int power;

    [HideInInspector] public float maxHealth;
    public float health;
    [HideInInspector] public float regenerateTimer;
    public Image healthBar;
    float healthTimer;
    public TextMeshProUGUI countText;
    [HideInInspector] public float destroyableObjectTimer;

    public bool attacked;
    public bool isDead;

    public List<GameObject> weapons;
    public Weapon currentWeapon;
    public int gunLevel;

   
    public float destroyableShootTime;

    public bool isTutorial;
   
    public enum Colors
    {
        Red,
        Green,
        Yellow

    }
    public Colors color;
    void Start()
    {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        navMesh = GetComponent<NavMeshAgent>();

        dummyCount = dummyParent.childCount;
        power = dummyCount + 1;
        countText.text = power.ToString();

        maxHealth = dummyCount*10+ 50;
        health = maxHealth;
        healthBar.fillAmount = health / maxHealth;

        movingTarget = true;

        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].SetActive(false);
        }
        weapons[gunLevel].SetActive(true);
        currentWeapon = weapons[gunLevel].GetComponent<Weapon>();
        range = currentWeapon.range;
        rangeObject.transform.localScale = new Vector3(range * 2, 0.01f, range * 2);
        if (isTutorial)
        {
            
            DOVirtual.DelayedCall(0.3f, () => {

                NewDummy(); NewDummy(); NewDummy(); NewDummy();
            });
        }
    }


    void Update()
    {
        rangeObject.transform.localScale = new Vector3(range * 2, 0.01f, range * 2);
        RegenerateHealth();
        checkPlayerDistanceToAttack();
        if (movingTarget)
        {
            MoveClosestTarget();
        }
        if (shootingTarget)
        {
            Shooting();
        }
        if (attacked)
        {
            Attacked();
        }
        if(!movingTarget && !attacked && !shootingTarget)
        {
            movingTarget = true;
        }
        if (health <= 0)
        {
            isDead = true;
            Destroy(gameObject);
        }
    }
    void RegenerateHealth()
    {
        regenerateTimer += Time.deltaTime;
        if (regenerateTimer > 7)
        {
            HealthCheck();
        }
    }
    void HealthCheck()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (health < maxHealth)
        {
            healthTimer += Time.deltaTime;
            if (healthTimer > 1)
            {
                healthTimer = 0;
                health += maxHealth / 10;
            }
        }
        healthBar.fillAmount = health / maxHealth;
    }
    void Attacked()
    {
        if (player != null)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < range)
            {
                Shooting();
                anim.SetBool("run", false);
                if (currentWeapon.isStick)
                {
                    anim.SetBool("stick", true);
                }
                else
                {
                    anim.SetBool("shoot", true);
                }
                AnimatorType();

                navMesh.speed = 0;
                movingTarget = false;
            }
            else
            {
                shootingTarget = false;
                movingTarget = true;
                navMesh.SetDestination(player.transform.position);
                anim.SetBool("run", true);
                anim.SetBool("shoot", false);
                anim.SetBool("stick", false);
                AnimatorType();
                navMesh.speed = 3;
            }
        }
        else
        {
            attacked = false;
          
            movingTarget = true;
            
        }
      
    }
    void checkPlayerDistanceToAttack()
    {
        if (player != null)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < range)
            {
                attacked = true;
            }
            else
            {
                if (player.attackingEnemy == null)
                {
                    if (attacked)
                    {
                        attacked = false;
                        movingTarget = true;
                        shootingTarget = false;
                    }
                   
                }
            }
        }
       
    }
    public void MoveClosestTarget()
    {
        if (!attacked)
        {
            float distanceClosestTarget = Mathf.Infinity;
            Destroyable closestTarget = null;
            Destroyable[] allTargets = FindObjectsOfType<Destroyable>();
            if (allTargets.Length != 0)
            {
                foreach (Destroyable currentTarget in allTargets)
                {
                    float distanceToTarget = (currentTarget.transform.position - this.transform.position).sqrMagnitude;
                    if (distanceToTarget < distanceClosestTarget)
                    {
                        distanceClosestTarget = distanceToTarget;
                        closestTarget = currentTarget;
                    }
                }
                shootingTarget = false;
                navMesh.speed = 3;
                closestObject = closestTarget.transform;
                movingTargetTimer += Time.deltaTime;
                if (movingTargetTimer > 1)
                {
                    movingTargetTimer = 0;
                    navMesh.SetDestination(closestObject.position);
                    anim.SetBool("run", true);
                    anim.SetBool("shoot", false);
                    anim.SetBool("stick", false);
                    AnimatorType();

                }
                if (Vector3.Distance(closestObject.position, transform.position) < range)
                {
                    movingTarget = false;
                    navMesh.speed = 0;
                    anim.SetBool("run", false);

                    if (currentWeapon.isStick)
                    {
                        anim.SetBool("stick", true);
                    }
                    else
                    {
                        anim.SetBool("shoot", true);
                    }
                    AnimatorType();
                    shootingTarget = true;
                }
            }
            else
            {
                attacked = true;
            }

        }
       
       
    }
    void Shooting()
    {
        if (closestObject != null)
        {
            if (!attacked)
            {
                
                var lookPos = closestObject.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 50);
                Fire();
                destroyableObjectTimer += Time.deltaTime;
                if (destroyableObjectTimer > destroyableShootTime)
                {
                    destroyableObjectTimer = 0;
                    closestObject.GetComponent<Destroyable>().health -= power;
                    if (closestObject.GetComponent<Destroyable>().health <= 0)
                    {
                        if (closestObject.GetComponent<Destroyable>().isHome)
                        {
                            NewDummy(); NewDummy(); NewDummy(); NewDummy(); NewDummy();
                        }
                        else if (closestObject.GetComponent<Destroyable>().isCar)
                        {
                            NewDummy(); NewDummy(); NewDummy();
                        }
                        else
                        {
                            NewDummy();
                        }
                        closestObject.GetComponent<Destroyable>().Destroyed();
                        shootingTarget = false;
                        movingTarget = true;
                        anim.SetBool("run", true);
                        anim.SetBool("shoot", false);
                        anim.SetBool("stick", false);
                        AnimatorType();
                    }
                }
             
            }
            else
            {
                if (player != null)
                {
                    var lookPos = player.transform.position - transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 50);
                    Fire();
                    destroyableObjectTimer += Time.deltaTime;
                    if (destroyableObjectTimer > 1)
                    {
                        destroyableObjectTimer = 0;

                    }
                }
              
            }


        }
        else
        {
            //    if (player != null)
            //    {
            //        var lookPos = player.transform.position - transform.position;
            //        lookPos.y = 0;
            //        var rotation = Quaternion.LookRotation(lookPos);
            //        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 50);
            //        Fire();
            //        destroyableObjectTimer += Time.deltaTime;
            //        if (destroyableObjectTimer > 1)
            //        {
            //            destroyableObjectTimer = 0;

            //        }
            //}
            shootingTarget = false;
            
        }
    }
    void Fire()
    {
        if (currentWeapon.isStick)
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
            if (!currentWeapon.isStick)
            {
                bulletMuzzle.Play();
            }
            fireTimer = 0;
            bulletParentTransform.GetChild(bulletNumber).transform.localPosition = Vector3.zero;
            bulletParentTransform.GetChild(bulletNumber).gameObject.SetActive(true);
            bulletParentTransform.GetChild(bulletNumber).GetComponent<Bullet>().BulletDisactive();
            if (!attacked && closestObject!=null)
            {
                bulletParentTransform.GetChild(bulletNumber).DOMove(closestObject.position, 0.2f).SetEase(Ease.Linear);
            }
            if(attacked && player!=null)
            {
                bulletParentTransform.GetChild(bulletNumber).DOMove(player.transform.position, 0.2f).SetEase(Ease.Linear);
                player.health -= power;
                player.healthBar.fillAmount = player.health / player.maxHealth;
                player.regenerateTimer = 0;
               
            }
            
            bulletNumber += 1;
            if (bulletNumber >= bulletParentTransform.childCount)
            {
                bulletNumber = 0;
            }
        }
        
    }
    void NewDummy()
    {
        var newDummy = Instantiate(dummy, new Vector3(closestObject.position.x, 0, closestObject.position.z), Quaternion.identity);
        newDummy.transform.parent = dummyParent;
        newDummy.GetComponent<EnemyDummy>().leader = transform.GetComponent<Ai>();
        switch (color)
        {
            case Colors.Red:
                for (int i = 0; i < newDummy.GetComponent<EnemyDummy>().meshes.Count; i++)
                {
                    newDummy.GetComponent<EnemyDummy>().meshes[i].material = newDummy.GetComponent<EnemyDummy>().mats[0];
                }
                break;
            case Colors.Green:
                for (int i = 0; i < newDummy.GetComponent<EnemyDummy>().meshes.Count; i++)
                {
                    newDummy.GetComponent<EnemyDummy>().meshes[i].material = newDummy.GetComponent<EnemyDummy>().mats[1];
                }
                break;
            case Colors.Yellow:
                for (int i = 0; i < newDummy.GetComponent<EnemyDummy>().meshes.Count; i++)
                {
                    newDummy.GetComponent<EnemyDummy>().meshes[i].material = newDummy.GetComponent<EnemyDummy>().mats[2];
                }
                break;
        }
        
        dummyCount += 1;
        power = dummyCount + 1;
        countText.text = power.ToString();
        maxHealth = dummyCount * 10 + 50;
        health = maxHealth;
        healthBar.fillAmount = health / maxHealth;
    }

    public void AnimatorType()
    {
        switch (anim.updateMode)
        {
            case AnimatorUpdateMode.Normal:
                anim.updateMode = AnimatorUpdateMode.UnscaledTime;
                break;
            case AnimatorUpdateMode.UnscaledTime:
                anim.updateMode = AnimatorUpdateMode.Normal;
                break;
           
        }
       
    }
}
