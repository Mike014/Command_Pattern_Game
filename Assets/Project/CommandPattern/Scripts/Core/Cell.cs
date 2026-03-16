using UnityEngine;
namespace BattleNavale.Core
{
    public enum CellState
    {
        Intact, Hit
    }
    public class Cell
    {
        private CellState _state;
        public CellState State => _state;
        public void ReceiveAttack()
        {
            _state = CellState.Hit;
            Debug.Log("[Cells]: Attacco Ricevuto");
        }

        public void UndoAttack()
        {
            _state = CellState.Intact;
            Debug.Log("[Cells]: Attacco Undo");
        }
    }
}

