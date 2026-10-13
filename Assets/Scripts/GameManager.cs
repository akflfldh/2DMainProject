using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{



    public int totalPoint;
    public int stagePoint;
    public int stageIndex;

    public int health = 0;

    public PlayerMove player;

    public GameObject[] stages;

    public UIDocument uiDocument;

    private Label stageLabel;
    private Image[] healthImage;

    private Button restartButton;

    private Label pointLabel;

    private Label ClearLabel;

    void Start()
    {

        for (int i = 0; i < stages.Length; ++i)
        {
            stages[i].SetActive(false);

        }

        stages[stageIndex].SetActive(true);


        stageLabel = uiDocument.rootVisualElement.Q<Label>("StageLabel");

        healthImage = uiDocument.rootVisualElement.Query<Image>("Health").ToList().ToArray();

        restartButton = uiDocument.rootVisualElement.Q<Button>("RetryButton");

        pointLabel = uiDocument.rootVisualElement.Q<Label>("PointLabel");

        ClearLabel = uiDocument.rootVisualElement.Q<Label>("ClearLabel");


        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += Restart;

        ClearLabel.style.display = DisplayStyle.None;

    }

    // Update is called once per frame
    void Update()
    {

        pointLabel.text = (stagePoint + totalPoint).ToString();

    }

    public void NextStage()
    {
        totalPoint += stagePoint;
        stagePoint = 0;

        if (stageIndex < stages.Length - 1)
        {

            stages[stageIndex].SetActive(false);
            stageIndex += 1;
            stages[stageIndex].SetActive(true);

    

            PlayerReposition();
            stageLabel.text = "STAGE : " + stageIndex+1;
        }
        else
        {
            //게임 클리어
            Time.timeScale = 0;//시간멈춤
            ClearLabel.style.display = DisplayStyle.Flex;
        }
    }

    public void HealthDown()
    {
        health--;
        if (health > 0)
        {

            healthImage[health].tintColor = new Color(1, 0, 0, 0.4f);
            
        }
        else
        {
            healthImage[health].tintColor = new Color(1, 0, 0, 0.4f);
            player.OnDie();

            restartButton.style.display = DisplayStyle.Flex;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {

            if (health > 1)
            {

                PlayerReposition();
            }

            HealthDown();

        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 3.0f, -1);
        player.VelocityZero();
    }



    public void Restart()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);

    }
}
