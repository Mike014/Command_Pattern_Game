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
                Debug.Log($"Tasto (Z) premuto");
            }

            if (Input.GetMouseButtonDown(0))
            {
                ICommand command = new AttackCommand(0, 0, _battleGrid); // placeholder
                _isMyTurn = false;
                _turnManager.ExecuteCommand(command);
                Debug.Log($"Tasto (0) premuto");
                // _turnManager.NextTurn();
            }
        }
    }
}


