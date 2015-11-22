using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Match3
{
    public class Board : MonoBehaviour
    {
        public int gridWidth = 5;
        public int gridHeight = 7;

        private float startHeight = 4.0f;

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
                    Gem gem = CreateGem(x, y);
                    m_gems[x, y] = gem;
                    Vector3 fromPosition = gem.transform.position;
                    Vector3 toPosition = new Vector3(-gridWidth / 2f + x + 0.5f, -gridHeight / 2f + y + 0.5f);
                    StartCoroutine(MoveGem(gem, fromPosition, toPosition, CheckBoard));
                }
            }
        }

        private void CheckBoard()
        {
            bool ready = true;
            foreach (var gem in m_gems)
            {
                if (gem.gemStatus != Data.GemStatus.Ready)
                {
                    ready = false;
                    break;
                }
            }

            if (ready)
            {
                ProcessBoard();
            }
        }

        private Gem CreateGem(int x, int y)
        {
            GameObject obj = Instantiate(gemPrefab, new Vector3(-gridWidth / 2f + x + 0.5f, startHeight), Quaternion.identity) as GameObject;
            obj.transform.SetParent(transform);
            Gem gem = obj.GetComponent<Gem>();

            gem.CreateGem();
            gem.x = x;
            gem.y = y;
            return gem;
        }

        private void ProcessBoard()
        {
            var matches = Match();
            if (matches.Count != 0)
            {
                var gemRemovedCount = RemoveMatchGems(matches);
                AddNewGems(gemRemovedCount);
            }
        }

        private void AddNewGems(int[] gemRemovedCount)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (gemRemovedCount[x] == 0)
                {
                    continue;
                }

                for (int y = 0; y < gridHeight; y++)
                {
                    if (m_gems[x, y] == null)
                    {
                        Gem gem = GetUpperGem(x, y);

                        if (gem != null)
                        {
                            gem.y = y;
                            m_gems[x, y] = gem;

                            Vector3 fromPosition = gem.transform.position;
                            Vector3 toPosition = new Vector3(-gridWidth / 2f + x + 0.5f, -gridHeight / 2f + y + 0.5f);
                            StartCoroutine(MoveGem(gem, fromPosition, toPosition, CheckBoard));
                        }
                        else
                        {
                            for (int i = y; i < gridHeight; i++)
                            {
                                gem = CreateGem(x, y);
                                m_gems[x, y] = gem;

                                Vector3 fromPosition = gem.transform.position;
                                Vector3 toPosition = new Vector3(-gridWidth / 2f + x + 0.5f, -gridHeight / 2f + y + 0.5f);
                                StartCoroutine(MoveGem(gem, fromPosition, toPosition, CheckBoard));

                                y++;
                            }
                        }
                    }
                }
            }
        }

        private Gem GetUpperGem(int x, int y)
        {
            Gem gem = null;
            if (IsInBoard(x, y + 1))
            {
                if (m_gems[x, y + 1] != null)
                {
                    gem = m_gems[x, y + 1];
                    m_gems[x, y + 1] = null;
                }
                else
                {
                    gem = GetUpperGem(x, y + 1);
                }
            }
            return gem;
        }

        private int[] RemoveMatchGems(List<List<Gem>> matches)
        {
            int[] gemRemovedCount = new int[gridWidth];

            foreach (var match in matches)
            {
                foreach (var gem in match)
                {
                    if (gem != null)
                    {
                        m_gems[gem.x, gem.y] = null;
                        gemRemovedCount[gem.x]++;
                        gem.Remove();
                    }
                }
            }

            return gemRemovedCount;
        }

        public void SwapGems(Gem selectedGem)
        {
            if (selectedGem.gemStatus != Data.GemStatus.Ready)
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
                m_currentGem.gemStatus = Data.GemStatus.Moving;
                selectedGem.gemStatus = Data.GemStatus.Moving;

                Swap(m_currentGem, selectedGem);

                m_currentGem.ToggleSelector();
                selectedGem.ToggleSelector();
                m_currentGem = null;
            }
        }

        private void Swap(Gem gem1, Gem gem2)
        {
            //Swap X, Y
            m_gems[gem1.x, gem1.y] = gem2;
            m_gems[gem2.x, gem2.y] = gem1;

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
            StartCoroutine(MoveGem(gem2, gem2Position, gem1Position, CheckBoard));
        }

        IEnumerator MoveGem(Gem gem, Vector3 from, Vector3 to, Action callback = null)
        {
            gem.gemStatus = Data.GemStatus.Moving;
            float time = Time.time;
            while (Vector3.Distance(gem.transform.position, to) > 0.01f)
            {
                gem.transform.position = Vector3.Lerp(from, to, (Time.time - time) * 1.2f);

                yield return null;
            }

            gem.transform.position = to;

            gem.gemStatus = Data.GemStatus.Ready;

            if (callback != null)
            {
                callback.Invoke();
            }
        }

        private List<List<Gem>> Match()
        {
            //Look through the board find all match
            List<List<Gem>> matches = new List<List<Gem>>();

            //Horizontal
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth - 2; x++)
                {
                    int count = CheckHorizontal(x, y) - x + 1;
                    //Got Match
                    if (count >= 3)
                    {
                        List<Gem> match = new List<Gem>();
                        for (int i = 0; i < count; i++)
                        {
                            //Add to match list
                            match.Add(m_gems[x + i, y]);
                        }
                        matches.Add(match);
                    }
                    x = x + count - 1;
                }
            }

            //Vertical
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight - 2; y++)
                {
                    int count = CheckVertical(x, y) - y + 1;
                    //Got Match
                    if (count >= 3)
                    {
                        List<Gem> match = new List<Gem>();
                        for (int i = 0; i < count; i++)
                        {
                            //Add to match list
                            match.Add(m_gems[x, y + i]);
                        }
                        matches.Add(match);
                    }
                    y = y + count - 1;
                }
            }
            return matches;
        }

        private int CheckHorizontal(int x, int y)
        {
            int last = x;

            if (IsInBoard(x + 1, y))
            {
                if (m_gems[x, y].IsMatch(m_gems[x + 1, y]))
                {
                    last = CheckHorizontal(x + 1, y);
                }
            }

            return last;
        }

        private int CheckVertical(int x, int y)
        {
            int last = y;

            if (IsInBoard(x, y + 1))
            {
                if (m_gems[x, y].IsMatch(m_gems[x, y + 1]))
                {
                    last = CheckVertical(x, y + 1);
                }
            }

            return last;
        }

        private bool IsInBoard(int x, int y)
        {
            return !(x < 0 || x >= gridWidth || y < 0 || y >= gridHeight);
        }
    }
}