using System.Collections.Generic;
using System.Text;
using System.Linq;

using asshcii.ecs;
using asshcii.game.components;

namespace asshcii.game {
    public class PlayerBase : Entity {
        public Planet Planet { get; private set; }

        public List<Building> Buildings = new List<Building>();

        public PlayerBase(string name, Planet planet) : base(name) {
            Planet = planet;
        }

        public bool TryBuild(Building building) {
            var costs = building.GetComponents<IUpgradeCost>();
            var resources = GetComponents<IStorage>();

            foreach(var cost in costs){
                var matchingResource = resources.FirstOrDefault(r => r.Resource.Equals(cost.Resource));
                if(matchingResource == null || matchingResource.Amount < cost.Amount) return false;
            }
            foreach(var cost in costs){
                var matchingResource = resources.First(r => r.Resource.Equals(cost.Resource));
                matchingResource.Subtract(cost.Amount);
            }

            building.Upgrade();
            return true;
        }

        public override string ToString() {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(base.ToString());
            stringBuilder.Append(", Resources: ");
            foreach(var resource in GetComponents<IStorage>()){
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
