using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance = null;

    [SerializeField]
    private GameObject box = null;
    [SerializeField]
    private GameObject fieldObject = null;
    [SerializeField]
    private GameObject gameModeDropdown = null;
    [SerializeField]
    private GameObject gameSizeSlider = null;
    [SerializeField]
    private GameObject gameSizeLabel = null;
    [SerializeField]
    private GameObject turnLabel = null;
    [SerializeField]
    private GameObject victoryLabel = null;
    [SerializeField]
    private GameObject computerTurn = null;
    [SerializeField]
    private GameObject computerTurnToggle = null;

    public bool FirstPlayerTurn { get; private set; }
    public bool IsTakingTurn { get; set; } = false;
    public bool IsGameOver { get; set; } = true;
    public int GameMode { get; set; } = 0;

    public int fieldSize = 3;
    public HashSet<HashSet<int>> winningSets = null;

    private GameObject[] field;
    private HashSet<int> firstPlayerSet = null;
    private HashSet<int> secondPlayerSet = null;
    private Computer computerFirstPlayer = null;
    private Computer computerSecondPlayer = null;


    private HashSet<HashSet<int>> winningSetsSizeThree = new HashSet<HashSet<int>>()
    {
        new HashSet<int> { 0, 1, 2},
        new HashSet<int> { 3, 4, 5},
        new HashSet<int> { 6, 7, 8},
        new HashSet<int> { 0, 3, 6},
        new HashSet<int> { 1, 4, 7},
        new HashSet<int> { 2, 5, 8},
        new HashSet<int> { 0, 4, 8},
        new HashSet<int> { 2, 4, 6}
    };
    private HashSet<HashSet<int>> winningSetsSizeFour = new HashSet<HashSet<int>>()
    {
        new HashSet<int> { 0, 1, 2, 3},
        new HashSet<int> { 4, 5, 6, 7},
        new HashSet<int> { 8, 9, 10, 11},
        new HashSet<int> { 12, 13, 14, 15},
        new HashSet<int> { 0, 4, 8, 12},
        new HashSet<int> { 1, 5, 9, 13},
        new HashSet<int> { 2, 6, 10, 14},
        new HashSet<int> { 3, 7, 11, 15},
        new HashSet<int> { 0, 5, 10, 15},
        new HashSet<int> { 3, 6, 9, 12}
    };
    private HashSet<HashSet<int>> winningSetsSizeFive = new HashSet<HashSet<int>>()
    {
        new HashSet<int> { 0, 1, 2, 3, 4},
        new HashSet<int> { 5, 6, 7, 8, 9},
        new HashSet<int> { 10, 11, 12, 13, 14},
        new HashSet<int> { 15, 16, 17, 18, 19},
        new HashSet<int> { 20, 21, 22, 23, 24},
        new HashSet<int> { 0, 5, 10, 15, 20},
        new HashSet<int> { 1, 6, 11, 16, 21},
        new HashSet<int> { 2, 7, 12, 17, 22},
        new HashSet<int> { 3, 8, 13, 18, 23},
        new HashSet<int> { 4, 9, 14, 19, 24},
        new HashSet<int> { 0, 6, 12, 18, 24},
        new HashSet<int> { 4, 8, 12, 16, 20}
    };


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        computerSecondPlayer = new Computer();
        NewGame();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void SliderValueChange()
    {
        gameSizeLabel.GetComponent<TextMeshProUGUI>().text = "Game Size : " + gameSizeSlider.GetComponent<Slider>().value;
    }

    public void NewGame()
    {
        foreach (Transform box in fieldObject.transform)
        {
            Destroy(box.gameObject);
        }

        GridLayoutGroup fieldLayout = fieldObject.GetComponent<GridLayoutGroup>();
        fieldSize = (int)gameSizeSlider.GetComponent<Slider>().value;      

        fieldLayout.constraintCount = fieldSize;
        switch (fieldSize)
        {
            case 3:
                fieldLayout.padding.left = fieldLayout.padding.top = -100;
                winningSets = winningSetsSizeThree;
                break;
            case 4:
                fieldLayout.padding.left = fieldLayout.padding.top = -150;
                winningSets = winningSetsSizeFour;
                break;
            case 5:
                fieldLayout.padding.left = fieldLayout.padding.top = -200;
                winningSets = winningSetsSizeFive;
                break;
        }

        switch (gameModeDropdown.GetComponent<TMP_Dropdown>().value)
        {
            case 0:
                computerFirstPlayer = null;
                computerSecondPlayer = null;
                GameMode = 0;
                break;
            case 1:
                if (computerTurnToggle.GetComponent<Toggle>().isOn)
                {
                    computerSecondPlayer = new Computer();
                    computerSecondPlayer.Initialize(winningSets);
                    computerFirstPlayer = null;
                }
                else
                {
                    computerFirstPlayer = new Computer();
                    computerFirstPlayer.Initialize(winningSets);
                    computerSecondPlayer = null;
                }
                GameMode = 1;
                break;
            case 2:
                computerFirstPlayer = new Computer();
                computerSecondPlayer = new Computer();
                computerFirstPlayer.Initialize(winningSets);
                computerSecondPlayer.Initialize(winningSets);
                GameMode = 2;
                break;
        }

        field = new GameObject[fieldSize * fieldSize];

        for (int i = 0; i < fieldSize * fieldSize; i++)
        {
            field[i] = Instantiate(box, fieldObject.transform);
            field[i].GetComponent<Box>().id = i;
        }

        IsGameOver = false;
        FirstPlayerTurn = true;
        turnLabel.GetComponent<TextMeshProUGUI>().text = "First Player Turn";
        victoryLabel.SetActive(false);
        firstPlayerSet = new HashSet<int>();
        secondPlayerSet = new HashSet<int>();

        SetAllSets();

        if (GameMode == 2 || (GameMode == 1 && computerFirstPlayer != null))
        {
            StartCoroutine(computerFirstPlayer.TakeATurn());
        }
    }

    public void BoxClicked(int id)
    {
        if (IsGameOver || IsTakingTurn)
            return;

        if (FirstPlayerTurn)
        {
            firstPlayerSet.Add(id);
            if (computerSecondPlayer != null)
                computerSecondPlayer.UpdatePotentialSets(id);
        }
        else
        {
            secondPlayerSet.Add(id);
            if (computerFirstPlayer != null)
                computerFirstPlayer.UpdatePotentialSets(id);
        }

        if (WinCheck())
        {
            string temp = FirstPlayerTurn ? "First" : "Second";
            Victory(temp);
            return;
        }
        else if (!EmptySpaceCheck())
        {
            Draw();
            return;
        }

        NextTurn();
    }

    private void NextTurn()
    {
        FirstPlayerTurn = !FirstPlayerTurn;

        if (FirstPlayerTurn)
            turnLabel.GetComponent<TextMeshProUGUI>().text = "First Player Turn";
        else
            turnLabel.GetComponent<TextMeshProUGUI>().text = "Second Player Turn";

        if (GameMode != 0)
        {
            if (FirstPlayerTurn && computerFirstPlayer != null)
            {
                IsTakingTurn = true;
                StartCoroutine(computerFirstPlayer.TakeATurn());
            }
            else if (!FirstPlayerTurn && computerSecondPlayer != null)
            {
                IsTakingTurn = true;
                StartCoroutine(computerSecondPlayer.TakeATurn());
            }
        }
    }

    private bool EmptySpaceCheck()
    {
        foreach(GameObject box in field)
        {
            if (box.GetComponent<Box>().IsEmpty)
                return true;
        }
        return false;
    }

    private bool WinCheck()
    {
        HashSet<int> temp;
        foreach (HashSet<int> winSet in winningSets)
        {
            temp = new HashSet<int>(winSet);
            
            if (FirstPlayerTurn)
                temp.IntersectWith(firstPlayerSet);
            else
                temp.IntersectWith(secondPlayerSet);

            if (temp.Count == fieldSize)
                return true;
        }
        return false;
    }

    private void Victory(string player)
    {
        IsGameOver = true;
        victoryLabel.GetComponentInChildren<TextMeshProUGUI>().text = player + " Player Won!";
        victoryLabel.SetActive(true);
    }

    private void Draw()
    {
        IsGameOver = true;
        victoryLabel.GetComponentInChildren<TextMeshProUGUI>().text = "Draw!";
        victoryLabel.SetActive(true);
    }

    public void DropdownValueChange()
    {
        if (gameModeDropdown.GetComponent<TMP_Dropdown>().value == 1)
            computerTurn.SetActive(true);
        else
            computerTurn.SetActive(false);
    }

    public void SetAllSets()
    {
        if (computerFirstPlayer != null)
        {
            computerFirstPlayer.ownSet = firstPlayerSet;
            computerFirstPlayer.opponentsSet = secondPlayerSet;
        }
        if (computerSecondPlayer != null)
        {
            computerSecondPlayer.ownSet = secondPlayerSet;
            computerSecondPlayer.opponentsSet = firstPlayerSet;
        }
    }

    public void ClickBox(int id)
    {
        field[id].GetComponent<Box>().WasClicked();
    }
}
