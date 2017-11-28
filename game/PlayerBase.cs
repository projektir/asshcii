using System.Collections.Generic;
using System.Text;
using System.Linq;

using asshcii.ecs;
using asshcii.game.components;

namespace asshcii.game {
    public class PlayerBase : Entity {
        public Resources Resources { get; private set; }
        public Planet Planet { get; private set; }

        private List<Building> _buildings;

        public PlayerBase(string name, Resources resources, Planet planet) : base(name) {
            Resources = resources;
            Planet = planet;

            _buildings = new List<Building>();
        }

        public bool TryBuild(Building building) {
            var costs = building.GetComponent<Resources>();
            var resources = Resources;

            if (resources.TrySubtractResources(costs, out resources)) {
                building.Upgrade();

                if (!_buildings.Contains(building)) {
                    _buildings.Add(building);
                }

                Resources = resources;

                return true;
            } else {
                return false;
            }
        }

        public override string ToString() {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(base.ToString());
            stringBuilder.Append(", ");
            stringBuilder.Append($"{nameof(Resources)}: {Resources}, ");
            stringBuilder.Append($"{nameof(Planet)}: {Planet}, ");

            stringBuilder.Append("Buildings: [");

            foreach (Building building in _buildings) {
                stringBuilder.Append(building);

                if (building != _buildings.Last()) {
                    stringBuilder.Append(", ");
                }
            }

            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }
    }
}
