using Liv_in_paris.Core.Graph;
using Liv_in_paris.Core.Entities;

namespace Liv_in_paris.Core.Services;

/// <summary>
/// Fournit des méthodes utilitaires pour construire un graphe de stations de métro
/// à partir de fichiers CSV.
/// </summary>
public class GrapheMetroBuilder
{
    /// <summary>
    /// Construit un graphe de stations de métro en lisant deux fichiers CSV :
    /// un pour les stations et un pour les connexions entre elles.
    /// </summary>
    /// <param name="cheminOnglet1">
    /// Chemin vers le fichier CSV contenant les informations de chaque station (ID, ligne, nom, coordonnées, etc.).
    /// </param>
    /// <param name="cheminOnglet2">
    /// Chemin vers le fichier CSV contenant les relations entre stations (suivant, précédent, temps, etc.).
    /// </param>
    /// <returns>
    /// Un objet <see cref="Graphe{Station}"/> représentant le réseau de stations construit.
    /// </returns>
    public static Graphe<Station> ConstruireDepuisCSV(string cheminOnglet1, string cheminOnglet2)
    {
        Graphe<Station> graphe = new Graphe<Station>();
        Dictionary<string, List<int>> stations = new Dictionary<string, List<int>>();
        
        using (StreamReader sr = new StreamReader(cheminOnglet1))
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
                    station.Longitude = values[3] is "" ? 0 : double.Parse(values[3].Replace('.',','));
                    station.Latitude = values[4] is "" ? 0 : double.Parse(values[4].Replace('.',','));
                    station.Commune = values[5];
                    station.Insee = values[6];

                    if (!stations.ContainsKey(station.Nom)) 
                        stations.Add(station.Nom, new List<int>());
                    stations[station.Nom].Add(id);
                }
            }
        }
        
        
        using (StreamReader reader = new StreamReader(cheminOnglet2))
        {
            
            //TODO : Riques si pas le bon format 
            reader.ReadLine();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
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
        
        return graphe;
    }
}