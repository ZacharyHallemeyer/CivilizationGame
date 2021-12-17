using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class is added to skill tree buttons in unity editor. This allows for each skill to have it's price displayed
/// </summary>
public class SkillToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string skillName;
    private int skillPrice;
    private bool isPurchased = false;

    private Vector3 anchoredPosition;

    private void Start()
    {
        anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    public void StartToolTip()
    {
        int _index;

        // Check if skill is already purchased
        if (!isPurchased)
        {
            // Check if skill has been purchased since the last time tool tip has been started
            for (_index = 0; _index < Constants.avaliableTroops.Count; _index++)
            {
                if (Constants.avaliableTroops[_index] == skillName)
                    isPurchased = true;
            }

            for (_index = 0; _index < Constants.avaliableBuildings.Count; _index++)
            {
                if (Constants.avaliableBuildings[_index] == skillName)
                    isPurchased = true;
            }

            if (!isPurchased)
            {
                // Get current skill price
                foreach (string _key in Constants.allSkills.Keys)
                {
                    if (_key == skillName)
                    {
                        skillPrice = Constants.allSkills[_key];
                    }
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isPurchased)
            PlayerCS.instance.playerUI.DisplayToolTip("Money ", skillPrice, anchoredPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isPurchased)
            PlayerCS.instance.playerUI.HideToolTip();
    }
}
