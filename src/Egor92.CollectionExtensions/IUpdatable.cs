namespace Egor92.CollectionExtensions
{
    public interface IUpdatable<in T>
    {
        void Update(T source);
    }
}
