using BattleNavale.Commands;
using BattleNavale.Managers;
using BattleNavale.Core;
using UnityEngine;

namespace BattleNavale.Entities
{
    public class Player : MonoBehaviour, IPlayer
    {
        private bool _isMyTurn = false; // TakeTurn() non legge input direttamente. Invece abilita un flag
        [SerializeField] private TurnManager _turnManager;
        [SerializeField] private BattleGrid _battleGrid;
        private Camera _mainCamera;
        // private float _cellSize;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public void TakeTurn()
        {
            _isMyTurn = true;
        }

        private void Update()
        {
            if (!_isMyTurn) return;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                _turnManager.UndoLastCommand();
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    int gridX = Mathf.RoundToInt(hit.point.x / _battleGrid.CellSize);
                    int gridY = Mathf.RoundToInt(hit.point.z / _battleGrid.CellSize);

                    Debug.Log($"[Player]: Cella colpita ({gridX}, {gridY})");

                    ICommand command = new AttackCommand(gridX, gridY, _battleGrid);
                    _isMyTurn = false;
                    _turnManager.ExecuteCommand(command);
                }
            }
        }
    }
}


