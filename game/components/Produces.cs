using asshcii.game.resources;

namespace asshcii.game.components
{
    public interface IProduces : IComponentWithResourceAndAmount
    {
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