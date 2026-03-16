using UnityEngine;
using BattleNavale.Entities;

namespace BattleNavale.Managers
{

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TurnManager _turnManager;
        [SerializeField] private Player _playerOne;
        [SerializeField] private Player _playerTwo;

        void Start()
        {
            _turnManager.StartGame(_playerOne, _playerTwo);
        }
    }
}


