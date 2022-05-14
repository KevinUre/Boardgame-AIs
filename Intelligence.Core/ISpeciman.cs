namespace Intelligence.Core
{
    public interface ISpeciman { 
        Guid ID { get; }

    }

    public interface IGene<T>
    {
        T Value { get; set; }
    }