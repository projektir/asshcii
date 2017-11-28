using asshcii.ecs;

namespace asshcii.game {
    public class Planet : Entity {
        public Planet(string name, AvailableResources resources) : base(name) {
            AddComponent(resources);
        }
    }
}
