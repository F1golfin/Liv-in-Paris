using System.Globalization;
using System.Windows.Data;

namespace Liv_in_paris
{
    /// <summary>
    /// Convertisseur qui transforme une liste de chaînes en une chaîne unique séparée par des virgules.
    /// </summary>
    public class ListeToStringConverter : IValueConverter
    {
        /// <summary>
        /// Convertit une <see cref="List{T}"/> de chaînes en une seule chaîne séparée par des virgules.
        /// </summary>
        /// <param name="value">Valeur à convertir, attendue comme <see cref="List{String}"/>.</param>
        /// <param name="targetType">Type cible de la liaison (normalement <see cref="string"/>).</param>
        /// <param name="parameter">Paramètre optionnel (non utilisé).</param>
        /// <param name="culture">Culture utilisée pour la conversion.</param>
        /// <returns>Une chaîne contenant les éléments de la liste séparés par ", ", ou une chaîne vide si la conversion échoue.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<string> list)
                return string.Join(", ", list);
            return "";
        }

        /// <summary>
        /// Non implémenté. La conversion inverse (string vers List&lt;string&gt;) n'est pas supportée.
        /// </summary>
        /// <param name="value">Valeur à convertir.</param>
        /// <param name="targetType">Type cible.</param>
        /// <param name="parameter">Paramètre optionnel.</param>
        /// <param name="culture">Culture utilisée.</param>
        /// <returns>Exception levée dans tous les cas.</returns>
        /// <exception cref="NotImplementedException">Toujours levée.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}