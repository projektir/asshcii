using asshcii.ecs;
using asshcii.game.resources;

namespace asshcii.game.components
{
    public interface IComponentWithResourceAndAmount : IComponent {
        int Amount { get; }
        IResource Resource { get; }
    }
    public abstract class ComponentWithResourceAndAmount<TResource> : IComponent where TResource : IResource, new()
    {
        protected ComponentWithResourceAndAmount(int amount)
        {
            Amount = amount;
        }
        public int Amount { get; protected set; }
        public IResource Resource => new TResource();
        public abstract override string ToString();
    }
}
