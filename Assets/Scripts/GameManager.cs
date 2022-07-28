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
    private GameObject GameModeDropdown = null;
    [SerializeField]
    private GameObject GameSizeSlider = null;
    [SerializeField]
    private GameObject GameSizeLabel = null;

    public bool FirstPlayerTurn { get; set; } = true;

    private int fieldSize = 3;
    private GameObject[] field;
    private HashSet<HashSet<int>> winningSets = null;
    private HashSet<int> firstPlayerSet = null;
    private HashSet<int> secondPlayerSet = null;


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
        NewGame();
    }

    public void SliderValueChange()
    {
        GameSizeLabel.GetComponent<TextMeshProUGUI>().text = "Game Size : " + GameSizeSlider.GetComponent<Slider>().value;
    }

    public void NewGame()
    {
        foreach (Transform box in fieldObject.transform)
        {
            Destroy(box.gameObject);
        }

        GridLayoutGroup fieldLayout = fieldObject.GetComponent<GridLayoutGroup>();
        fieldSize = (int)GameSizeSlider.GetComponent<Slider>().value;

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

        field = new GameObject[fieldSize * fieldSize];

        for (int i = 0; i < fieldSize * fieldSize; i++)
        {
            field[i] = Instantiate(box, fieldObject.transform);
            field[i].GetComponent<Box>().id = i;
        }

        FirstPlayerTurn = true;
        firstPlayerSet = new HashSet<int>();
        secondPlayerSet = new HashSet<int>();
    }

    public void BoxClicked(int id)
    {
        if (FirstPlayerTurn)
        {
            firstPlayerSet.Add(id);
        }
        else
        {
            secondPlayerSet.Add(id);
        }

        if (WinCheck())
        {
            //victory stuff
            string temp = FirstPlayerTurn ? "First" : "Second";
            Debug.Log("Victory " + temp);
            return;
        }
        else
        {
            FirstPlayerTurn = !FirstPlayerTurn;
        }
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
}
