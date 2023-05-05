using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
public class Player : MonoBehaviour
{
    [Header("Movement")]
    Rigidbody rb;
    public float moveSpeed;
    public DynamicJoystick js;
    Animator anim;
    public bool moving;

    

    public Transform nodeParent;
    public GameObject dummy;

    public GameManager gm;
    public Transform rangeObject;
    public float range;
    [HideInInspector] public Transform closestObject;
    [HideInInspector] public bool fighting;
    [HideInInspector] public float fightingTime;


    public Transform bulletParentTransform;
    public float fireRate;
    [HideInInspector] public float fireTimer;
    [HideInInspector] public int bulletNumber;
    public ParticleSystem bulletMuzzle;

    public Transform dummyParent;
    [HideInInspector] public int dummyCount;
    [HideInInspector] public int power;
    [HideInInspector] public float destroyableObjectTimer;

    public Transform closestEnemy;
    public bool isEnemyInRange;
     public Transform attackingEnemy;

    [HideInInspector] public float maxHealth;
     public float health;

    public Image healthBar;
    float healthTimer;
    public TextMeshProUGUI countText;
    [HideInInspector] public float regenerateTimer;

    public bool isDead;

    public List<GameObject> weapons;
    public Weapon currentWeapon;
    public int gunLevel;
    public List<Transform> startPosList;


    public float hitSoundTimer;
   
