using asshcii.ecs;
using asshcii.game.resources;

namespace asshcii.game.components
{
    public interface IUpgradeCost : IComponent {
        int Amount {get;}
        IResource Resource {get;}

        void Increase();
    }
    public class UpgradeCost<TResource> : ComponentWithResourceAndAmount<TResource>, IUpgradeCost where TResource : IResource, new()
    {
        public UpgradeCost(int amount) : base(amount)
        {
        }
        public void Increase(){
            Amount *= 2;
        }

        public override string ToString() {
            return $"costs {Amount} {Resource.Name}";
        }
    }
}