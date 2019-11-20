using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    Text _scoreText, _highScoreText;
    [SerializeField]
    Transform _livesContainer;
    [SerializeField]
    GameObject _ready;

    [SerializeField]
    RectTransform worldCanvas;
    [SerializeField]
    Text ghostScore;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SetScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }
    public void SetHighScore(int score)
    {
        _highScoreText.text = "Highscore: " + score;
    }

    public void SetLives(int lives)
    {
        for (int i = 0; i < _livesContainer.childCount; i++)
        {
            _livesContainer.GetChild(i).gameObject.SetActive(i < lives);
        }
    }

    public void SetReadyPanel(bool show)
    {
        _ready.SetActive(show);
    }
    public void SetGhostCanvasScore(Vector2 position, int score)
    {
        ShowGhostScore(position, score);
        StartCoroutine(UnityUtils.DelayedAction(2, () => HideGhostScore()));
    }

    void ShowGhostScore(Vector2 position, int score)
    {
        worldCanvas.anchoredPosition = position;
        ghostScore.text = score.ToString();
        worldCanvas.gameObject.SetActive(true);

    }

    void HideGhostScore()
    {
        worldCanvas.gameObject.SetActive(false);

    }
}
