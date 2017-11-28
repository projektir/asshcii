using asshcii.ecs;
using asshcii.game.resources;

namespace asshcii.game.components
{
    public abstract class ComponentWithResourceAndAmount<TResource> : IComponent where TResource : IResource, new() {
        protected ComponentWithResourceAndAmount(int amount) {
            Amount = amount;
        }
        public int Amount {get; protected set;}
        public IResource Resource => new TResource();
    }
    public interface IConsumes : IComponent { 
        int Amount {get;}
        IResource Resource {get;}
    }
    public class Consumes<TResource> : ComponentWithResourceAndAmount<TResource>, IConsumes where TResource : IResource, new()
    {
        public Consumes(int amount) : base(amount)
        {
        }
    }
    public interface IProduces : IComponent {
        int Amount {get;}
        IResource Resource {get;}
    }
    public class Produces<TResource> : ComponentWithResourceAndAmount<TResource>, IProduces where TResource : IResource, new()
    {
        public Produces(int amount) : base(amount)
        {
        }
    }
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
    }
    public class UpgradeLevel : IComponent {
        public UpgradeLevel(int level) {
            Level = level;
        }

        public void Upgrade() {
            Level ++;
        }

        public int Level {get;private set;}
    }

    public interface IStorage : IComponent {
        int Amount {get;}
        IResource Resource{get;}
    }

    public class Storage<TResource> : ComponentWithResourceAndAmount<TResource>, IStorage where TResource : IResource, new() {
        public Storage(int amount) : base(amount) {
        }
    }
}