using asshcii.ecs;
using asshcii.game.resources;

namespace asshcii.game.components
{
    public interface IConsumes : IComponent { 
        int Amount {get;}
        IResource Resource {get;}
    }
    public class Consumes<TResource> : ComponentWithResourceAndAmount<TResource>, IConsumes where TResource : IResource, new()
    {
        public Consumes(int amount) : base(amount)
        {
        }

        public override string ToString() {
            return $"consumes {Amount} {Resource.Name}";
        }
    }
}