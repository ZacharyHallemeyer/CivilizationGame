using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string skillName;
    private int skillPrice;
    private bool mouseOver;

    private Vector3 anchoredPosition;

    private void Start()
    {
        anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    private void Update()
    {
        if(mouseOver)
        {
            PlayerCS.instance.playerUI.DisplayToolTip("Money", skillPrice, anchoredPosition);
        }
    }

    public void StartToolTip()
    {
        // Get current skill price
        foreach (string _key in Constants.allSkills.Keys)
        {
            if (_key == skillName)
            {
                skillPrice = Constants.allSkills[_key];
            }
        }

        // Enable update function
        enabled = true;
    }

    public void StopToolTip()
    {
        // Disable update function and set mouse over to false
        enabled = false;
        mouseOver = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerCS.instance.playerUI.DisplayToolTip("Money ", skillPrice, anchoredPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerCS.instance.playerUI.HideToolTip();
    }
}
