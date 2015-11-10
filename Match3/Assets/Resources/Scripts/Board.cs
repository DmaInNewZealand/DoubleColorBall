using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;

    public GameObject gemPrefab;

    Gem[,] m_gems;
    Gem m_currentGem;

    void Awake()
    {
        m_gems = new Gem[gridWidth, gridHeight];
    }

    void Start()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject obj = Instantiate(gemPrefab, new Vector3(-gridWidth / 2f + x + 0.5f, -gridHeight / 2f + y + 0.5f), Quaternion.identity) as GameObject;
                obj.transform.SetParent(transform);
                m_gems[x, y] = obj.GetComponent<Gem>();
                m_gems[x, y].x = x;
                m_gems[x, y].y = y;
            }
        }
    }

    public void SwapGems(Gem selectedGem)
    {
        if(m_currentGem == null)
        {
            m_currentGem = selectedGem;
        }
        else if (m_currentGem == selectedGem)
        {
            m_currentGem = null;
        }
        else if(!m_currentGem.IsAdjacent(selectedGem))
        {
            m_currentGem.ToggleSelector();
            m_currentGem = selectedGem;
        }
        else
        {
            m_currentGem.ToggleSelector();
            selectedGem.ToggleSelector();
            m_currentGem = null;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
