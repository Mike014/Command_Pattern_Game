using BattleNavale.Core;

namespace BattleNavale.Commands
{
    public class AttackCommand : ICommand
    {
        private BattleGrid _enemyGrid;
        private int _x, _y;

        public AttackCommand(int x, int y, BattleGrid enemyGrid)
        {
            _x = x;
            _y = y;
            _enemyGrid = enemyGrid;
        }

        public void Execute()
        {
            _enemyGrid.ReceiveAttack(_x, _y);
        }

        public void Undo()
        {
            _enemyGrid.UndoAttack(_x, _y);
        }
    }
}

