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
    /// Lógica de interacción para VentanaAnadir.xaml
    /// </summary>
    public partial class VentanaAnadir : Window
    {

        ObservableCollection<Coche> coches = null;
        ObservableCollection<Repostaje> listaRepostaje = null;
       // Coche coche_ = null;

        /*public Coche coche
        {
            get { return this.coche_; }
        }*/

        public VentanaAnadir(ObservableCollection<Coche> l)
        {
            InitializeComponent();

            this.listaRepostaje = new ObservableCollection<Repostaje>();
            this.coches = l;
        }

        /*Para añadir las distintas paradas de respostaje de un coche*/
        private void Button_Click_Anadir(object sender, RoutedEventArgs e)
        {   
           /*Comprobamos que todas las textBox del apartado de repostaje ni esten en rojo,
             aka, tengan valores erroneos, ni esten vacias o con caracteres espacio antes de añadirlas*/
            if(cajaDia.Background != Brushes.Tomato && cajaMes.Background != Brushes.Tomato && cajaAno.Background != Brushes.Tomato
                && cajaKm.Background != Brushes.Tomato && cajaCoste.Background != Brushes.Tomato && cajaLitros.Background != Brushes.Tomato &&
                String.IsNullOrWhiteSpace(cajaDia.Text) == false && String.IsNullOrWhiteSpace(cajaMes.Text) == false
                && String.IsNullOrWhiteSpace(cajaAno.Text) == false && String.IsNullOrWhiteSpace(cajaKm.Text) == false &&
                String.IsNullOrWhiteSpace(cajaCoste.Text) == false && String.IsNullOrWhiteSpace(cajaLitros.Text) == false)
            {
                int dia = int.Parse(cajaDia.Text);
                int mes = int.Parse(cajaMes.Text);
                int ano = int.Parse(cajaAno.Text);
                int km = int.Parse(cajaKm.Text);
                int litros = int.Parse(cajaLitros.Text);
                int coste = int.Parse(cajaCoste.Text);
                int i = 0;
                bool flag = false;

                DateTime fecha = new DateTime(ano, mes, dia);

                Repostaje repostaje = new Repostaje(fecha, km, litros, coste);
                /*Comprobamos que los datos del cuentakilometros son coherentes con las fechas*/
                /*Se comprueba si hay elementos en la lista de repostajes*/
                if (listaRepostaje.Any())
                {
                    /*Con i comprobamos en que posicion colocar el nuevo objeto de repostaje segun
                     la fecha*/
                    i = 0;
                    while (!flag && i < listaRepostaje.Count)
                    {
                        if (0 < DateTime.Compare(listaRepostaje[i].fecha, repostaje.fecha))
                            flag = true;
                        i++;
                    }

                    /*Si se coloca de forma intermedia el objeto, comprobamos que el de la fecha
                     menor tiene un valor del cuentakilometros menor tambien*/
                    if (flag && listaRepostaje[i - 1].km > repostaje.km)
                    {
                        listaRepostaje.Insert(i - 1, repostaje);
                        cajaKm.Background = Brushes.LightPink;
                        cajaDia.Text = cajaMes.Text = cajaAno.Text = cajaKm.Text = cajaLitros.Text = cajaCoste.Text = string.Empty;
                    }
                    else
                    {
                        /*Si el nuevo repostaje introducido tiene la fecha mayor, comprobamos tambien que su km es mayor*/
                        if (listaRepostaje[i - 1].km < repostaje.km && 0 > DateTime.Compare(listaRepostaje[i - 1].fecha, repostaje.fecha))
                        {
                            listaRepostaje.Insert(i, repostaje);
                            cajaDia.Text = cajaMes.Text = cajaAno.Text = cajaKm.Text = cajaLitros.Text = cajaCoste.Text = string.Empty;
                        }
                        //En cualquier otro caso, no es coherente la fecha con el valor del cuentakilometros
                        else
                            cajaKm.Background = Brushes.Tomato;
                    }
                }
                else
                {
                    //Si no hay elementos en la lista se añade directamente
                    listaRepostaje.Add(repostaje);
                    cajaDia.Text = cajaMes.Text = cajaAno.Text = cajaKm.Text = cajaLitros.Text = cajaCoste.Text = string.Empty;
                }
            }
        } 

        /*Guardamos los datos del nuevo coche introducido en la lista de coches*/
        private void Button_Click_OK(object sender, RoutedEventArgs e)
        {
            /* Comprobamos que los textBox de matricula y marca no esten ni vacios ni con caracteres espacio
             y que la listaRepostaje tenga algun objeto dentro para añadir el nuevo coche a la coleccion*/
            if(String.IsNullOrWhiteSpace(cajaMatricula.Text) == false && String.IsNullOrWhiteSpace(cajaMarca.Text) == false
                && listaRepostaje.Any() == true){

                Coche c = new Coche(cajaMatricula.Text, cajaMarca.Text, listaRepostaje);
                //this.coche_ = new Coche(matricula, marca, listaRepostaje);
                coches.Add(c);

                this.DialogResult = true;
            }   
        }

        private void Button_Click_Cancelar(object sender, RoutedEventArgs e)
        {
            //listaRepostaje.Clear();
            this.DialogResult = false;
        }

        private void textBoxFecha_TextChanged(object sender, TextChangedEventArgs e)
        {
            validarFecha(this, e);
        }

        /*VAlidamos que los valores introducidos para la fecha sean correctos, en caso contrario,
         se pondra la caja en rojo*/
        private void validarFecha(object sender, TextChangedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace(cajaDia.Text) == false)
            {
                //Si el dia < 1 o > 31, se pone la cajaDia a rojo
                if (int.Parse(cajaDia.Text) >= 1 && int.Parse(cajaDia.Text) <= 31)
                {
                    if (String.IsNullOrWhiteSpace(cajaMes.Text) == false || cajaMes.Background != Brushes.Tomato)
                    {
                        //Si el mes es 4, 6, 9 u 11 y tiene 31 dias, cajaDia a rojo
                        if ((cajaMes.Text.Equals("4") || cajaMes.Text.Equals("6") || cajaMes.Text.Equals("9") ||
                            cajaMes.Text.Equals("11")) && cajaDia.Text.Equals("31"))
                        {
                            cajaDia.Background = Brushes.Tomato;
                        }
                        else
                        {
                            //Si el mes es 2 y tiene más de 30 dias, a rojo
                            if (cajaMes.Text.Equals("2") && int.Parse(cajaDia.Text) >= 30)
                                
                                cajaDia.Background = Brushes.Tomato;
                            else
                            {
                                //Si el ano no es bisisesto y mes 2 tiene 29 dias, a rojo
                                if (String.IsNullOrWhiteSpace(cajaAno.Text) == false)
                                {
                                    if ((int.Parse(cajaAno.Text) % 4 != 0) && cajaDia.Text.Equals("29"))
                                        cajaDia.Background = Brushes.Tomato;
                                    else
                                        cajaDia.Background = null;
                                }
                                //En los casos restantes, el mes es 1, 3, 5, 7, 8, 10 o 12 y tiene 31 dias
                                else
                                    cajaDia.Background = null;
                            }
                        }
                    }
                }
                else
                    cajaDia.Background = Brushes.Tomato;
            }

            if(String.IsNullOrWhiteSpace(cajaMes.Text) == false)
            {
                if (int.Parse(cajaMes.Text) < 1 || int.Parse(cajaMes.Text) > 12)
                    cajaMes.Background = Brushes.Tomato;
                else
                    cajaMes.Background = null;
            }
            else
                cajaMes.Background = null;

            if (String.IsNullOrWhiteSpace(cajaAno.Text) == false)
            {
                if (int.Parse(cajaAno.Text) < 1950 || int.Parse(cajaAno.Text) > DateTime.Now.Year)
                    cajaAno.Background = Brushes.Tomato;
                else
                    cajaAno.Background = null;
            }
            else
                cajaAno.Background = null;
        }

        //Comprobamos que cajaLitros, cajaCostes y cajaKm solo tengan valores numericos
        private void cajaKm_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (true == soloContieneNumeros(cajaKm.Text))
            {
                cajaKm.Background = null;
            }
            else
                cajaKm.Background = Brushes.Tomato;
        }

        private void cajaLitros_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (true == soloContieneNumeros(cajaLitros.Text))
            {
                cajaLitros.Background = null;
            }
            else
                cajaLitros.Background = Brushes.Tomato;
        }

        private void cajaCoste_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (true == soloContieneNumeros(cajaCoste.Text))
            {
                cajaCoste.Background = null;
            }
            else
                cajaCoste.Background = Brushes.Tomato;
        }

        private bool soloContieneNumeros(string cadena)
        {
            foreach (char c in cadena)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }
    }
}
