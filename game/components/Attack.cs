using asshcii.ecs;

namespace asshcii {
    public class Attack : IComponent {
        public int Damage { get; }

        public Attack(int damage) {
            Damage = damage;
        }

        public override string ToString() {
            return $"{nameof(Damage)}: {Damage}";
        }
    }
}
