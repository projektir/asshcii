using System.Collections.Generic;
using System.Text;
using System.Linq;

using asshcii.ecs;
using asshcii.game.components;

namespace asshcii.game {
    public class PlayerBase : Entity {
        public Resources Resources { get; set; }
        public Planet Planet { get; private set; }
        public List<Building> Buildings { get; private set; }

        public PlayerBase(string name, Resources resources, Planet planet) : base(name) {
            Resources = resources;
            Planet = planet;

            Buildings = new List<Building>();
        }

        public override string ToString() {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(base.ToString());
            stringBuilder.Append(", ");
            stringBuilder.Append($"{nameof(Resources)}: {Resources}, ");
            stringBuilder.Append($"{nameof(Planet)}: {Planet}, ");

            stringBuilder.Append("Buildings: [");

            foreach (Building building in Buildings) {
                stringBuilder.Append(building);

                if (building != Buildings.Last()) {
                    stringBuilder.Append(", ");
                }
            }

            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }
    }
}
