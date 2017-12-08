using System.Collections.Generic;
using System.Text;
using System.Linq;

using asshcii.ecs;
using asshcii.game.components;
using asshcii.game.buildings;

namespace asshcii.game
{
    public class PlayerBase : Entity
    {
        public Planet Planet { get; private set; }

        // TODO: Assuming buildings are unique, we could make a build building also a component
        // E.g. `AddComponent(new Build<IronMine>());`
        // To get all buildings, we simply do `.GetComponents<Building>();`
        public List<Building> Buildings = new List<Building>();

        public PlayerBase(string name, Planet planet) : base(name)
        {
            Planet = planet;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(base.ToString());
            stringBuilder.Append(", Resources: ");
            foreach (var resource in GetComponents<IStorage>())
            {
                stringBuilder.Append($"{resource.Resource.Name}: {resource.Amount}, ");
            }
            stringBuilder.Append($"{nameof(Planet)}: {Planet}, ");

            stringBuilder.AppendLine("Buildings: [");

            foreach (var building in Buildings)
            {
                stringBuilder.Append("    ").Append(building);

                if (building != Buildings.Last())
                {
                    stringBuilder.AppendLine(", ");
                }
                else
                {
                    stringBuilder.AppendLine();
                }
            }

            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }
    }
}
