using asshcii.game.components;
using asshcii.game.resources;

namespace asshcii.game.buildings
{
    public class IronMine : Building
    {
        private static readonly Ascii ascii = new Ascii(new[,] {
            { 'M', 'M' },
            { 'M', 'M' }
        });

        public IronMine() : base("Iron mine", ascii)
        {
            AddComponent(new Produces<IronResource>(100));

            AddComponent(new UpgradeCost<IronResource>(100));
            AddComponent(new UpgradeCost<PowerResource>(100));
        }
    }
}