using System.Collections.Generic;
using System.Text;
using System.Linq;

using asshcii.ecs;

namespace asshcii {
    public enum Resource { Power, Iron }

    public class AvailableResources : Component {
        public Dictionary<Resource, bool> Resources { get; private set; }

        public AvailableResources(bool power, bool iron) {
            Resources = new Dictionary<Resource, bool>();

            Resources.Add(Resource.Power, power);
            Resources.Add(Resource.Iron, iron);
        }

        public override string ToString() {
            var stringBuilder = new StringBuilder();
            
            foreach (var resource in Resources) {
                stringBuilder.Append($"{resource.Key}: {resource.Value}");

                if (resource.Key != Resources.Last().Key) {
                    stringBuilder.Append(", ");
                }
            }

            return stringBuilder.ToString();
        }
    }

    public class Resources : Component {
        public Dictionary<Resource, int> InnerResources { get; set; }

        public Resources(int power, int iron) {
            InnerResources = new Dictionary<Resource, int>();

            InnerResources.Add(Resource.Power, power);
            InnerResources.Add(Resource.Iron, iron);
        }

        public bool TrySubtractResources(Resources resources, out Resources subtractedResources) {
            subtractedResources = new Resources(0, 0);

            foreach(var resource in resources.InnerResources) {
                subtractedResources.InnerResources[resource.Key] = this.InnerResources[resource.Key] - resource.Value;
                if (subtractedResources.InnerResources[resource.Key] < 0) {
                    return false;
                }
            }

            return true;
        }

        public override string ToString() {
            var stringBuilder = new StringBuilder();
            
            foreach (var resource in InnerResources) {
                stringBuilder.Append($"{resource.Key}: {resource.Value}");

                if (resource.Key != InnerResources.Last().Key) {
                    stringBuilder.Append(", ");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
