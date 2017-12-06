using asshcii.ecs;
using asshcii.game.components;

namespace asshcii.game
{
    public class Ship : Entity
    {
        public Ship(string name, Attack attack, Health health)
         : base(name)
        {
            AddComponent(attack);
            AddComponent(health);
        }

        public void Attack(Ship ship)
        {
            var enemyHealth = ship.GetComponent<Health>();
            var shipDamage = GetComponent<Attack>().Damage;
            enemyHealth.DamageBy(shipDamage);
        }
    }
}