    void Start()
    {
         Physics.reuseCollisionCallbacks = false;

        transform.position = startPosList[Random.Range(0, startPosList.Count)].position;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        dummyCount = dummyParent.childCount;
        power = dummyCount + 1;
        countText.text = power.ToString();

        maxHealth = power * 50;
        health = maxHealth;
        healthBar.fillAmount = health / maxHealth;

        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].SetActive(false);
        }
        weapons[0].SetActive(true);
        currentWeapon = weapons[0].GetComponent<Weapon>();
        range = currentWeapon.range;
        rangeObject.transform.localScale = new Vector3(range * 2, 0.01f, range * 2);
        gm.transposerValue = gunLevel * 2 + 10;
        DOTween.To(() => gm.transposer.m_FollowOffset, x => gm.transposer.m_FollowOffset = x, new Vector3(0, gm.transposerValue, -gm.transposerValue), 1);
        StartSound();
    }

  
    void FixedUpdate()
    {
       

        RegenerateHealth();
        Movement();
        ClosestTarget();
        ClosestEnemy();
        FightingControl();


        if (health <= 0 && !isDead)
        {
            isDead = true;
            FinishSound();
            gm.losePanel.SetActive(true);
            gm.SetSortPanelLocation();
            gm.timeIsOver = true;
            Destroy(gameObject);
        }

        
        if (Input.GetKeyDown(KeyCode.K))
        {
            NewDummy();
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
     void Movement()
    {

        if (moving)
        {
            rb.velocity = new Vector3(js.Horizontal * moveSpeed, rb.velocity.y, js.Vertical * moveSpeed);
        }
        if (js.Horizontal != 0 || js.Vertical != 0)
        {
            Running();
            if (!moving)
            {
                moving = true;
                rb.isKinematic = false;
            }
           
        }
        else
        {
            if (js.Horizontal == 0 && js.Vertical == 0)
            {
                Waiting();
                if (moving)
                {
                    moving = false;
                    rb.isKinematic = true;
                }
               
            }
        }
        if (fighting && !moving)
        {
            Shooting();
            if (moving)
            {
                moving = false;
                rb.isKinematic = true;
            }
      
        }

       
       
       
    }
     void Running()
    {
        if (rb.velocity != Vector3.zero)
        {
            anim.SetBool("run", true);
            anim.SetBool("idle", false);
            anim.SetBool("shoot", false);
            anim.SetBool("stick", false);
            transform.rotation = Quaternion.LookRotation(rb.velocity);
            if (fighting)
            {
                for (int i = 0; i < bulletParentTransform.childCount; i++)
                {
                    bulletParentTransform.GetChild(i).transform.localPosition = Vector3.zero;
                }
                fighting = false;
                bulletParentTransform.gameObject.SetActive(false);
            }
            if (bulletParentTransform.gameObject.activeInHierarchy)
            {
                bulletParentTransform.gameObject.SetActive(false);
            }
            rb.isKinematic = false;
        }
    }
     void NewDummy()
    {
        var newDummy = Instantiate(dummy, new Vector3(closestObject.position.x, 0, closestObject.position.z), Quaternion.identity);
        newDummy.transform.parent = dummyParent;
        dummyCount += 1;
        power = dummyCount + 1;
        countText.text = power.ToString();
        maxHealth = dummyCount * 10 + 50;
        health = maxHealth;
        healthBar.fillAmount = health / maxHealth;
    }
     void Shooting()
    {   //ShootingDestroyable
        if (closestObject != null && !isEnemyInRange)
        {
            anim.SetBool("run", false);
            anim.SetBool("idle", false);
            if (currentWeapon.isStick)
            {
                anim.SetBool("stick", true);
            }
            else
            {
                anim.SetBool("shoot", true);
            }
            
            var lookPos = closestObject.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 50);
            Fire();

            destroyableObjectTimer += Time.deltaTime;
            if (destroyableObjectTimer > 0.5f)
            {
                destroyableObjectTimer = 0;
                closestObject.GetComponent<Destroyable>().health -= power;
                if (closestObject.GetComponent<Destroyable>().health > 0)
                {
                    closestObject.GetComponent<Destroyable>().Hit();
                }

                if (closestObject.GetComponent<Destroyable>().health <= 0)
                {
                    if (closestObject.GetComponent<Destroyable>().isHome)
                    {
                        NewDummy(); NewDummy(); NewDummy(); NewDummy(); NewDummy();
                        HomeSound();
                    }
                    else if (closestObject.GetComponent<Destroyable>().isCar)
                    {
                        NewDummy(); NewDummy(); NewDummy();
                        CarSound();
                    }
                    else
                    {
                        NewDummy();
                        PropSound();
                    }
                   
                  
                    closestObject.GetComponent<Destroyable>().Destroyed();
                }

            }
            fightingTime += Time.deltaTime;
            if (fightingTime > 0.5f)
            {
                fightingTime = 0;
                fighting = false;
            }
        }

        //ShootingEnemy
        if (closestEnemy != null && isEnemyInRange)
        {
            anim.SetBool("run", false);
            anim.SetBool("idle", false);
            if (currentWeapon.isStick)
            {
                anim.SetBool("stick", true);
            }
            else
            {
                anim.SetBool("shoot", true);
            }
            var lookPos = closestEnemy.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 50);
            Fire();

            destroyableObjectTimer += Time.deltaTime;
            if (destroyableObjectTimer > 1)
            {
                destroyableObjectTimer = 0;
              
            }
            fightingTime += Time.deltaTime;
            if (fightingTime > 1)
            {
                fightingTime = 0;
                fighting = false;
            }
        }

    }
     void Waiting()
    {
        anim.SetBool("run", false);
        anim.SetBool("idle", true);
        anim.SetBool("shoot", false);
        anim.SetBool("stick", false);
    }
     void FightingControl()
    {
        if (closestObject != null)
        {
            if (Vector3.Distance(transform.position, closestObject.position) < range )
            {
                if (!fighting)
                {
                    for (int i = 0; i < bulletParentTransform.childCount; i++)
                    {
                        bulletParentTransform.GetChild(i).transform.localPosition = Vector3.zero;
                    }
                    fireTimer = 0;
                    fighting = true;
                }
                fightingTime = 0;
            }
        }
        if (isEnemyInRange)
        {
            if (!fighting)
            {
                for (int i = 0; i < bulletParentTransform.childCount; i++)
                {
                    bulletParentTransform.GetChild(i).transform.localPosition = Vector3.zero;
                }
                fireTimer = 0;
                fighting = true;
            }
            fightingTime = 0;
        }

    }
     void Fire()
    {
        if (currentWeapon.isStick)
        {
            bulletParentTransform.gameObject.SetActive(false);
            hitSoundTimer += Time.deltaTime;
            if (hitSoundTimer > 0.6f)
            {
                StickSound();
                hitSoundTimer = 0;
            }
        }
        else
        {
            bulletParentTransform.gameObject.SetActive(true);
            hitSoundTimer += Time.deltaTime;
            if (hitSoundTimer > 1)
            {
                GunSound();
                hitSoundTimer = 0;
            }
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
            if (!isEnemyInRange)
            {
                bulletParentTransform.GetChild(bulletNumber).DOMove(closestObject.position, 0.2f).SetEase(Ease.Linear);
            }
            else
            {
                bulletParentTransform.GetChild(bulletNumber).DOMove(closestEnemy.position, 0.2f).SetEase(Ease.Linear);
                attackingEnemy = closestEnemy;
                var Enemy = attackingEnemy.GetComponent<Ai>();
                Enemy.attacked = true;
                Enemy.health -= power;
                Enemy.healthBar.fillAmount = Enemy.health / Enemy.maxHealth;
                Enemy.regenerateTimer = 0;
                if (Enemy.health <= 0)
                {
                    isEnemyInRange = false;
                }
            }
           
            bulletNumber += 1;
            if (bulletNumber >= bulletParentTransform.childCount)
            {
                bulletNumber = 0;
            }
        }
    }
     void ClosestTarget()
     {
            float distanceClosestTarget = Mathf.Infinity;
            Destroyable closestTarget = null;
            Destroyable[] allTargets = GameObject.FindObjectsOfType<Destroyable>();
            if (allTargets.Length != 0)
            {
            for (int i = 0; i < allTargets.Length; i++)
            {
                if (allTargets[i].GetComponent<Destroyable>().outlineMesh != null)
                {
                    allTargets[i].GetComponent<Destroyable>().outlineMesh.GetComponent<Outline>().enabled = false;
                }
                if (allTargets[i].GetComponent<Destroyable>().outlineMesh2 != null)
                {
                    allTargets[i].GetComponent<Destroyable>().outlineMesh.GetComponent<Outline>().enabled = false;
                }
                if (allTargets[i].GetComponent<Destroyable>().outlineMesh3 != null)
                {
                    allTargets[i].GetComponent<Destroyable>().outlineMesh.GetComponent<Outline>().enabled = false;
                }

            }
            foreach (Destroyable currentTarget in allTargets)
            {
                float distanceToTarget = (currentTarget.transform.position - this.transform.position).sqrMagnitude;
                if (distanceToTarget < distanceClosestTarget)
                {
                    distanceClosestTarget = distanceToTarget;
                    closestTarget = currentTarget;
                }
            }
            closestObject = closestTarget.transform;
            if (closestObject.GetComponent<Destroyable>().outlineMesh != null)
            {
                if (Vector3.Distance(transform.position, closestObject.position) < range)
                {
                    closestObject.GetComponent<Destroyable>().outlineMesh.GetComponent<Outline>().enabled = true;
                }
            }
            if (closestObject.GetComponent<Destroyable>().outlineMesh2 != null)
            {
                if (Vector3.Distance(transform.position, closestObject.position) < range)
                {
                    closestObject.GetComponent<Destroyable>().outlineMesh2.GetComponent<Outline>().enabled = true;
                }
            }
            if (closestObject.GetComponent<Destroyable>().outlineMesh3 != null)
            {
                if (Vector3.Distance(transform.position, closestObject.position) < range)
                {
                    closestObject.GetComponent<Destroyable>().outlineMesh3.GetComponent<Outline>().enabled = true;
                }
            }
        }
     }
     void ClosestEnemy()
    {
        float distanceClosestTarget = Mathf.Infinity;
        Ai closestTarget = null;
        Ai[] allTargets = FindObjectsOfType<Ai>();
        if (allTargets.Length != 0)
        {
            foreach (Ai currentTarget in allTargets)
            {
                float distanceToTarget = (currentTarget.transform.position - this.transform.position).sqrMagnitude;
                if (distanceToTarget < distanceClosestTarget)
                {
                    distanceClosestTarget = distanceToTarget;
                    closestTarget = currentTarget;
                }
            }
            closestEnemy = closestTarget.transform;
            if (Vector3.Distance(closestEnemy.position, transform.position) < range)
            {
               
                    isEnemyInRange = true;
                
               
            }
            else
            {
                if (attackingEnemy != null)
                {
                    attackingEnemy.GetComponent<Ai>().attacked = false;
                    attackingEnemy.GetComponent<Ai>().MoveClosestTarget();
                    attackingEnemy = null;
                }
                isEnemyInRange = false;
                
            }
        }
        else
        {
            isEnemyInRange = false;
            if (!gm.timeIsOver)
            {
                gm.timeIsOver = true;
                FinishSound();
                DOVirtual.DelayedCall(2f, () => {
                    gm.winPanel.SetActive(true);
                    gm.SetSortPanelLocation();
                    this.enabled = false;

                });
            }
          
            
        }
    }

   
    public void CapturedDummy(Vector3 pos)
    {
        var newDummy = Instantiate(dummy, pos, Quaternion.identity);
        newDummy.transform.parent = dummyParent;
        dummyCount += 1;
        power = dummyCount + 1;
        countText.text = power.ToString();
        maxHealth = dummyCount * 10 + 50;
        health = maxHealth;
        healthBar.fillAmount = health / maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<WeaponBox>())
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].SetActive(false);
            }
            gunLevel = other.GetComponent<WeaponBox>().gunLevel;
            weapons[other.GetComponent<WeaponBox>().gunLevel].SetActive(true);
            currentWeapon = weapons[other.GetComponent<WeaponBox>().gunLevel].GetComponent<Weapon>();
            range = currentWeapon.range;
            for (int i = 0; i < dummyParent.childCount; i++)
            {
                dummyParent.GetChild(i).GetComponent<Dummy>().RefreshGun();
            }
            if (gunLevel < weapons.Count-1)
            {
                gm.BoxControl(gunLevel);
            }
            gm.transposerValue = gunLevel*2.4f + 10;
            DOTween.To(() => gm.transposer.m_FollowOffset, x => gm.transposer.m_FollowOffset = x, new Vector3(0, gm.transposerValue, -gm.transposerValue), 1);
            Destroy(other.gameObject);
            rangeObject.transform.localScale = new Vector3(range * 2, 0.01f, range * 2);
        }
    }

    public void StickSound()
    {
        gm.audioSource.PlayOneShot(gm.voices[0]);
    }
    public void GunSound()
    {
        gm.audioSource.PlayOneShot(gm.voices[1]);
    }
    public void StartSound()
    {
        gm.audioSource.PlayOneShot(gm.voices[2]);
    }
    public void PropSound()
    {
        gm.audioSource.PlayOneShot(gm.voices[3]);
    }
    public void CarSound()
    {
        gm.audioSource.PlayOneShot(gm.voices[4]);
    }
    public void HomeSound()
    {
        gm.audioSource.PlayOneShot(gm.voices[5]);
    }
    public void FinishSound()
    {
        gm.audioSource.PlayOneShot(gm.voices[6]);
    }
}
