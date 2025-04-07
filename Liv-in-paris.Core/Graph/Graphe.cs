
namespace Liv_in_paris.Core.Graph;

/// <summary>
/// Représente un graphe orienté et pondéré générique avec gestion des nœuds et des liens.
/// </summary>
/// <typeparam name="T">Le type de données contenues dans chaque nœud. Ce type doit avoir un constructeur par défaut.</typeparam>
public class Graphe<T> where T : new()
{

    private Dictionary<int, Noeud<T>> _noeuds = new Dictionary<int, Noeud<T>>();
    private Dictionary<int, Dictionary<int, int>> _matrice = new Dictionary<int, Dictionary<int, int>>();

    public Dictionary<int, Noeud<T>> Noeuds => _noeuds;

    /// <summary>
    /// Liste de tous les liens du graphe (calculée à partir de la matrice).
    /// </summary>
    public List<Lien<T>> Liens
    {
        get
        {
            var liens = new List<Lien<T>>();
            foreach (var origine in _matrice)
            {
                foreach (var destination in origine.Value)
                {
                    var noeudDepart = _noeuds[origine.Key];
                    var noeudArrivee = _noeuds[destination.Key];
                    int poids = destination.Value;

                    liens.Add(new Lien<T>(noeudDepart, poids, noeudArrivee));
                }
            }

            return liens;
        }
    }

    /// <summary>
    /// Calcule la somme des poids d’un chemin donné.
    /// </summary>
    /// <param name="chemin">Liste des identifiants des nœuds à suivre.</param>
    /// <returns>Poids total du chemin.</returns>
    public int CalculerPoids(List<int> chemin)
    {
        int total = 0;
        for (int i = 0; i < chemin.Count - 1; i++)
        {
            int a = chemin[i], b = chemin[i + 1];
            if (_matrice.ContainsKey(a) && _matrice[a].ContainsKey(b))
                total += _matrice[a][b];
        }
        return total;
    }

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

