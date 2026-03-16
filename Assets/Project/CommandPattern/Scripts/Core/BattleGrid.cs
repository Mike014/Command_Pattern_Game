using UnityEngine;

namespace BattleNavale.Core
{
    public class BattleGrid : MonoBehaviour
    {
        private Cell[,] _cells;
        private const int GridSize = 10;

        private void Awake()
        {
            _cells = new Cell[GridSize, GridSize];

            // Logica per costruire la griglia
            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    _cells[x, y] = new Cell();
                }
            }
        }

        public void ReceiveAttack(int x, int y)
        {
            _cells[x, y].ReceiveAttack();
            Debug.Log("[Grid]: Attacco Ricevuto");
        }

        public void UndoAttack(int x, int y)
        {
            _cells[x, y].UndoAttack();
            Debug.Log("[Grid]: Attacco Undo");
        }
    }
}

