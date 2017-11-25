using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ashhcii
{
    public abstract class Entity {
        public string Name { get; }
        private List<Component> components;

        internal Entity(string name) {
            Name = name;
            components = new List<Component>();
        }

        public Component GetComponent(Type type) {
            return components.Single(cmpt => cmpt.GetType().Equals(type));
        }

        public void AddComponent(Component component) {
            var type = component.GetType();
            if (components.Exists(cmpt => cmpt.GetType().Equals(type))) {
                throw new ArgumentException($"The component {nameof(type)} is already present.");
            }

            components.Add(component);
        }

        public override string ToString() {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"Name: {Name}, ");
            
            foreach (Component component in components) {
                var type = component.GetType().Name;
                stringBuilder.Append($"{type}: {{{component.ToString()}}}");

                if (component != components.Last()) {
                    stringBuilder.Append(", ");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
