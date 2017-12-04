using System;

namespace asshcii.game.resources
{
    public interface IResource : IEquatable<IResource>
    {
        string Name { get; }
    }

    public abstract class BaseResource : IResource
    {
        public abstract string Name { get; }

        public abstract bool Equals(IResource other);
        public override bool Equals(object o)
        {
            return Equals(o as IResource);
        }
        public abstract override int GetHashCode();

        public static bool operator ==(BaseResource self, IResource other)
        {
            throw new Exception("Use .Equals instead");
        }
        public static bool operator !=(BaseResource self, IResource other)
        {
            throw new Exception("Use !.Equals instead");
        }

    }

    public class PowerResource : BaseResource
    {
        public override string Name => "Power";

        public override bool Equals(IResource other)
        {
            return other is PowerResource;
        }
        public override int GetHashCode() { return 1; }
    }

    public class IronResource : BaseResource
    {
        public override string Name => "Iron";

        public override bool Equals(IResource other)
        {
            return other is IronResource;
        }
        public override int GetHashCode() { return 2; }
    }
}