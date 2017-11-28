using asshcii.ecs;

namespace asshcii.game.components {
    public class Health : IComponent {
        private int _hitPoints { get; set; }
        private int _maxHitPoints { get; set; }

        public Health(int hitPoints) {
            _hitPoints = hitPoints;
            _maxHitPoints = hitPoints;
        }

        public void DamageBy(int damage) {
            _hitPoints -= damage;

            if (_hitPoints < 0) {
                _hitPoints = 0;
            }
        }

        public void HealBy(int healing) {
            _hitPoints += healing;

            if (_hitPoints > _maxHitPoints) {
                _hitPoints = _maxHitPoints;
            }
        }

        public override string ToString() {
            return $"HitPoints: {_hitPoints}, MaxHitPoints: {_maxHitPoints}";
        }
    }
}
