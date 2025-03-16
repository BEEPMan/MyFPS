public abstract class BaseState
{
    public StateMachine stateMachine;

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Perform();
}