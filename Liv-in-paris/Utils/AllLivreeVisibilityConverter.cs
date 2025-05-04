using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Liv_in_paris.Core.Models;

namespace Liv_in_paris
{
    /// <summary>
    /// Convertisseur utilisé pour afficher ou masquer un élément en fonction du statut de toutes les lignes de commande.
    /// L'élément est visible uniquement si toutes les lignes sont marquées comme "Livree".
    /// </summary>
    public class AllLivreeVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convertit une collection de <see cref="LigneCommande"/> en une visibilité.
        /// </summary>
        /// <param name="value">La valeur transmise, attendue comme un <see cref="IEnumerable"/> de <see cref="LigneCommande"/>.</param>
        /// <param name="targetType">Le type cible de la liaison (ici <see cref="Visibility"/>).</param>
        /// <param name="parameter">Paramètre facultatif.</param>
        /// <param name="culture">Culture à utiliser dans la conversion.</param>
        /// <returns><see cref="Visibility.Visible"/> si toutes les lignes ont le statut "Livree", sinon <see cref="Visibility.Collapsed"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable lignes)
            {
                var allLivree = lignes.Cast<LigneCommande>().All(l => l.Statut == "Livree");
                return allLivree ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        /// <summary>
        /// Non implémenté. La conversion inverse n'est pas nécessaire pour ce convertisseur.
        /// </summary>
        /// <param name="value">Valeur à convertir en retour.</param>
        /// <param name="targetType">Type cible.</param>
        /// <param name="parameter">Paramètre facultatif.</param>
        /// <param name="culture">Culture utilisée.</param>
        /// <returns>Exception levée dans tous les cas.</returns>
        /// <exception cref="NotImplementedException">Toujours levée.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}