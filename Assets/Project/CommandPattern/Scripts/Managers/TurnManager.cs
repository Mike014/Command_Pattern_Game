using System.Collections.Generic;
using BattleNavale.Entities;
using BattleNavale.Commands;
using UnityEngine;

namespace BattleNavale.Managers
{
    public class TurnManager : MonoBehaviour
    {
        private Queue<IPlayer> _turnQueue;
        private Stack<ICommand> _commandHistory;

        private void Awake()
        {
            _turnQueue = new Queue<IPlayer>();
            _commandHistory = new Stack<ICommand>();
        }
        public void StartGame(IPlayer playerOne, IPlayer playerTwo)
        {
            _turnQueue.Enqueue(playerOne);
            _turnQueue.Enqueue(playerTwo);
        }

        public void NextTurn()
        {
            IPlayer currentPlayer = _turnQueue.Dequeue();
            currentPlayer.TakeTurn();
            _turnQueue.Enqueue(currentPlayer);
        }

        public void UndoLastCommand()
        {
            if (_commandHistory.Count == 0) return;

            ICommand lastCommand = _commandHistory.Pop();
            lastCommand.Undo();
        }

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _commandHistory.Push(command);
        }
    }
}
