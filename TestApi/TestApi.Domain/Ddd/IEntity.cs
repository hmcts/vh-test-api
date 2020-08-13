namespace TestApi.Domain.Ddd
{
    public interface IEntity<out TKey>
    {
        TKey Id { get; }
    }
}