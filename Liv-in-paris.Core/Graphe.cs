namespace Rendu1;

public class Graphe
{
    private Dictionary<int, List<int>> listeAdjacence = new();
    private bool[,] matriceAdjacence;
    private int nbSommets;

    /// <summary>
    /// Constructeur du graphe.
    /// Initialise la matrice et la liste d'adjacence.
    /// </summary>
    /// <param name="nbSommets">Nombre de sommets du graphe</param>
    public Graphe(int nbSommets)
    {
        this.nbSommets = nbSommets;
        matriceAdjacence = new bool[nbSommets, nbSommets];

        for (int i = 0; i < nbSommets; i++)
        {
            listeAdjacence[i] = new List<int>(); ///création d'une liste vide pour chaque sommet
        }
        
    }
    
    /// <summary>
    /// Obtient la liste d'adjacence du graphe.
    /// </summary>
    public Dictionary<int, List<int>> ListeAdjacence { get => listeAdjacence; }
    /// <summary>
    /// Obtient le nombre total de sommets dans le graphe.
    /// </summary>
    public int NbSommets { get => nbSommets; }
    /// <summary>
    /// Obtient la matrice d'adjacence du graphe.
    /// </summary>
    public bool[,] MatriceAdjacence { get => matriceAdjacence; }

    /// <summary>
    /// Ajoute une arête entre deux sommets dans la liste et la matrice d'adjacence.
    /// </summary>
    /// <param name="a">Premier sommet</param>
    /// <param name="b">Deuxième sommet</param>
    public void AjouterLien(int a, int b)
    {
        ///Pour la liste d'adjancence 
        listeAdjacence[a].Add(b);
        listeAdjacence[b].Add(a);
        
        ///Pour ne matrice d'adjacences 
        matriceAdjacence[a, b] = true;
        matriceAdjacence[b, a] = true;
    }

    /// <summary>
    /// Affiche la liste d'adjacence du graphe.
    /// </summary>
    public void AfficherListeAdjacence()
    {
        foreach (var VARIABLE in listeAdjacence)
        {
            Console.WriteLine($"{VARIABLE.Key} -> {string.Join(", ", VARIABLE.Value)}");
        }
    }

    /// <summary>
    /// Affiche la matrice d'adjacence du graphe.
    /// </summary>
    public void AfficherMatriceAdjacence()
    {
        Console.Write("   "); /// Espacement pour l'en-tête
        for (int i = 0; i < nbSommets; i++)
            Console.Write($"{i,2} "); /// Afficher les indices des colonnes
        Console.WriteLine("\n  " + new string('-', nbSommets * 3));

        for (int i = 0; i < nbSommets; i++)
        {
            Console.Write($"{i,2} | "); /// Afficher l'indice du sommet
            for (int j = 0; j < nbSommets; j++)
            {
                Console.Write(matriceAdjacence[i, j] ? "1 " : "0 "); 
            }
            Console.WriteLine();
        }
    }
    
    /// <summary>
    /// Parcours en profondeur récursif.
    /// </summary>
    /// <param name="sommet">Sommet de départ</param>
    /// <param name="visite">Tableau des sommets visités</param>
    public void ParcoursEnProfondeurRec(int sommet, bool[] visite)
    {
        Console.Write(sommet + " ");
        visite[sommet] = true;
        Stack<int> pile = new Stack<int>();
        foreach(int voisin in listeAdjacence[sommet])
        {
            if (visite[voisin] == false)
            {
                ParcoursEnProfondeurRec(voisin, visite);
            }
        }
    }
    
    /// <summary>
    /// Lance un parcours en profondeur à partir d'un sommet donné.
    /// </summary>
    /// <param name="sommet">Sommet de départ</param>
    public void ParcoursEnProfondeur(int sommet)
    {
        int max = nbSommets;
        bool[] visite = new bool[max];
        Console.WriteLine("Parcours en Profondeur : ");
        Console.Write("[");
        ParcoursEnProfondeurRec(sommet, visite);
        Console.WriteLine("]");
    }
    
    /// <summary>
    /// Effectue un parcours en largeur (BFS) à partir d'un sommet donné.
    /// </summary>
    /// <param name="depart">Sommet de départ</param>
    public void ParcoursEnLargeur(int depart)
    {
        bool[] visited = new bool[nbSommets];
        Queue<int> file = new Queue<int>(); 

        file.Enqueue(depart);
        visited[depart] = true;

        Console.WriteLine("Parcours en Largeur : ");
        Console.Write("[");

        while (file.Count > 0)
        {
            int sommet = file.Dequeue();
            Console.Write(sommet + " ");

            foreach (int voisin in listeAdjacence[sommet])
            {
                if (visited[voisin]==false)
                {
                    file.Enqueue(voisin);
                    visited[voisin] = true;
                }
            }
        }
        Console.WriteLine("]");
    }
    
    /// <summary>
    /// Vérifie si le graphe est connexe (tous les sommets sont atteignables).
    /// </summary>
    /// <returns>True si connexe, False sinon</returns>
    public bool EstConnexe()
    {
        bool[] visite = new bool[nbSommets];
        int pointDepart = 0;
        
        ParcoursEnProfondeurRec(pointDepart, visite);
        
        for (int i = 0; i < nbSommets; i++)
        {
            if (!visite[i]) return false;
        }
        return true;
    }
    
    /// <summary>
    /// Vérifie si le graphe contient un cycle.
    /// </summary>
    /// <returns>True si un cycle est détecté, False sinon</returns>
    public bool ContientCycle()
    {
        bool[] visite = new bool[nbSommets];
        
        for (int sommet = 0; sommet < nbSommets; sommet++)
        {
            if (!visite[sommet])
            {
                if (DFSDetecterCycle(sommet, visite, -1))
                    return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Détection de cycle avec un parcours en profondeur (DFS).
    /// </summary>
    /// <param name="sommet">Sommet courant</param>
    /// <param name="visite">Tableau des sommets visités</param>
    /// <param name="parent">Sommet parent pour éviter de faux cycles</param>
    /// <returns>True si un cycle est détecté, False sinon</returns>
    private bool DFSDetecterCycle(int sommet, bool[] visite, int parent)
    {
        visite[sommet] = true;

        foreach (int voisin in listeAdjacence[sommet])
        {
            if (!visite[voisin])
            {
                if (DFSDetecterCycle(voisin, visite, sommet))
                    return true;
            }
            else if (voisin != parent)
            {
                return true;
            }
        }
        return false;
    }
}