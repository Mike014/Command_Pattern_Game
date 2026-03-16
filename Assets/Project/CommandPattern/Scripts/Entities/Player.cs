using BattleNavale.Commands;
using BattleNavale.Core;
using BattleNavale.Managers;
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

            if (Input.GetMouseButtonDown(0))
            {
                ICommand command = new AttackCommand(0, 0, _battleGrid); // placeholder
                _turnManager.ExecuteCommand(command);
                _isMyTurn = false;
            }
        }
    }
}


