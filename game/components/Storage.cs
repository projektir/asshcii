using asshcii.ecs;
using asshcii.game.resources;

namespace asshcii.game.components
{
    public interface IStorage : IComponent {
        int Amount {get;}
        IResource Resource{get;}
        void Subtract(int amount);
    }

    public class Storage<TResource> : ComponentWithResourceAndAmount<TResource>, IStorage where TResource : IResource, new() {
        public Storage(int amount) : base(amount) {
        }
        public void Subtract(int amount){
            Amount -= amount;
        }

        public override string ToString() {
            return $"stores {Amount} {Resource.Name}";
        }
    }
}