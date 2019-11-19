using UnityEngine;

public class GameManager : GenericSingletonClass<GameManager>
{
    [SerializeField]
    UIController _UIController;
    [SerializeField]
    PacmanController _pacman;

    [SerializeField]
    GhostAI _blinky;

    public GhostAI[] ghosts;

    int _score = 0;

    public PacmanController Pacman { get => _pacman; private set => _pacman = value; }
    public GhostAI Blinky { get => _blinky; private set => _blinky = value; }

    public int pacdotCount;
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
        pacdotCount++;
        CheckWin();
        _score += score;
        _UIController.SetScore(_score);
        
    }

    public void GotEnergizer()
    {
        foreach (var ghost in ghosts)
        {
            ghost.SetFrightened();
        }
    }

    private void CheckWin()
    {
        if(pacdotCount >= 333)
        {
            print("win");
        }
    }

}
