using asshcii.ecs;
using asshcii.game.resources;

namespace asshcii.game.components
{
    public interface IProduces : IComponent
    {
        int Amount { get; }
        IResource Resource { get; }
    }
    public class Produces<TResource> : ComponentWithResourceAndAmount<TResource>, IProduces where TResource : IResource, new()
    {
        public Produces(int amount) : base(amount)
        {
        }

        public override string ToString()
        {
            return $"produces {Amount} {Resource.Name}";
        }
    }
}