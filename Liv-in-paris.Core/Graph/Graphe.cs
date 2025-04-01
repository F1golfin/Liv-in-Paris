using Liv_in_paris.Core.Entities;

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
        Console.WriteLine(lien.ToString()); //TODO : A enlever c'est pour débug
        
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
    public List<int> ParcoursEnProfondeur(int noeud, bool[] visites, List<int> chemin)
    {
        visites[noeud] = true;
        chemin.Add(noeud);
        foreach(var voisin in _matrice[noeud].Keys)
        {
            if (!visites[voisin])
            {
                ParcoursEnProfondeur(voisin, visites, chemin);
            }
        }
        return chemin;
    }
    public List<int> InitParcoursEnProfondeur(int noeud)
    {
        bool[] visites = new bool[_noeuds.Count];
        List<int> chemin = new List<int>();
        return ParcoursEnProfondeur(noeud, visites, chemin);
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

    public void AfficheListe(List<int> list)
    {
        foreach(int n in list)
        {
            Console.Write(n + " ");
        }
    }
    #endregion

    #region Analyse du graphe
    
    /// <summary>
    /// Détermine si le graphe est connexe.
    /// </summary>
    /// <returns>True si connexe, sinon false.</returns>
    public bool EstFaiblementConnexe()
    {
        bool FaConnexe = true;
        if(!(ParcoursEnProfondeur(1, new bool[_noeuds.Count], new List<int>()).Count==_noeuds.Count))
        {
            FaConnexe = false;
        }
        return FaConnexe;
        
        throw new NotImplementedException();
    }
    public bool EstFortementConnexe()
    {
        bool[] visite = new bool[_noeuds.Count];
        ParcoursEnProfondeur(1, visite, new List<int>());
        if(Array.Exists(visite, v => false))
        {
            return false;
        }
        Graphe<T> g = Inverser();
        Array.Fill(visite, false);
        g.ParcoursEnProfondeur(1, visite, new List<int>());
        if(Array.Exists(visite, v=> false))
        {
            return false;
        }
        return true;
    }
    public Graphe<T> Inverser()
    {
        Graphe<T> g = new Graphe<T>();
        g._noeuds = _noeuds;
        Dictionary<int, List<int>> dico = new Dictionary<int, List<int>>();
        for (int i =0; i<_noeuds.Count; i++)
        {
            foreach(var n in _matrice[i].Keys)
            {
                if (!dico.ContainsKey(n))
                {
                    dico.Add(n, new List<int>());
                }
                dico[n].Add(i);
            }
        }
        foreach(int i in dico.Keys)
        {
            Dictionary<int,int> succ = new Dictionary<int,int>();
            foreach(int j in dico[i])
            {
                succ.Add(j, _matrice[j][i]);
            }
            g._matrice.Add(i, succ);
        }
        return g;
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
    public List<int> Dijkstra(int debut, int fin)
    {
        int itération = 1;
        int[,] dist = new int[_noeuds.Count + 1, _noeuds.Count + 1];
        for (int i = 1; i <= _noeuds.Count; i++)  
        {
            for (int j = 1; j <= _noeuds.Count; j++)
            {
                dist[i, j] = int.MaxValue; 
            }
        }
        for (int i = 1; i <= _noeuds.Count; i++)
        {
            dist[i, i] = 0; 
        }

        int[,] pred = new int[_noeuds.Count + 1, _noeuds.Count + 1];
        for (int i = 1; i <= _noeuds.Count; i++) 
        {
            pred[1, i] = -1;
        }
        return DijkstraRec(debut, fin, itération+1, new List<int>(), dist, pred);


    }
    public List<int> DijkstraRec(int noeud, int fin, int itération, List<int> list, int[,] dist, int[,] pred)
    {
        list.Add(noeud);
        if (noeud == fin)
        {
            return list;
        }
        if (list.Count > 0 && noeud == list[0] && !_matrice.ContainsKey(noeud))
        {
            return new List<int>(); 
        }
        for (int i = 1; i <= _noeuds.Count; i++)
        {
            if (list.Contains(i))
            {
                dist[itération, i] = int.MaxValue;
            }
            else if (_matrice[noeud].Keys.Contains(i))
            {
                if (dist[itération - 1, noeud] + _matrice[noeud][i] < dist[itération - 1, i] || dist[itération - 1, i] == int.MaxValue)
                {
                    dist[itération, i] = dist[itération - 1, noeud] + _matrice[noeud][i];
                    pred[itération, i] = noeud;
                }
                else
                {
                    dist[itération, i] = dist[itération - 1, i];
                    pred[itération, i] = pred[itération - 1, i];
                }
            }
            else
            {
                dist[itération, i] = dist[itération - 1, i];
                pred[itération, i] = pred[itération - 1, i];
            }
        }
        int min = int.MaxValue;
        int it = -1;

        for (int i = 1; i <= _noeuds.Count; i++)  
        {
            if (dist[itération, i] < min && !list.Contains(i))
            {
                min = dist[itération, i];
                it = i;
            }
        }

        if (it == -1) 
        {
            Console.WriteLine("Aucun chemin trouvé !");
            return new List<int>();
        }
        else
        {
            return DijkstraRec(it, fin, itération+1, list, dist, pred);

        }
    }

    public List<int> BellmanFord(int source, int destination)
    {
        var distances = new Dictionary<int, int>();
        var predecesseurs = new Dictionary<int, int>();
        foreach (var noeud in _noeuds.Values)
        {
            distances[noeud.Id] = int.MaxValue; 
            predecesseurs[noeud.Id] = -1;
        }
        distances[source] = 0; 

        for (int i = 1; i < _noeuds.Count; i++)  
        {
            foreach (var origine in _matrice)
            {
                foreach (var destinationNoeud in origine.Value)
                {
                    int u = origine.Key;
                    int v = destinationNoeud.Key;
                    int poids = destinationNoeud.Value;

                    if (distances[u] != int.MaxValue && distances[u] + poids < distances[v])
                    {
                        distances[v] = distances[u] + poids;
                        predecesseurs[v] = u;
                    }
                }
            }
        }

        foreach (var origine in _matrice)
        {
            foreach (var destinationNoeud in origine.Value)
            {
                int u = origine.Key;
                int v = destinationNoeud.Key;
                int poids = destinationNoeud.Value;

                if (distances[u] != int.MaxValue && distances[u] + poids < distances[v])
                {
                    Console.WriteLine("Le graphe contient un cycle négatif.");
                    return null; 
                }
            }
        }

        List<int> chemin = new List<int>();
        for (int v = destination; v != -1; v = predecesseurs[v])
        {
            chemin.Add(v);
        }

        chemin.Reverse();  
        if (chemin[0] != source)
        {
            Console.WriteLine("Aucun chemin trouvé entre la source et la destination.");
            return new List<int>();
        }

        return chemin;
    }

    public Dictionary<int, Dictionary<int, int>> FloydWarshall()
    {
        var dist = new Dictionary<int, Dictionary<int, int>>();
        var pred = new Dictionary<int, Dictionary<int, int>>();

        foreach (var noeud in _noeuds.Keys)
        {
            dist[noeud] = new Dictionary<int, int>();
            pred[noeud] = new Dictionary<int, int>();
            foreach (var autreNoeud in _noeuds.Keys)
            {
                if (noeud == autreNoeud)
                {
                    dist[noeud][autreNoeud] = 0; 
                    pred[noeud][autreNoeud] = -1;
                }
                else if (_matrice.ContainsKey(noeud) && _matrice[noeud].ContainsKey(autreNoeud))
                {
                    dist[noeud][autreNoeud] = _matrice[noeud][autreNoeud]; 
                    pred[noeud][autreNoeud] = noeud;
                }
                else
                {
                    dist[noeud][autreNoeud] = int.MaxValue;
                    pred[noeud][autreNoeud] = -1;
                }
            }
        }

        foreach (var k in _noeuds.Keys) 
        {
            foreach (var i in _noeuds.Keys) 
            {
                foreach (var j in _noeuds.Keys) 
                {
                    if (dist[i][k] != int.MaxValue && dist[k][j] != int.MaxValue && dist[i][j] > dist[i][k] + dist[k][j])
                    {
                        dist[i][j] = dist[i][k] + dist[k][j];
                        pred[i][j] = pred[k][j]; 
                    }
                }
            }
        }

        return dist; 
    }

    public List<int> CheminLePlusCourt(int source, int destination)
    {
        var dist = FloydWarshall();
        var pred = new Dictionary<int, Dictionary<int, int>>();

        foreach (var noeud in _noeuds.Keys)
        {
            pred[noeud] = new Dictionary<int, int>();
            foreach (var autreNoeud in _noeuds.Keys)
            {
                pred[noeud][autreNoeud] = -1;
            }
        }

        if (dist[source][destination] == int.MaxValue)
        {
            Console.WriteLine("Aucun chemin disponible entre la source et la destination.");
            return new List<int>(); 
        }

        List<int> chemin = new List<int>();
        int courant = destination;

        while (courant != source)
        {
            chemin.Add(courant);
            courant = pred[source][courant]; 
            if (courant == -1) 
            {
                Console.WriteLine("Aucun chemin trouvé.");
                return new List<int>(); 
            }
        }

        chemin.Add(source);
        chemin.Reverse();

        return chemin;
    }
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