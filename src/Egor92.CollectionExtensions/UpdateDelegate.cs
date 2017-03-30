namespace Egor92.CollectionExtensions
{
    public delegate void UpdateDelegate<in TTarget, in TSource>(TTarget target, TSource source);
}
