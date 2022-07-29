using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer
{
    public HashSet<int> ownSet = null;
    public HashSet<int> opponentsSet = null;

    private HashSet<int> targetSet = null;
    private Dictionary<int, HashSet<int>> potentialSets = null;

    public void Initialize(HashSet<HashSet<int>> winningSets)
    {
        targetSet = null;
        potentialSets = new Dictionary<int, HashSet<int>>();
        int i = 0;

        foreach(HashSet<int> set in winningSets)
        {
            potentialSets.Add(i, set);
            i++;
        }
    }

    public void UpdatePotentialSets(int id)
    {
        if (potentialSets.Count <= 0)
            return;

        List<int> keysForRemoval = new List<int>();

        foreach(KeyValuePair<int, HashSet<int>> set in potentialSets)
        {
            if (set.Value.Contains(id))
                keysForRemoval.Add(set.Key);
        }

        foreach(int key in keysForRemoval)
        {
            potentialSets.Remove(key);
        }
    }

    public IEnumerator TakeATurn()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.IsTakingTurn = false;

        //pick a target set if there is none or chosen target set is no longer viable and there is at least one potential set
        if (targetSet == null || !CheckIsTargetSetValiable())
        {
            PickTargetSet();
        }

        //check if there is a winning move
        if (CheckIsWinPossible())
        {
            HashSet<int> temp = new HashSet<int>(targetSet);
            temp.ExceptWith(ownSet);
            if (temp.Count == 1)
            {
                foreach (int id in temp)
                    GameManager.Instance.ClickBox(id);
            }
            else
            {
                Debug.LogError("More than one Winning Id");
            }
            yield break;
        }

        //check if opposing player has a winning move if yes block it
        int idToBlock = GetOpponentsWinningId();
        if (idToBlock >= 0)
        {
            GameManager.Instance.ClickBox(idToBlock);
            yield break;
        }

        //make a random move out of targeted set
        if (targetSet != null)
            MakeRandomTargetedMove();
        else
            MakeRandomMove();
    }

    private void MakeRandomMove()
    {
        HashSet<int> possibleMoves = new HashSet<int>();
        for (int i = 0; i < GameManager.Instance.fieldSize * GameManager.Instance.fieldSize; i++)
        {
            possibleMoves.Add(i);
        }
        possibleMoves.ExceptWith(ownSet);
        possibleMoves.ExceptWith(opponentsSet);

        int idToPick = Random.Range(0, possibleMoves.Count);
        int j = 0;
        foreach (int id in possibleMoves)
        {
            if (j == idToPick)
            {
                GameManager.Instance.ClickBox(id);
                return;
            }
            j++;
        }
    }

    private void MakeRandomTargetedMove()
    {
        HashSet<int> setOfChoices = new HashSet<int>(targetSet);
        setOfChoices.ExceptWith(ownSet);

        int idToPick = Random.Range(0, setOfChoices.Count);
        int i = 0;
        foreach (int id in setOfChoices)
        {
            if (i == idToPick)
            {
                GameManager.Instance.ClickBox(id);
                return;
            }
            i++;
        }
    }

    private bool CheckIsTargetSetValiable()
    {
        HashSet<int> temp = new HashSet<int>(opponentsSet);
        temp.IntersectWith(targetSet);
        if (temp.Count > 0)
            return false;
        else 
            return true;
    }

    private void PickTargetSet()
    {
        if(potentialSets.Count == 0)
        {
            targetSet = null;
            return;
        }

        //picks the most complete set, if all sets are at 0 completion picks a random one
        int biggestIntersection = 0;
        int targetKey = 0;
        foreach (KeyValuePair<int, HashSet<int>> set in potentialSets)
        {
            HashSet<int> temp = new HashSet<int>(ownSet);
            temp.IntersectWith(set.Value);
            if (temp.Count > biggestIntersection)
            {
                biggestIntersection = temp.Count;
                targetKey = set.Key;
            }
        }
        if (biggestIntersection > 0)
        {
            targetSet = potentialSets[targetKey];
        }
        else
        {
            int target = Random.Range(0, potentialSets.Count);
            foreach (KeyValuePair<int, HashSet<int>> set in potentialSets)
            {
                if (target == 0)
                {
                    targetSet = set.Value;
                    return;
                }
                target--;
            }
        }     
    }

    private bool CheckIsWinPossible()
    {
        if (targetSet == null)
            return false;

        HashSet<int> temp = new HashSet<int>(targetSet);
        temp.IntersectWith(ownSet);

        if (temp.Count == GameManager.Instance.fieldSize - 1)
            return true;
        else
            return false;
    }

    private int GetOpponentsWinningId()
    {
        HashSet<int> temp;
        foreach(HashSet<int> set in GameManager.Instance.winningSets)
        {
            temp = new HashSet<int>(set);
            temp.IntersectWith(ownSet);
            if (temp.Count > 0)
                continue;

            temp = new HashSet<int>(set);
            temp.ExceptWith(opponentsSet);
            if (temp.Count == 1)
            {
                foreach (int id in temp)
                    return id;
            }
        }
        return -1;
    }
}
