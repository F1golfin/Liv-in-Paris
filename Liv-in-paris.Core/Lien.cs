namespace Liv_in_paris.Core;

public class Lien
{
    private Noeud noeud1;
    private Noeud noeud2;

    public Lien(Noeud noeud1, Noeud noeud2)
    {
        this.noeud1 = noeud1;
        this.noeud2 = noeud2;
    }
    
    public Noeud Noeud1 { get => noeud1;}
    public Noeud Noeud2 { get => noeud2;}
    
}