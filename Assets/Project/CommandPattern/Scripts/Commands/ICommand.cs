
namespace BattleNavale.Commands
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}
