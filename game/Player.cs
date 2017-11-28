using asshcii.ecs;

namespace asshcii.game {
    public class Player : Entity {
        public PlayerBase PlayerBase { get; private set; }

        public Player(string name) : base(name) { }
    }
}
