using System.Text;

namespace ashhcii {
    public class Ship : Entity {
        public Ship(string name, Attack attack, Health health) : base(name) {            
            this.AddComponent(attack);
            this.AddComponent(health);
        }

        public void Attack(Ship ship) {
            var enemyHealth = ship.GetComponent(typeof(Health)) as Health;
            var shipDamage = (this.GetComponent(typeof(Attack)) as Attack).Damage;
            enemyHealth.DamageBy(shipDamage);
        }
    }
}
