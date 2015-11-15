using UnityEngine;
using System.Collections;
using System;

public class Board : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;

    public GameObject gemPrefab;

    Gem[,] m_gems;
    Gem m_currentGem;

    private bool isSwapping = false;

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
        if(isSwapping)
        {
            return;
        }

        selectedGem.ToggleSelector();

        if (m_currentGem == null)
        {
            m_currentGem = selectedGem;
        }
        else if (m_currentGem == selectedGem)
        {
            m_currentGem = null;
        }
        else if (!m_currentGem.IsAdjacent(selectedGem))
        {
            m_currentGem.ToggleSelector();
            m_currentGem = selectedGem;
        }
        else
        {
            isSwapping = true;

            Swap(m_currentGem, selectedGem);

            m_currentGem.ToggleSelector();
            selectedGem.ToggleSelector();
            m_currentGem = null;

        }
    }

    private void Swap(Gem gem1, Gem gem2)
    {
        //Swap X, Y
        int tempX = gem1.x;
        int tempY = gem1.y;

        gem1.x = gem2.x;
        gem1.y = gem2.y;

        gem2.x = tempX;
        gem2.y = tempY;

        //Swap position
        Vector3 gem1Position = gem1.transform.position;
        Vector3 gem2Position = gem2.transform.position;

        StartCoroutine(MoveGem(gem1, gem1Position, gem2Position));
        StartCoroutine(MoveGem(gem2, gem2Position, gem1Position));

    }

    IEnumerator MoveGem(Gem gem, Vector3 from, Vector3 to)
    {

        float time = Time.time;
        while (Vector3.Distance(gem.transform.position, to) > 0.01f)
        {
            gem.transform.position = Vector3.Lerp(from, to, (Time.time - time) * 1.2f);

            yield return null;
        }

        gem.transform.position = to;

        isSwapping = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
