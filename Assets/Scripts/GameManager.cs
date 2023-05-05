using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;
public class GameManager : MonoBehaviour
{
    public GameObject winPanel, losePanel;
    public List<GameObject> boxes;
    float sortTimer;
    public Player player;

    public Ai enemy, enemy2, enemy3;
    public TextMeshProUGUI enemy1text, enemy2text,enemy3text;

    public TextMeshProUGUI myPowerText;

    public List<GameObject> Uis;
    public RectTransform pos1, pos2, pos3,pos4;
    public Counts uiCount1, uiCount2, uiCount3, uiCount4;

   
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineTransposer transposer;
    public float transposerValue;

    public TextMeshProUGUI timerText;
    public float minute, second;
    public bool timeIsOver;

    [HideInInspector]
    public bool enemy1isDead, enemy2isDead, enemy3isDead;

    public TextMeshProUGUI levelText;
    public int level;

    public AudioSource audioSource;
    public AudioClip[] voices;
    public RectTransform sortPanel;
    int isTutorialPlayed;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        BoxControl(0);
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        isTutorialPlayed = PlayerPrefs.GetInt("isTutorialPlayed");
        if (isTutorialPlayed != 0 && SceneManager.GetActiveScene().buildIndex==1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
         
        }
        level = PlayerPrefs.GetInt("level"); if (level == 0) { level = 1; PlayerPrefs.SetInt("level", level); }
        levelText.text = "LEVEL " + (level-1).ToString();
        isTutorialPlayed = 1;
        PlayerPrefs.SetInt("isTutorialPlayed", isTutorialPlayed);
       
        
    }

    private void Update()
    {
        sortTimer += Time.deltaTime;

        if (sortTimer > 0.2f)
        {
            sortTimer = 0;
            if (enemy != null)
            {
                enemy1text.text = enemy.power.ToString();
                uiCount1.count = enemy.power;
            }
            else
            {
                enemy1text.text = "0";
                uiCount1.count = 0;
            }
            if (enemy2 != null)
            {
                enemy2text.text = enemy2.power.ToString();
                uiCount2.count = enemy2.power;
            }
            else
            {
                enemy2text.text = "0";
                uiCount2.count = 0;
            }

            if (enemy3 != null)
            {
                enemy3text.text = enemy3.power.ToString();
                uiCount3.count = enemy3.power;
            }
            else
            {
                enemy3text.text = "0";
                uiCount3.count = 0;
            }

            if (player != null)
            {
                myPowerText.text = player.GetComponent<Player>().power.ToString();
                uiCount4.count = player.GetComponent<Player>().power;
            }
            else
            {
                myPowerText.text = "0";
                uiCount4.count = 0;
            }
           


            static int SortByCost(GameObject p1, GameObject p2)
            {
                return p1.GetComponent<Counts>().count.CompareTo(p2.GetComponent<Counts>().count);
            }
            Uis.Sort(SortByCost);
            Uis[3].GetComponent<RectTransform>().position = pos1.position;
            Uis[2].GetComponent<RectTransform>().position = pos2.position;
            Uis[1].GetComponent<RectTransform>().position = pos3.position;
            Uis[0].GetComponent<RectTransform>().position = pos4.position;
        }

        if (!timeIsOver)
        {
            second -= Time.deltaTime;
            if (second <= 0)
            {
                second = 60;
                minute -= 1;
            }
            if (second > 10)
            {
                timerText.text = minute.ToString() + ":" + (int)second;
            }
            else
            {
                timerText.text = minute.ToString() + ":0" + (int)second;
            }

            if(minute==0 && second < 10)
            {
                timerText.color = Color.red;
                timerText.fontSize = 110;
            }
        }
       
        if(minute<0 && !timeIsOver)
        {
            timeIsOver = true;
            player.FinishSound();
            timerText.text = "0:00";
            if (enemy == null)
            {
                enemy1isDead = true;
            }
            else
            {
                enemy.enabled = false;
                if (player.power > enemy.power)
                {
                    enemy1isDead = true;
                   
                }
            }
            if (enemy2 == null)
            {
                enemy2isDead = true;
            }
            else
            {
                enemy2.enabled = false;
                if (player.power > enemy2.power)
                {
                    enemy2isDead = true;
                   
                }
            }
            if (enemy3 == null)
            {
                enemy3isDead = true;
            }
            else
            {
                enemy3.enabled = false;
                if (player.power > enemy3.power)
                {
                    enemy3isDead = true;
                   
                }
            }
            if (player != null)
            {
                player.enabled = false;
            }

            if(enemy1isDead && enemy2isDead && enemy3isDead)
            {
                winPanel.SetActive(true);
                SetSortPanelLocation();
            }
            else
            {
                losePanel.SetActive(true);
                SetSortPanelLocation();
            }

        }

    }

    public void SetSortPanelLocation()
    {
        sortPanel.localScale = new Vector3(2, 2, 1);
        SetLeft(-750);
        SetRight(750);
        SetTop(1870);
        SetBottom(-1870);
    }
    public void SetLeft(float left)
    {
        sortPanel.offsetMin = new Vector2(left, sortPanel.offsetMin.y);
    }
    public  void SetRight( float right)
    {
        sortPanel.offsetMax = new Vector2(-right, sortPanel.offsetMax.y);
    }

    public  void SetTop( float top)
    {
        sortPanel.offsetMax = new Vector2(sortPanel.offsetMax.x, -top);
    }

    public  void SetBottom( float bottom)
    {
        sortPanel.offsetMin = new Vector2(sortPanel.offsetMin.x, bottom);
    }
    public void BoxControl(int level)
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].gameObject.SetActive(false);
        }
        boxes[level].gameObject.SetActive(true);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLevel()
    {
        level += 1; PlayerPrefs.SetInt("level", level);
        if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings-1)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
      
    }
    public void TransposerChange()
    {
        transposer.m_FollowOffset = new Vector3(0, transposerValue, -transposerValue);
    }
   
       
}
