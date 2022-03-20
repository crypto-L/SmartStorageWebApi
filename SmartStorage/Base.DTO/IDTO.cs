namespace Base.DTO;

public interface IDTO<TKey> where TKey : IDTO<TKey>
{
    static abstract TKey Contract(TKey other);
}