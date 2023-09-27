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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace coches
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<Coche> coches = new ObservableCollection<Coche>();
        VentanaVer ventanaVer = null;
        VentanaAnadir ventanaAnadir = null;

        public MainWindow()
        {
            InitializeComponent();
            coches.CollectionChanged += coches_CollectionChanged;
        }

        private void MenuItem_Click_AnadirCoche(object sender, RoutedEventArgs e)
        {
            if (ventanaAnadir == null)
            {
                ventanaAnadir = new VentanaAnadir(coches);
                ventanaAnadir.Owner = this;
                ventanaAnadir.Closed += VentanaAnadir_Closed;

                ventanaAnadir.ShowDialog();

                /*if (ventanaAnadir.DialogResult == true)
                {
                    coches.Add(ventanaAnadir.coche);
                }*/
            }

            /*ventanaAnadir.ShowDialog();

            if(ventanaAnadir.DialogResult == true)
            {
                coches.Add(ventanaAnadir.coche);
            }*/
        }

        private void VentanaAnadir_Closed(object sender, EventArgs e)
        {
            ventanaAnadir = null;
        }

        /* Al abrir una nueva instancia de la ventanaVer, se asignan los gestores de los eventos
          Closed y CambioSeleccion*/
        private void MenuItem_Click_VerCoches(object sender, RoutedEventArgs e)
        {
            if(ventanaVer == null)
            {
                ventanaVer = new VentanaVer(coches);
                ventanaVer.Closed += VentanaVer_Closed;
                ventanaVer.cambioSeleccion += ventanaVer_CambioSeleccion;
            }

            ventanaVer.Show();
        }

        private void ventanaVer_CambioSeleccion(object sender, CambioSeleccionEventArgs e)
        {
            double mayorKm = 0, mayorCoste = 0, mayorConsumo = 0, anteriorKm = 0, diferenciaKm;
            double alturaPintar, escalaConsumo, escalaCoste, escalaKm, distanciaPuntos;
            int cont = 0;

            Polyline lineaCoste = new Polyline();
            Polyline lineaConsumo = new Polyline();
            Polyline lineaKm = new Polyline();

            lineaCoste.StrokeThickness = lineaConsumo.StrokeThickness = lineaKm.StrokeThickness = 1;
            lineaCoste.Stroke = Brushes.Red;
            lineaConsumo.Stroke = Brushes.Green;
            lineaKm.Stroke = Brushes.Blue;

            //Vaciamos los canvas antes de pintar en ellos
            lienzoLateralDer.Children.Clear();
            lienzoLateralIz.Children.Clear();
            lienzoPrincipal.Children.Clear();

            /*Obtenemos el mayor coste, mayores km recorridos y mayor consumo de litros
              para poder dibujar los ejes verticales y para poder realizar los calculos para
              situar cada dato en el lienzo principal*/
            foreach (Repostaje r in e.cocheSeleccionado.paradas)
            {
                if (r.coste > mayorCoste)
                    mayorCoste = r.coste;

                if (r.litros > mayorConsumo)
                    mayorConsumo = r.litros;

                if(cont == 0)
                {
                    anteriorKm = r.km;
                }
                else
                {
                    diferenciaKm = r.km - anteriorKm;
                    anteriorKm = r.km;

                    if (diferenciaKm > mayorKm)
                        mayorKm = diferenciaKm;
                }

                cont++;
            }

            //Redondeamos a 10 los mayores datos
            mayorCoste = redondeoUnidadBase((int)mayorCoste, 10);
            mayorConsumo = redondeoUnidadBase((int)mayorConsumo, 10);
            
            //Calculamos la altura para pintar en el lienzo que será altura - altura*5%
            alturaPintar = lienzoLateralDer.ActualHeight - lienzoLateralDer.ActualHeight * 0.05;

            //Los ejes dibujaran rayas cad 10 puntos, y aquí calculamos el equivalente en pixeles del canvas
            escalaCoste = (10 * alturaPintar) / mayorCoste;
            escalaConsumo = (10 * alturaPintar) / mayorConsumo;          

            //Dibujamos el eje de los costes
            for (int i = 0, j = 0; j <= mayorCoste; i++, j += 10)
            {
                Line l = new Line();
                TextBlock tb = new TextBlock();
                l.Stroke = Brushes.Red;
                l.StrokeThickness = 2;
                l.Y1 = l.Y2 = alturaPintar - escalaCoste * i;
                l.X1 = 0;
                l.X2 = 8;
                lienzoLateralIz.Children.Add(l);

                tb.Text = j.ToString();
                tb.Foreground = Brushes.Red;
                tb.FontSize = 7;
                Canvas.SetLeft(tb, 10);
                Canvas.SetTop(tb, l.Y1 - tb.FontSize);
                lienzoLateralIz.Children.Add(tb);
            }

            //Dibujamos el eje de consumo
            for (int i = 0, j = 0; j <= mayorConsumo; i++, j += 10)
            {
                Line l = new Line();
                TextBlock tb = new TextBlock();
                l.Stroke = Brushes.Green;
                l.StrokeThickness = 2;
                l.Y1 = l.Y2 = alturaPintar - escalaConsumo * i;
                l.X1 = lienzoLateralIz.ActualWidth;
                l.X2 = lienzoLateralIz.ActualWidth - 8;
                lienzoLateralIz.Children.Add(l);

                tb.Text = j.ToString();
                tb.Foreground = Brushes.Green;
                tb.FontSize = 7;
                Canvas.SetLeft(tb, lienzoLateralIz.ActualWidth - 14);
                Canvas.SetTop(tb, l.Y1 - tb.FontSize);
                lienzoLateralIz.Children.Add(tb);
            }

            /*Si solo introducimos un objeto repostaje en el coche, al introducir los datos del cuenta km
             por cada respostaje, no hay diferencia entre paradas y por ende no hay distancia recorrida,
             por lo que es necesario tener en cuenta este caso*/
            if(mayorKm != 0)
            {
                mayorKm = redondeoUnidadBase((int)mayorKm, 100);
                escalaKm = (100 * alturaPintar) / mayorKm;

                //Dibujamos eje de Km en caso de haber dos o más objetos repostaje
                for (int i = 0, j = 0; j <= mayorKm; i++, j += 100)
                {
                    Line l = new Line();
                    TextBlock tb = new TextBlock();
                    l.Stroke = Brushes.Blue;
                    l.StrokeThickness = 2;

                    l.Y1 = l.Y2 = alturaPintar - escalaKm * i;
                    l.X1 = 0;
                    l.X2 = 8;
                    lienzoLateralDer.Children.Add(l);

                    tb.Text = j.ToString();
                    tb.Foreground = Brushes.Blue;
                    tb.FontSize = 7;
                    Canvas.SetLeft(tb, 10);
                    Canvas.SetTop(tb, l.Y1 - tb.FontSize);
                    lienzoLateralDer.Children.Add(tb);
                }
            }

            //Distancia horizontal entre los valores de costes, consumo y km de una fecha y otra
            distanciaPuntos = lienzoPrincipal.ActualWidth / (double)(e.cocheSeleccionado.paradas.Count + 1);
            cont = 0;

            //Dibujamos la gráfica
            foreach (Repostaje r in e.cocheSeleccionado.paradas)
            {
                Point puntoCoste = new Point();
                Point puntoConsumo = new Point();

                TextBlock tb = new TextBlock();

                puntoCoste.X = puntoConsumo.X = distanciaPuntos * (cont + 1);
                puntoCoste.Y = alturaPintar - (double)r.coste * alturaPintar / mayorCoste;
                puntoConsumo.Y = alturaPintar - (double)r.litros * alturaPintar / mayorConsumo;

                if(cont == 0)
                {
                    anteriorKm = r.km;
                }
                else
                {
                    Point puntoKm = new Point();

                    diferenciaKm = r.km - anteriorKm;
                    anteriorKm = r.km;
                    puntoKm.Y = alturaPintar - (double)diferenciaKm * alturaPintar / mayorKm;
                    puntoKm.X = puntoCoste.X;

                    lineaKm.Points.Add(puntoKm);
                }
                
                tb.Text = "" + r.fecha.Day + "-" + r.fecha.Month + "-" + r.fecha.Year;
                tb.FontSize = 8;
                Canvas.SetLeft(tb, puntoCoste.X - 15);
                Canvas.SetTop(tb, alturaPintar);

                lienzoPrincipal.Children.Add(tb);

                lineaCoste.Points.Add(puntoCoste);
                lineaConsumo.Points.Add(puntoConsumo);

                cont++;
            }

            /*Al ser una "pantalla" distinta a la principal, habilitamos un boton que nos permita ver
             el grafico general*/
            botonVolverPrincipal.Visibility = Visibility.Visible;

            //Añadimos las lineas calculadas al lienzoPrincipal
            lienzoPrincipal.Children.Add(lineaCoste);
            lienzoPrincipal.Children.Add(lineaConsumo);
            lienzoPrincipal.Children.Add(lineaKm);
        }

        private void VentanaVer_Closed(object sender, EventArgs e)
        {
            ventanaVer = null;
        }

        private void coches_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            pintarPantallaPrincipal();
        }

        //Redondea un entero a la decena, centena o millar superior más próximo
        private int redondeoUnidadBase(int num, int unidadBase)
        {
            if(unidadBase == 10 || unidadBase == 1000 || unidadBase == 100)
            {
                if (num % unidadBase == 0)
                    return num;
                return (unidadBase - num % unidadBase) + num;
            }

            return -1;
        }

        private void botonVolverPrincipal_Click(object sender, RoutedEventArgs e)
        {
            botonVolverPrincipal.Visibility = Visibility.Hidden;
            pintarPantallaPrincipal();
        }

        private void pintarPantallaPrincipal()
        {
            int mayorCoste = 0, mayorKm = 0, mayorConsumo = 0, cont;
            double alturaPintar, distanciaSeparacionCoche, distanciaSeparacionLinea = 30, ultimaColumna;

            double escalaCoste, escalaKm, escalaConsumo;

            botonVolverPrincipal.Visibility = Visibility.Hidden;
            lienzoLateralDer.Children.Clear();
            lienzoLateralIz.Children.Clear();
            lienzoPrincipal.Children.Clear();
            lienzoInf.Children.Clear();

            //Dibujamos en el lienzo principal si la colección de coches no está vacia
            if (this.coches != null)
            {
                /* Dibujamos la leyenda inferior que consiste en un una raya de un color y el dato que 
                 representa: Rojo - costes, Verde - Consumo y Azul - Km*/
                for (int i = 0; i <= 2; i++)
                {
                    Line l = new Line();
                    l.StrokeThickness = 5;

                    l.X1 = (double)i / 3 * lienzoInf.ActualWidth + 0.1 * lienzoInf.ActualWidth;
                    l.X2 = l.X1 + 20;

                    l.Y1 = l.Y2 = lienzoInf.ActualHeight / 2;

                    TextBlock tb = new TextBlock();
                    tb.FontSize = 10;
                    Canvas.SetLeft(tb, l.X2 + 20);
                    Canvas.SetTop(tb, l.Y1 - 9);

                    if (i == 0)
                    {
                        l.Stroke = tb.Foreground = Brushes.Red;
                        tb.Text = "Coste (€)";
                    }
                    if (i == 1)
                    {
                        l.Stroke = tb.Foreground = Brushes.Green;
                        tb.Text = "Consumo (litros)";
                    }
                    if (i == 2)
                    {
                        l.Stroke = tb.Foreground = Brushes.Blue;
                        tb.Text = "Km";
                    }

                    lienzoInf.Children.Add(l);
                    lienzoInf.Children.Add(tb);
                }

                //Obtenemos el máximo valor de costes, consumo y km
                foreach (Coche c in this.coches)
                {
                    if (c.mediaCoste > mayorCoste)
                        mayorCoste = (int)Math.Round(c.mediaCoste, 0);

                    if (c.km > mayorKm)
                        mayorKm = c.km;

                    if (c.mediaConsumo > mayorConsumo)
                        mayorConsumo = (int)Math.Round(c.mediaConsumo, 0);
                }

                alturaPintar = lienzoLateralDer.ActualHeight - lienzoLateralDer.ActualHeight * 0.1;

                /* En caso de que solo haya un coche en la lista de coches, y este
                 coche solo tenga registrado un objeto repostaje, los valores mayorConsumo
                 y mayorCoste estarán a cero por lo que no se podrá dibujar el eje vertical*/
                if (mayorConsumo != 0 && mayorCoste != 0)
                {
                    /* mayorCoste = mayorCoste + 0.05f * mayorCoste;
                     mayorConsumo = mayorConsumo + 0.05f * mayorConsumo;
                     mayorKm = mayorKm + 0.05f * mayorKm;*/

                    mayorCoste = redondeoUnidadBase(mayorCoste, 10);
                    mayorConsumo = redondeoUnidadBase(mayorConsumo, 10);

                    escalaCoste = (10 * alturaPintar) / (double)mayorCoste;
                    escalaConsumo = (10 * alturaPintar) / (double)mayorConsumo;

                    //Dibujamos eje de costes
                    for (int i = 0, j = 0; j <= mayorCoste; i++, j += 10)
                    {
                        Line l = new Line();
                        TextBlock tb = new TextBlock();
                        l.Stroke = Brushes.Red;
                        l.StrokeThickness = 2;
                        l.Y1 = l.Y2 = alturaPintar - escalaCoste * i;
                        l.X1 = 0;
                        l.X2 = 8;
                        lienzoLateralIz.Children.Add(l);

                        tb.Text = j.ToString();
                        tb.Foreground = Brushes.Red;
                        tb.FontSize = 7;
                        Canvas.SetLeft(tb, 10);
                        Canvas.SetTop(tb, l.Y1 - tb.FontSize);
                        lienzoLateralIz.Children.Add(tb);
                    }

                    //Dibujamos eje de consumo
                    for (int i = 0, j = 0; j <= mayorConsumo; i++, j += 10)
                    {
                        Line l = new Line();
                        TextBlock tb = new TextBlock();
                        l.Stroke = Brushes.Green;
                        l.StrokeThickness = 2;
                        l.Y1 = l.Y2 = alturaPintar - escalaConsumo * i;
                        l.X1 = lienzoLateralIz.ActualWidth;
                        l.X2 = lienzoLateralIz.ActualWidth - 8;
                        lienzoLateralIz.Children.Add(l);

                        tb.Text = j.ToString();
                        tb.Foreground = Brushes.Green;
                        tb.FontSize = 7;
                        Canvas.SetLeft(tb, lienzoLateralIz.ActualWidth - 14);
                        Canvas.SetTop(tb, l.Y1 - tb.FontSize);
                        lienzoLateralIz.Children.Add(tb);
                    }
                }

                /* Si hay más de un objeto repostaje, se puede calcular el mayorKm, si no no*/
                if (mayorKm != 0)
                {
                    mayorKm = redondeoUnidadBase(mayorKm, 1000);
                    escalaKm = (1000 * alturaPintar) / (double)mayorKm;

                    //Calculamos eje de km
                    for (int i = 0, j = 0; j <= mayorKm; i++, j += 1000)
                    {
                        Line l = new Line();
                        TextBlock tb = new TextBlock();
                        l.Stroke = Brushes.Blue;
                        l.StrokeThickness = 2;

                        l.Y1 = l.Y2 = alturaPintar - escalaKm * i;
                        l.X1 = 0;
                        l.X2 = 8;
                        lienzoLateralDer.Children.Add(l);

                        tb.Text = j.ToString();
                        tb.Foreground = Brushes.Blue;
                        tb.FontSize = 7;
                        Canvas.SetLeft(tb, 10);
                        Canvas.SetTop(tb, l.Y1 - tb.FontSize);
                        lienzoLateralDer.Children.Add(tb);
                    }
                }

                cont = 0;
                ultimaColumna = 0;

                /*Por cada coche, dibujamos el grafico*/
                foreach (Coche c in this.coches)
                {
                    Line lineaCostes = new Line();
                    Line lineaConsumo = new Line();
                    Line lineaKm = new Line();

                    lineaCostes.Stroke = Brushes.Red;
                    lineaConsumo.Stroke = Brushes.Green;
                    lineaKm.Stroke = Brushes.Blue;

                    lineaCostes.StrokeThickness = lineaConsumo.StrokeThickness = lineaKm.StrokeThickness = 10;

                    if (cont == 0)
                        distanciaSeparacionCoche = 50;
                    else
                    {
                        distanciaSeparacionCoche = 80;

                        //if (cont >= 4)
                        //scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    }

                    lineaCostes.Y1 = lineaConsumo.Y1 = lineaKm.Y1 = alturaPintar;
                    lineaCostes.X1 = lineaCostes.X2 = distanciaSeparacionCoche + ultimaColumna;
                    lineaConsumo.X1 = lineaConsumo.X2 = lineaCostes.X1 + distanciaSeparacionLinea;
                    lineaKm.X1 = lineaKm.X2 = lineaConsumo.X1 + distanciaSeparacionLinea;

                    ultimaColumna = lineaKm.X1;

                    if (c.mediaCoste != 0 && c.mediaConsumo != 0)
                    {
                        lineaCostes.Y2 = alturaPintar - alturaPintar * c.mediaCoste / (double)mayorCoste;
                        lineaConsumo.Y2 = alturaPintar - alturaPintar * c.mediaConsumo / (double)mayorConsumo;
                    }
                    else
                    {
                        lineaCostes.Y2 = alturaPintar;
                        lineaConsumo.Y2 = alturaPintar;
                    }

                    if (c.km != 0)
                    {
                        lineaKm.Y2 = alturaPintar - alturaPintar * c.km / (double)mayorKm;
                    }
                    else
                    {
                        lineaKm.Y2 = alturaPintar;
                    }

                    lienzoPrincipal.Children.Add(lineaCostes);
                    lienzoPrincipal.Children.Add(lineaConsumo);
                    lienzoPrincipal.Children.Add(lineaKm);


                    TextBlock tb = new TextBlock();
                    tb.Text = c.matricula;
                    tb.FontSize = 7;
                    Canvas.SetLeft(tb, lineaCostes.X1 + distanciaSeparacionLinea);
                    Canvas.SetTop(tb, alturaPintar + alturaPintar * 0.05);
                    lienzoPrincipal.Children.Add(tb);

                    cont++;
                }
            }
        }
    }
}
