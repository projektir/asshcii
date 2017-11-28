using asshcii.ecs;

namespace asshcii.game.components
{
    public class UpgradeLevel : IComponent {
        public UpgradeLevel(int level) {
            Level = level;
        }

        public void Upgrade() {
            Level ++;
        }

        public int Level {get;private set;}
        public override string ToString(){
            return $"level {Level}";
        }
    }
} 