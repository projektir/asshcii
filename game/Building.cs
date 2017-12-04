using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using asshcii.ecs;
using asshcii.game.components;

namespace asshcii.game {
    public class Building : Entity {
        [Obsolete]
        public byte UpgradeLevel { get; private set; } = 0;

        public Building(string name, Ascii ascii) : base(name) {
            this.AddComponent(ascii);
            this.AddComponent(new UpgradeLevel(0));
        }

        public void Upgrade() {
            var costs = this.GetComponents<IUpgradeCost>();
            var level = this.GetComponent<UpgradeLevel>();

            foreach (var resource in costs.ToList()) {
                // Write some exponential increase logic instead
                resource.Increase();
            }

            level.Upgrade();
        }

        public override string ToString() {
            var level = this.GetComponent<UpgradeLevel>();
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(base.ToString());
            stringBuilder.Append(", ");
            stringBuilder.Append($"Upgrade level: {level.Level}");

            return stringBuilder.ToString();
        }
    }
}
