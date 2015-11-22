using UnityEngine;
using System;

namespace Match3
{
    public class Gem : MonoBehaviour
    {
        public Renderer modelRenderer;
        public GameObject selector;
        public ParticleSystem selectorParticle;

        public int x;
        public int y;

        public int gemType;
        public Data.GemStatus gemStatus;

        private bool m_isSelected = false;

        public void CreateGem()
        {
            gemType = UnityEngine.Random.Range(0, Data.GEMTYPE.Length);
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

        public bool IsMatch(Gem otherGem)
        {
            return gemType == otherGem.gemType;
        }

        void OnMouseDown()
        {
            //TODO: improve performance
            GameObject.Find("Board").GetComponent<Board>().SwapGems(this);
        }

        public void Remove()
        {
            gemStatus = Data.GemStatus.Removing;
            DestroyObject(gameObject);
        }
    }
}