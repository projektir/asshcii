using asshcii.ecs;
using asshcii.game.components;

namespace asshcii.game {
    public class Ship : Entity {
        public Ship(string name, Attack attack, Health health)
         : base(name) {
            AddComponent(attack);
            AddComponent(health);
        }

        public void Attack(Ship ship) {
            Health enemyHealth = ship.GetComponent<Health>();
            int shipDamage = GetComponent<Attack>().Damage;
            enemyHealth.DamageBy(shipDamage);
        }
    }
}
