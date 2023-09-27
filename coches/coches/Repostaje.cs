using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coches
{
    /*Clase que representa los datos de un repostaje, la fecha, el valor del cuentakilometros,
     litros echados y coste*/
    public class Repostaje
    {
        private DateTime fecha_;
        private int km_;
        private int litros_;
        private int coste_;

        public Repostaje(DateTime f, int k, int l, int c)
        {
            this.fecha_ = f;
            this.km_ = k;
            this.litros_ = l;
            this.coste_ = c;
        }

        public DateTime fecha
        {
            get { return this.fecha_; }
            set { this.fecha_ = value; }
        }

        public int km
        {
            get { return this.km_; }
            set { this.km_ = value; }
        }

        public int litros
        {
            get { return this.litros_; }
            set { this.litros_ = value; }
        }

        public int coste
        {
            get { return this.coste_; }
            set { this.coste_ = value; }
        }


    }
}
