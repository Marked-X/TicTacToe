using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour
{
    [SerializeField]
    private GameObject cross = null;
    [SerializeField]
    private GameObject circle = null;

    public int id;

    public bool IsEmpty { get; set; } = true;
    public bool IsCross { get; set; }

    public void WasClicked()
    {
        if (!IsEmpty || GameManager.Instance.IsGameOver || GameManager.Instance.IsTakingTurn)
            return;
        IsEmpty = false;

        if (GameManager.Instance.FirstPlayerTurn)
        {
            IsCross = true;
            cross.SetActive(true);
        }
        else
        {
            IsCross = false;
            circle.SetActive(true);
        }

        GameManager.Instance.BoxClicked(id);
    }
}
