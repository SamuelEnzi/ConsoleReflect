namespace ConsoleReflect.Types
{
    public interface ICommand
    {
        string CommandString { get; set; }
        string Help { get; set; }

        void Execute();
    }
}
