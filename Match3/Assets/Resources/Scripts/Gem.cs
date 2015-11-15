using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gem : MonoBehaviour
{
    public Renderer modelRenderer;
    public GameObject selector;
    public ParticleSystem selectorParticle;

    public int x;
    public int y;

    public int gemType;

    private bool m_isSelected = false;

    void Start()
    {
        CreateGem();
    }

    public void CreateGem()
    {
        gemType = Random.Range(0, Data.GEMTYPE.Length);
        string color = Data.GEMTYPE[gemType];
        Material m = Resources.Load<Material>("Materials/" + color);

        modelRenderer.material = m;
        selectorParticle.startColor = m.color;
    }

    public void ToggleSelector()
    {
        m_isSelected = !m_isSelected;
        selector.SetActive(m_isSelected);
    }

    public bool IsAdjacent(Gem otherGem)
    {
        if (otherGem != null)
        {
            if (this.x == otherGem.x)
            {
                if (this.y == otherGem.y + 1 || this.y == otherGem.y - 1)
                    return true;
            }

            if (this.y == otherGem.y)
            {
                if (this.x == otherGem.x + 1 || this.x == otherGem.x - 1)
                    return true;
            }
        }
        return false;
    }

    void OnMouseDown()
    {
        GameObject.Find("Board").GetComponent<Board>().SwapGems(this);
    }
}
