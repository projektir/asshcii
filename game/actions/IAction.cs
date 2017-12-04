namespace asshcii.game.actions
{
    public interface IAction
    {
        ExecuteResult Execute(PlayerBase playerBase);
    }

    public abstract class ExecuteResult
    {
        public bool Success { get; protected set; }
        public string Message { get; protected set; }

        public ExecuteResult(bool success, string message = null)
        {
            Success = success;
            Message = message;
        }
    }

    public class Success : ExecuteResult {
        public Success() : base(true) {

        }
    }
}