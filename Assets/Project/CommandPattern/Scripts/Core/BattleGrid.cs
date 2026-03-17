using UnityEngine;

namespace BattleNavale.Core
{
    public class BattleGrid : MonoBehaviour
    {
        private Cell[,] _cells;
        private GameObject[,] _cellObjects;
        private Renderer[,] _cellRenderers;
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private float _cellSize = 1f;
        public float CellSize => _cellSize; // getter
        [SerializeField] private int _gridWidth = 5;
        [SerializeField] private int _gridHeight = 5;
        private GameObject _gridParent;

        private void Awake()
        {
            _cells = new Cell[_gridWidth, _gridHeight];
            _cellObjects = new GameObject[_gridWidth, _gridHeight];
            _cellRenderers = new Renderer[_gridWidth, _gridHeight];
            GenerateGrid();
        }

        public void ReceiveAttack(int x, int y)
        {
            if (_cells[x, y].State == CellState.Hit)
            {
                Debug.Log("[Grid]: Cella già colpita!");

                return;
            }
            _cells[x, y].ReceiveAttack();
            Debug.Log("[Grid]: Attacco Ricevuto");

            _cellRenderers[x, y].material.color = Color.red;
        }

        public void UndoAttack(int x, int y)
        {
            _cells[x, y].UndoAttack();
            Debug.Log("[Grid]: Attacco Undo");

            _cellRenderers[x, y].material.color = Color.white;
        }

        void GenerateGrid()
        {
            // Creiamo un contenitore vuoto per mantenere ordinata la scena
            _gridParent = new GameObject("GridHolder");

            // Ciclo esterno: scorre le righe (Y logico, che diventerà Z fisico in Unity)
            for (int y = 0; y < _gridHeight; y++)
            {
                // Ciclo interno: scorre le colonne (X logico e fisico)
                for (int x = 0; x < _gridWidth; x++)
                {
                    Vector3 spawnPosition = new Vector3(x * _cellSize, 0, y * _cellSize);

                    _cells[x, y] = new Cell();

                    GameObject newCell = Instantiate(_cellPrefab, spawnPosition, Quaternion.identity);

                    _cellObjects[x, y] = newCell;
                    _cellRenderers[x, y] = newCell.GetComponent<Renderer>();

                    newCell.transform.SetParent(_gridParent.transform);

                    newCell.name = $"Cell_{x}_{y}";
                }
            }
        }

        void OnDestroy()
        {
            Destroy(_gridParent);
        }
    }
}

