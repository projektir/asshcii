namespace ashhcii {
    public class Attack : Component {
        public int Damage { get; }

        public Attack(int damage) {
            Damage = damage;
        }

        public override string ToString() {
            return $"{nameof(Damage)}: {Damage}";
        }
    }
}
