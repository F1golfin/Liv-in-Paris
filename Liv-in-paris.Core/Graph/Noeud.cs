namespace Liv_in_paris.Core.Graph;

public class Noeud<T> where T : new()
{
    private readonly int _id;
    private T _data;

    public int Id => _id;
    public T Data => _data;
    
    public Noeud(int id)
    {
        _id = id;
        _data = new T();
    }

    public override string ToString()
    {
        if (typeof(T).GetMethod("ToString") != null && _data != null)
        {
            return $"{_id} [{_data.ToString()}]";
        }
        else
        {
            return $"{_id}";
        }
    }
}
