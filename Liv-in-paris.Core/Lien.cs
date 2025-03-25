namespace Liv_in_paris.Core;

public class Lien<T> where T : new()
{
    private readonly Noeud<T> _origine;
    private readonly Noeud<T> _destination;
    private readonly int _poids;

    public Noeud<T> Origine => _origine;
    public Noeud<T> Destination => _destination;
    public int Poids => _poids;
    
    public Lien(Noeud<T> origine, int poids, Noeud<T> destination)
    {
        _origine = origine;
        _destination = destination;
        _poids = poids;
    }

    public override string ToString()
    {
        return $"{_origine} --({_poids})--> {_destination}";
    }
    
}