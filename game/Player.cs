using System.Collections.Generic;
using asshcii.ecs;

namespace asshcii.game {
    public class Player : Entity {
        public List<PlayerBase> PlayerBase { get; private set; } = new List<PlayerBase>();

        public Player(string name) : base(name) { }
    }
}
