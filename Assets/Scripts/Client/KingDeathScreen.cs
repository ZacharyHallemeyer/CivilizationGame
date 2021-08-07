using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KingDeathScreen : MonoBehaviour
{
    public GameObject mainContainer, backGround, kingDeathText, endTurnButton;

    public void EndTurn()
    {
        PlayerCS.instance.EndTurn();
        PlayerCS.instance.enabled = true;
        PlayerCS.instance.isAbleToCommitActions = false;
        mainContainer.SetActive(false);
    }
}
