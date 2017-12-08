using System.Linq;
using asshcii.game.buildings;
using asshcii.game.components;

namespace asshcii.game.actions
{
    public class UpgradeBuildingAction : IAction
    {
        public Building Building { get; }

        public UpgradeBuildingAction(Building building)
        {
            Building = building;
        }
        public ExecuteResult Execute(PlayerBase playerBase)
        {
            var storedResources = playerBase.GetComponents<IStorage>().ToArray();
            var costs = Building.GetComponents<IUpgradeCost>().ToArray();

            foreach (var cost in costs)
            {
                var storedResource = storedResources.FirstOrDefault(r => r.Resource.Equals(cost.Resource));
                if (storedResource == null || storedResource.Amount < cost.Amount)
                {
                    return new NotEnoughResources(cost);
                }
            }

            foreach (var cost in costs)
            {
                var storedResource = storedResources.First(r => r.Resource.Equals(cost.Resource));
                storedResource.Subtract(cost.Amount);
            }

            Building.Upgrade();

            if (!playerBase.Buildings.Contains(Building))
            {
                playerBase.Buildings.Add(Building);
            }

            return new Success();
        }
    }

    public class NotEnoughResources : ExecuteResult
    {
        public NotEnoughResources() : base(false, "Not enough resources")
        {
        }

        public NotEnoughResources(IComponentWithResourceAndAmount resourceAndAmount)
            : base(false, "Not enough " + resourceAndAmount.Resource.Name + "; need " + resourceAndAmount.Amount)
        {

        }
    }
}