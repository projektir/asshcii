using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace asshcii.ecs
{
    public abstract class Entity
    {
        public string Name { get; }
        private List<IComponent> components;

        protected Entity(string name)
        {
            Name = name;
            components = new List<IComponent>();
        }

        public IComponent GetComponent(Type type)
        {
            return components.Single(cmpt => cmpt.GetType().Equals(type));
        }

        public T GetComponent<T>() where T : IComponent
        {
            return components.OfType<T>().Single();
        }

        public IEnumerable<T> GetComponents<T>() where T : IComponent
        {
            return components.OfType<T>();
        }

        public void AddComponent(IComponent component)
        {
            var type = component.GetType();
            if (components.Exists(cmpt => cmpt.GetType().Equals(type)))
            {
                throw new ArgumentException($"The component {nameof(type)} is already present.");
            }

            components.Add(component);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"Name: {Name}");

            if (components.Any())
            {
                stringBuilder.Append(", ");
            }

            foreach (IComponent component in components)
            {
                stringBuilder.Append($"{{ {component.ToString()} }}");

                if (component != components.Last())
                {
                    stringBuilder.Append(", ");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
