namespace Liv_in_paris.Core.Graph;

/// <summary>
/// Représente un graphe orienté et pondéré générique avec gestion des nœuds et des liens.
/// </summary>
/// <typeparam name="T">Le type de données contenues dans chaque nœud. Ce type doit avoir un constructeur par défaut.</typeparam>
public class Graphe<T> where T : new()
{
    
    private Dictionary<int, Noeud<T>> _noeuds = new Dictionary<int, Noeud<T>>();
    private Dictionary<int, Dictionary<int, int>> _matrice = new Dictionary<int, Dictionary<int, int>>();

    #region Gestion des nœuds et des liens
    
    /// <summary>
    /// Ajoute un nœud au graphe s'il n'existe pas déjà.
    /// </summary>
    /// <param name="noeud">Le nœud à ajouter.</param>
    /// <returns>Le nœud ajouté ou déjà existant.</returns>
    public Noeud<T> ajouterNoeud(Noeud<T> noeud)
    {
        if (_noeuds.ContainsKey(noeud.Id))
            return _noeuds[noeud.Id];
        _noeuds.Add(noeud.Id, noeud);
        return noeud;
    }

    /// <summary>
    /// Ajoute un lien pondéré entre deux nœuds. Lève une exception si le lien existe déjà.
    /// </summary>
    /// <param name="lien">Le lien à ajouter.</param>
    public void ajouterLien(Lien<T> lien)
    {
        Console.WriteLine(lien.ToString());
        
        Noeud<T> origine = lien.Origine;
        Noeud<T> destination = lien.Destination;

        if (!_noeuds.ContainsKey(origine.Id))
            ajouterNoeud(origine);
        if (!_noeuds.ContainsKey(destination.Id))
            ajouterNoeud(destination);
        if (!_matrice.ContainsKey(origine.Id))
            _matrice.Add(origine.Id, new Dictionary<int, int>());
        
        if (_matrice[origine.Id].ContainsKey(destination.Id))
            throw new Exception($"Le lien entre {origine.Id} et {destination.Id} existe déja");
            
        _matrice[origine.Id].Add(destination.Id,lien.Poids);
    }
    #endregion
    
    #region Algorithmes de parcours
    
    /// <summary>
    /// Affiche le parcours en profondeur à partir d’un nœud donné.
    /// </summary>
    /// <param name="noeud">Le nœud de départ.</param>
    public void ParcoursEnProfondeur(Noeud<T> noeud)
    {
        bool[] visite = new bool[_noeuds.Count];
        Console.WriteLine("Parcours en Profondeur : ");
        Console.Write("[");
        ParcoursEnProfondeurRec(noeud.Id, visite);
        Console.WriteLine("]");
    }
    
    private void ParcoursEnProfondeurRec(int id, bool[] visite)
    {
        Console.Write(id + " ");
        visite[id] = true;
        if(_matrice.ContainsKey(id))
        {
            foreach(int voisin in _matrice[id].Keys)
            {
                if (visite[voisin] == false)
                {
                    ParcoursEnProfondeurRec(voisin, visite);
                }
            }
        }
        
    }
    
    /// <summary>
    /// Affiche le parcours en largeur à partir d’un nœud donné.
    /// </summary>
    /// <param name="noeud">Le nœud de départ.</param>
    public void ParcoursEnLargeur(Noeud<T> noeud)
    {
        bool[] visited = new bool[_noeuds.Count];
        Queue<int> file = new Queue<int>();
        int depart = noeud.Id;
        
        file.Enqueue(depart);
        visited[depart] = true;

        Console.WriteLine("Parcours en Largeur : ");
        Console.Write("[");

        while (file.Count > 0)
        {
            int sommet = file.Dequeue();
            Console.Write(sommet + " ");

            if (_matrice.ContainsKey(sommet))
            {
                foreach (int voisin in _matrice[sommet].Keys)
                {
                    if (visited[voisin] == false)
                    {
                        file.Enqueue(voisin);
                        visited[voisin] = true;
                    }
                }
            }
        }
        Console.WriteLine("]");
    }
    #endregion

    #region Analyse du graphe
    
    /// <summary>
    /// Détermine si le graphe est connexe.
    /// </summary>
    /// <returns>True si connexe, sinon false.</returns>
    public bool EstConnexe()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Vérifie si le graphe contient un cycle.
    /// </summary>
    /// <returns>True si un cycle est détecté, sinon false.</returns>
    public bool ContientCycle()
    {
        throw new NotImplementedException();
    }

    private bool DFSDetecterCycle()
    {
        throw new NotImplementedException();
    }
    #endregion
    
    #region Algorithmes de plus court chemin
    //TODO: A implementer
    #endregion
    
    #region Affichage
    
    /// <summary>
    /// Affiche la liste d’adjacence du graphe.
    /// </summary>
    public void AfficherListeAdjacence()
    {
        var keys = _noeuds.Keys;
        foreach (int key in keys)
        {
            var liens = _matrice.ContainsKey(key) ? _matrice[key].Keys : new Dictionary<int, int>().Keys;
            Console.WriteLine($"{key} -> {string.Join(",", liens)}");
        }
    }
    
    /// <summary>
    /// Affiche la matrice d’adjacence du graphe avec les poids.
    /// </summary>
    public void AfficherMatriceAdjacence()
    {
        var keys = _matrice.Keys;
        
        Console.Write("     ");
        foreach (int colonne in keys)
        {
            Console.Write($"{colonne:000} "); //BUG: Revoir l'affichage de la matrice, noeud 1 avec le jeu de tests
        }
        Console.WriteLine();
        
        foreach (int ligne in keys)
        {
            Console.Write($"{ligne:000} :");
            foreach (int colonne in keys)
            {
                if (_matrice.ContainsKey(ligne) && _matrice[ligne].ContainsKey(colonne))
                {
                    Console.Write($"{_matrice[ligne][colonne]:000} ");
                }
                else
                {
                    Console.Write("000 ");
                }
            }
            Console.WriteLine();
        }
    }
    #endregion
}