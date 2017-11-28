using System.Collections.Generic;
using System.Text;
using System.Linq;

using asshcii.ecs;
using asshcii.game.components;

namespace asshcii.game {
    public class PlayerBase : Entity {
        public Planet Planet { get; }
        public readonly List<Building> Buildings = new List<Building>();

        public PlayerBase(string name, Planet planet) : base(name) {
            Planet = planet;
        }

        public bool TryBuild(Building building) {
            IUpgradeCost[] costs = building.GetComponents<IUpgradeCost>().ToArray();
            IStorage[] resources = GetComponents<IStorage>().ToArray();

            foreach(IUpgradeCost cost in costs){
                IStorage matchingResource = resources.FirstOrDefault(r => r.Resource.Equals(cost.Resource));
                if(matchingResource == null || matchingResource.Amount < cost.Amount) return false;
            }
            /*
            if ((
                from cost in costs
                let matchingResource = resources.FirstOrDefault(r => r.Resource.Equals(cost.Resource))
                where matchingResource == null || matchingResource.Amount < cost.Amount
                select cost
            ).Any())
            {
                return false;
            }
            */
            foreach(IUpgradeCost cost in costs){
                IStorage matchingResource = resources.First(r => r.Resource.Equals(cost.Resource));
                matchingResource.Subtract(cost.Amount);
            }

            building.Upgrade();
            return true;
        }

        public override string ToString() {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(base.ToString());
            stringBuilder.Append(", Resources: ");
            foreach(IStorage resource in GetComponents<IStorage>()){
                stringBuilder.Append($"{resource.Resource.Name}: {resource.Amount}, ");
            }
            stringBuilder.Append($"{nameof(Planet)}: {Planet}, ");

            stringBuilder.AppendLine("Buildings: [");

            foreach (Building building in Buildings) {
                stringBuilder.Append("    ").Append(building);

                if (building != Buildings.Last()) {
                    stringBuilder.AppendLine(", ");
                } else {
                    stringBuilder.AppendLine();
                }
            }

            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }
    }
}
