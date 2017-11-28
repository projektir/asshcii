using System.Collections.Generic;
using System.Linq;
using System.Text;

using asshcii.ecs;
using asshcii.game.components;

namespace asshcii.game {
    public class Building : Entity {
        public byte UpgradeLevel { get; private set; } = 0;

        public Building(string name, Ascii ascii, Resources resourceCosts) : base(name) {
            this.AddComponent(ascii);
            this.AddComponent(resourceCosts);
        }

        public void Upgrade() {
            var resourceCosts = this.GetComponent<Resources>().InnerResources;

            foreach (var resource in resourceCosts.ToList()) {
                // Write some exponential increase logic instead
                resourceCosts[resource.Key] = resource.Value + ((resource.Value / 2) * UpgradeLevel);
            }

            UpgradeLevel++;
        }

        public override string ToString() {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(base.ToString());
            stringBuilder.Append(", ");
            stringBuilder.Append($"{nameof(UpgradeLevel)}: {UpgradeLevel}");

            return stringBuilder.ToString();
        }
    }
}
