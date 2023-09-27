using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace coches
{
    public class Coche
    {
        private string matricula_;
        private string marca_;
        private ObservableCollection<Repostaje> paradas_;

        public Coche(string mat, string marc, ObservableCollection<Repostaje> p)
        {
            this.matricula_ = mat;
            this.marca_ = marc;
            this.paradas_ = p;
        }

        public string matricula
        {
            get { return this.matricula_; }
            set { this.matricula_ = value; }
        }

        public string marca
        {
            get { return this.marca_; }
            set { this.marca_ = value; }
        }

        public ObservableCollection<Repostaje> paradas
        {
            get { return this.paradas_; }
        }

        /*Calcula el mayor valor del cuentakilometros del coche*/
        public int km
        {
            get
            {
                if(this.paradas_ != null)
                {
                    int mayor = 0;

                    foreach (Repostaje p in this.paradas_)
                    {
                        if (p.km >= mayor)
                            mayor = p.km;
                    }

                    return mayor;
                }
                else
                {
                    return 0;
                }
            }
        }

        /*Calcula la media de consumo por 100 km recorridos*/
        public float mediaConsumo{
            get
            {
                if(this.paradas_ != null)
                {
                    float sumaLitros = 0, mayor = 0, menor = 0;
                    float media, mediaLitros, mediaKm, i = 0;

                    foreach(Repostaje p in this.paradas_)
                    {
                        if (p.km >= mayor)
                            mayor = p.km;

                        if (i == 0 || p.km < menor)
                            menor = p.km;

                        sumaLitros += p.litros;
                        i++;
                    }

                    if (i != 1)
                    {
                        mediaLitros = sumaLitros / i;
                        mediaKm = (mayor - menor) / i;

                        media = (mediaLitros / mediaKm) * 100;
                        return media;
                    }

                    return 0;
                }
                else
                {
                    return 0;
                }
            }
        }

        /*Calcula la media de coste por 100 km recorridos*/
        public float mediaCoste
        {
            get
            {
                if (this.paradas_ != null)
                {
                    float sumaCoste = 0;
                    float media, mediaCoste, mediaKm, mayor = 0, menor = 0;
                    int i = 0;

                    foreach (Repostaje p in this.paradas_)
                    {
                        if(p.km >= mayor)
                            mayor = p.km;

                        if (i == 0 || p.km < menor)
                            menor = p.km;

                        sumaCoste += p.coste;
                        i++;
                    }

                    if (i != 1)
                    {
                        mediaCoste = sumaCoste / i;
                        mediaKm = (mayor - menor) / i;

                        media = (mediaCoste / mediaKm) * 100;

                        return media;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }


    }
}
