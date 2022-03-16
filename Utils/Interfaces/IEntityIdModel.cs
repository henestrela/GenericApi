namespace Utils.Interfaces
{
    public interface IEntityIdModel<T> : IEntity
    {
        public T Id { get; set; }
    }
}