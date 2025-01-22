using MediatR;
using KafkaSample.Domain.SeedWork;

namespace KafkaSample.Domain.SeedWork;

public abstract class Entity<T> : BaseEntity where T : struct 
{
    int? _requestedHashCode;
    T _Id;
    public virtual T Id
    {
        get
        {
            return _Id;
        }
        protected set
        {
            _Id = value;
        }
    }

    public bool IsTransient()
    {
        return this.Id.Equals(default(T));
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Entity<T>))
            return false;

        if (Object.ReferenceEquals(this, obj))
            return true;

        if (this.GetType() != obj.GetType())
            return false;

        Entity<T> item = (Entity<T>)obj;

        if (item.IsTransient() || this.IsTransient())
            return false;
        else
            return item.Id.Equals(this.Id);
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

            return _requestedHashCode.Value;
        }
        else
            return base.GetHashCode();

    }
    public static bool operator == (Entity<T> left, Entity<T> right)
    {
        if (Object.Equals(left, null))
            return (Object.Equals(right, null)) ? true : false;
        else
            return left.Equals(right);
    }

    public static bool operator !=(Entity<T> left, Entity<T> right)
    {
        return !(left == right);
    }
}
