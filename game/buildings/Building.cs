using System.Text;
using asshcii.ecs;
using asshcii.game.components;

namespace asshcii.game.buildings
{
    public class Building : Entity
    {
        public Building(string name, Ascii ascii) : base(name)
        {
            AddComponent(ascii);
            AddComponent(new UpgradeLevel(0));
        }

        public void Upgrade()
        {
            var costs = GetComponents<IUpgradeCost>();
            var level = GetComponent<UpgradeLevel>();

            foreach (var resource in costs)
            {
                // Write some exponential increase logic instead
                resource.Increase();
            }

            level.Upgrade();
        }

        public override string ToString()
        {
            var level = GetComponent<UpgradeLevel>();
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(base.ToString());
            stringBuilder.Append(", ");
            stringBuilder.Append($"Upgrade level: {level.Level}");

            return stringBuilder.ToString();
        }
    }
}
