using Liv_in_paris.Core.Graph;

namespace Liv_in_paris.Core;

class Program
{
    // Ici est éxécuté uniquement le projet .Core
    static void Main(string[] args)
    {
        Graphe<Station> graphe = new Graphe<Station>();
        string onglet_1 = "../../../../Files/MetroParis_test_onglet1.csv";
        string onglet_2 = "../../../../Files/MetroParis_test_onglet2.csv";
        
        Dictionary<string, List<int>> stations = new Dictionary<string, List<int>>();

        using (StreamReader sr = new StreamReader(onglet_1))
        {
            //TODO : Riques si pas le bon format 
            sr.ReadLine();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] values = line.Split(';');
                
                int id = values[0] is "" ? 0 : int.Parse(values[0]);
                if (id > 0)
                {
                    Noeud<Station> noeud = graphe.ajouterNoeud(new Noeud<Station>(id));
                    Station station = noeud.Data;
                    
                    station.Nom = values[2];
                    station.Ligne = values[1];
                    station.Longitude = values[3] is "" ? 0 : float.Parse(values[3].Replace('.',','));
                    station.Latitude = values[4] is "" ? 0 : float.Parse(values[4].Replace('.',','));
                    station.Commune = values[5];
                    station.Insee = values[6];

                    if (!stations.ContainsKey(station.Nom)) 
                        stations.Add(station.Nom, new List<int>());
                    stations[station.Nom].Add(id);
                }
            }
        }
        
        
        using (StreamReader reader = new StreamReader(onglet_2))
        {
            
            //TODO : Riques si pas le bon format 
            reader.ReadLine();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // Traiter chaque ligne
                Console.WriteLine(line);
                string[] values = line.Split(';');
                
                int id = int.Parse(values[0]);
                int precedent = values[2] is "" ? 0 : int.Parse(values[2]);
                int suivant = values[3] is "" ? 0 : int.Parse(values[3]);
                int temps = int.Parse(values[4]);
                int changement = values[5] is "" ? 0 : int.Parse(values[5]);

                Noeud<Station> noeud = graphe.ajouterNoeud(new Noeud<Station>(id));

                foreach (int station in stations[noeud.Data.Nom])
                {
                    if (station != id)
                    {
                        Noeud<Station> meme_station = graphe.ajouterNoeud(new Noeud<Station>(station));
                        graphe.ajouterLien(new Lien<Station>(noeud,changement,meme_station));
                    }
                }

                
                if (suivant > 0)
                {
                    Noeud<Station> noeud_suivant = graphe.ajouterNoeud(new Noeud<Station>(suivant));
                    graphe.ajouterLien(new Lien<Station>(noeud,temps,noeud_suivant));
                }

                if (precedent > 0)
                {
                    Noeud<Station> noeud_precedent = graphe.ajouterNoeud(new Noeud<Station>(precedent));
                    graphe.ajouterLien(new Lien<Station>(noeud,temps,noeud_precedent));
                }
                
            }
        }
        
        Console.WriteLine("Hello, World!");
        
        graphe.AfficherListeAdjacence();
        graphe.AfficherMatriceAdjacence();
        
        
    }

    class Station
    {
        public string Nom { get; set; }
        public string Ligne { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public string Commune { get; set; }
        public string Insee { get; set; }
    }
}