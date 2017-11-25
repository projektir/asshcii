using System.Text;

namespace ashhcii {
    public class Building : Entity {
        public byte UpgradeLevel { get; private set; } = 1;
        public int Cost { get; }

        public Building(string name, Ascii ascii) : base(name) {
            this.AddComponent(ascii);
        }

        public void Upgrade() {
            UpgradeLevel++;
        }

        public override string ToString() {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(base.ToString());
            stringBuilder.Append(", ");
            stringBuilder.Append($"{nameof(UpgradeLevel)}: {UpgradeLevel}");
            stringBuilder.Append($"{nameof(Cost)}: {Cost}");

            return stringBuilder.ToString();
        }
    }
}
