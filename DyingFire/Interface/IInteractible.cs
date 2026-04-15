namespace DyingFire.Models
{
    public interface IInteractable
    {
        string Name { get; }
        string Interact(); // Returns text describing what happened
    }
}