        _matrice[origine.Id].Add(destination.Id, lien.Poids);
    }

    #endregion

    #region Algorithmes de parcours

    /// <summary>
    /// Lance un parcours en profondeur à partir d’un nœud donné.
    /// </summary>
    /// <param name="noeud">Identifiant du nœud de départ.</param>
    /// <param name="visites">Tableau des nœuds visités.</param>
    /// <param name="chemin">Chemin en cours de construction.</param>
    /// <returns>Liste des identifiants des nœuds visités.</returns>
    public List<int> ParcoursEnProfondeur(int noeud, bool[] visites, List<int> chemin)
    {
        visites[noeud] = true;
        chemin.Add(noeud);
        foreach (var voisin in _matrice[noeud].Keys)
        {
            if (!visites[voisin])
            {
                ParcoursEnProfondeur(voisin, visites, chemin);
            }
        }

        return chemin;
    }

    /// <summary>
    /// Initialise un parcours en profondeur à partir d’un nœud donné.
    /// </summary>
    /// <param name="noeud">Identifiant du nœud de départ.</param>
    /// <returns>Liste des identifiants des nœuds visités.</returns>
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

    /// <summary>
    /// Affiche dans la console une liste d’identifiants de nœuds.
    /// </summary>
    /// <param name="list">Liste à afficher.</param>
    public void AfficheListe(List<int> list)
    {
        foreach (int n in list)
        {
            Console.Write(n + " ");
        }
    }

    #endregion

    #region Analyse du graphe

    /// <summary>
    /// Détermine si le graphe est faiblement connexe (tous les nœuds atteignables sans tenir compte du sens des arcs).
    /// </summary>
    /// <returns>True si le graphe est faiblement connexe, sinon false.</returns>
    public bool EstFaiblementConnexe()
    {
        bool FaConnexe = true;
        if (!(ParcoursEnProfondeur(1, new bool[_noeuds.Count], new List<int>()).Count == _noeuds.Count))
        {
            FaConnexe = false;
        }

        return FaConnexe;

        throw new NotImplementedException();
    }

    /// <summary>
    /// Détermine si le graphe est fortement connexe (tous les nœuds atteignables dans les deux sens).
    /// </summary>
    /// <returns>True si fortement connexe, sinon false.</returns>
    public bool EstFortementConnexe()
    {
        bool[] visite = new bool[_noeuds.Count];
        ParcoursEnProfondeur(1, visite, new List<int>());
        if (Array.Exists(visite, v => false))
        {
            return false;
        }

        Graphe<T> g = Inverser();
        Array.Fill(visite, false);
        g.ParcoursEnProfondeur(1, visite, new List<int>());
        if (Array.Exists(visite, v => false))
        {
            return false;
        }

        return true;
    }
    
    /// <summary>
    /// Crée un graphe avec tous les liens inversés.
    /// </summary>
    /// <returns>Le graphe inversé.</returns>
    public Graphe<T> Inverser()
    {
        Graphe<T> g = new Graphe<T>();
        g._noeuds = _noeuds;
        Dictionary<int, List<int>> dico = new Dictionary<int, List<int>>();
        for (int i = 0; i < _noeuds.Count; i++)
        {
            foreach (var n in _matrice[i].Keys)
            {
                if (!dico.ContainsKey(n))
                {
                    dico.Add(n, new List<int>());
                }

                dico[n].Add(i);
            }
        }

        foreach (int i in dico.Keys)
        {
            Dictionary<int, int> succ = new Dictionary<int, int>();
            foreach (int j in dico[i])
            {
                succ.Add(j, _matrice[j][i]);
            }

            g._matrice.Add(i, succ);
        }

        return g;
    }
    #endregion

    #region Algorithmes de plus court chemin

    /// <summary>
    /// Applique l’algorithme de Dijkstra pour trouver le plus court chemin entre deux nœuds.
    /// </summary>
    /// <param name="debut">Identifiant du nœud de départ.</param>
    /// <param name="fin">Identifiant du nœud d’arrivée.</param>
    /// <returns>Liste des identifiants de nœuds représentant le plus court chemin.</returns>
    public List<int> Dijkstra(int debut, int fin)
    {
        var dist = new Dictionary<int, int>(); // Distance minimale depuis debut
        var pred = new Dictionary<int, int>(); // Stocke les prédécesseurs pour reconstruire le chemin
        var visite = new HashSet<int>(); // Garde une trace des nœuds visités
        var pq = new PriorityQueue<int, int>(); // File de priorité (min-heap)

        // Initialisation
        foreach (var noeud in _noeuds.Keys)
        {
            dist[noeud] = int.MaxValue;
            pred[noeud] = -1; // Aucun prédécesseur au départ
        }

        dist[debut] = 0;
        pq.Enqueue(debut, 0);

        while (pq.Count > 0)
        {
            int noeud = pq.Dequeue(); // Récupère le nœud avec la plus petite distance
            if (noeud == fin) break; // Si on atteint fin, on arrête

            if (!visite.Add(noeud)) continue; // Si déjà visité, on ignore

            if (!_matrice.ContainsKey(noeud)) continue; // Pas de voisins

            foreach (var voisin in _matrice[noeud])
            {
                int voisinId = voisin.Key;
                int poids = voisin.Value;
                int nouvelleDist = dist[noeud] + poids;

                if (nouvelleDist < dist[voisinId])
                {
                    dist[voisinId] = nouvelleDist;
                    pred[voisinId] = noeud;
                    pq.Enqueue(voisinId, nouvelleDist);
                }
            }
        }

        // Reconstruire le chemin si possible
        if (dist[fin] == int.MaxValue)
            return new List<int>(); // Aucun chemin trouvé

        List<int> chemin = new();
        for (int at = fin; at != -1; at = pred[at])
        {
            chemin.Add(at);
        }

        chemin.Reverse();
        return chemin;
    }

    /// <summary>
    /// Applique l’algorithme de Bellman-Ford pour trouver le plus court chemin.
    /// Gère les poids négatifs mais pas les cycles négatifs.
    /// </summary>
    /// <param name="source">Nœud de départ.</param>
    /// <param name="destination">Nœud d’arrivée.</param>
    /// <returns>Liste des identifiants du chemin, ou null s’il y a un cycle négatif.</returns>
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

    /// <summary>
    /// Applique l’algorithme de Floyd-Warshall pour calculer toutes les plus courtes distances entre tous les couples de nœuds.
    /// </summary>
    /// <returns>
    /// Une paire de dictionnaires :
    /// - dist[source][destination] donne la distance minimale.
    /// - pred[source][destination] donne le prédécesseur dans le chemin.
    /// </returns>
    public (Dictionary<int, Dictionary<int, int>>, Dictionary<int, Dictionary<int, int>>) FloydWarshall()
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
                    if (dist[i][k] != int.MaxValue && dist[k][j] != int.MaxValue &&
                        dist[i][j] > dist[i][k] + dist[k][j])
                    {
                        dist[i][j] = dist[i][k] + dist[k][j];
                        pred[i][j] = pred[k][j]; // Correction ici : on met à jour le prédécesseur
                    }
                }
            }
        }

        return (dist, pred);

    }
    
    /// <summary>
    /// Renvoie le plus court chemin entre deux nœuds en utilisant Floyd-Warshall.
    /// </summary>
    /// <param name="source">Identifiant du nœud source.</param>
    /// <param name="destination">Identifiant du nœud destination.</param>
    /// <returns>Liste des identifiants du plus court chemin, ou une liste vide si aucun chemin n’existe.</returns>
    public List<int> CheminLePlusCourt(int source, int destination)
    {
        var (dist, pred) = FloydWarshall();

        if (dist[source][destination] == int.MaxValue)
        {
            Console.WriteLine("Aucun chemin disponible entre la source et la destination.");
            return new List<int>();
        }

        List<int> chemin = new List<int>();
        int courant = destination;

        while (courant != source)
        {
            if (courant == -1)
            {
                Console.WriteLine("Aucun chemin trouvé.");
                return new List<int>();
            }

            chemin.Add(courant);
            courant = pred[source][courant];
        }

        chemin.Add(source);
        chemin.Reverse();

        return chemin;
    }


    #endregion

    #region Affichage

    /// <summary>
    /// Affiche la liste d’adjacence du graphe dans la console.
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
    /// Affiche la matrice d’adjacence pondérée du graphe dans la console.
    /// </summary>
    /// <remarks>
    /// L’affichage doit être revu si certains identifiants ne sont pas consécutifs ou commencent à un index élevé.
    /// </remarks>
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