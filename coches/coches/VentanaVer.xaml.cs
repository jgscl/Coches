using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;


namespace coches
{
    /// <summary>
    /// Lógica de interacción para VentanaVer.xaml
    /// </summary>
    /// 

    /*Clase derivada de EventArgs que contiene un dato Coche*/
    /*Se utiliza para poder mostrar graficamente en la ventana principal 
     * los datos de un coche seleccionado en el listView */
    public class CambioSeleccionEventArgs : EventArgs
    {
        public Coche cocheSeleccionado { get; set; }

        public CambioSeleccionEventArgs(Coche c)
        {
            cocheSeleccionado = c;
        }
    }

    public partial class VentanaVer : Window
    {
        ObservableCollection<Coche> coches = null;
        public event EventHandler<CambioSeleccionEventArgs> cambioSeleccion;

        public VentanaVer(ObservableCollection<Coche> c)
        {
            InitializeComponent();

            this.coches = c;

            //Al inicializar la ventana nos aseguramos listaCoches lea elementos de la lista coches
            listaCoche.ItemsSource = coches;
            
        }

        /*Cada vez que cambia el elemento seleccionado del ListView principal, se muestra la lista de
         paradas de repostaje de dicho coche en el segundo ListView y se indica que coche ha sido seleccionado
         como evento, para que se dibuje su gráfico en la ventana principal*/
        private void listaCoche_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaCoche.SelectedItem != null)
            {
                listaRepostaje.ItemsSource = ((Coche)listaCoche.SelectedItem).paradas;

                CambioSeleccionEventArgs esea = new CambioSeleccionEventArgs((Coche)listaCoche.SelectedItem);
                OnCambioSeleccion(esea);
            }
        }

        /*Antes de activar el evento cambioSeleccion nos aseguramos que tiene un controlador de evento
         asociado*/
        private void OnCambioSeleccion(CambioSeleccionEventArgs e)
        {
            if (cambioSeleccion != null)
                cambioSeleccion(this, e);
        }
    }
}
