using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtualMemory
{
    class Memoria
    {
        //
        //ocupado/libre (busqueda donde recibe el marco y te regresa un bool)
        //getLibre(te da la direccion del espacio libre y el marco )

        private bool[] marcoPagina; 

        public Memoria(int tamMarcos)
        {
            marcoPagina = new bool[tamMarcos];

        }

        //te devuelve un vector <pa_low,pa_high> donde dice cual es la pag disponible
        public int[] getNextFreePF(bool isReal)
        {
            int[] arr=new int [2];
            for (int i = 0; i < marcoPagina.Length; i++)
            {
                if (marcoPagina[i] == false)
                {
                    arr[0] = i * 16; //paLow
                    arr[1] = arr[0] + 15; //paHigh
                    marcoPagina[i] = true;
                    if (isReal)
                    {
                       Console.WriteLine("Se asignó marco de pagina a memoria real " + i);
                    }
                    else
                    {
                        Console.WriteLine("Se asignó marco de pagina a memoria contigua " + i);
                    }
                    break;
                }
                else
                {
                    arr[0] = -1;
                    arr[1] = -1;
                }
            }
            //hacer busqueda del marcoPagina el primero que sea false es LIBRE
            //hacer operacion automatica para saber de donde a donde va pa_low->pa_high
            return arr; 
        }

        public void eliminaMarco(int marco)
        {
            marcoPagina[marco] = false;
        }
    }
}
