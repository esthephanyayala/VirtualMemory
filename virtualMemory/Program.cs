using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtualMemory
{
    class Program
    {

        static void Main(string[] args)
        {
            int bytes, proceso, dir, tipo_accion;
            int[] a_resultado = new int[3];
            string input;
            Queue FIFO = new Queue();
            Procesador P = new Procesador();
             while (true)
             {  
                 input = Console.ReadLine();
                string[] sp_intrucc = input.Split(' ');
                char instruccion = input[0];

                 switch (instruccion)
                 {
                     case 'C':
                         Console.WriteLine(input+ " comenzamos...");
                         break;
                     case 'P':

                        bytes = int.Parse(sp_intrucc[1]);
                        proceso = int.Parse(sp_intrucc[2]);
                        Console.WriteLine("Asignar " + bytes + " bytes al proceso " + proceso);
                        if (bytes > 2048)
                        {
                            Console.WriteLine("Error! Bytes solicitados excede tamaño de memoria Real");
                            break;
                        } 
                         P.asignaProceso(bytes, proceso);
                        // P.imprime(proceso);
                         break;
                     case 'A':
                         dir = int.Parse(sp_intrucc[1]);
                         proceso = int.Parse(sp_intrucc[2]);
                         tipo_accion = int.Parse(sp_intrucc[3]);
                        Console.WriteLine("Obtener la direccion real correpondiente a la direccion virtual " + dir + " del proceso  " + proceso);
                        a_resultado =P.leeDireccion(dir, proceso, tipo_accion);
                        if (a_resultado== null)
                        {
                            break;
                        }
                        Console.WriteLine("Pagina: " + a_resultado[0] + " Direccion virtual"+dir+" Direccion real: " + a_resultado[1] + " Marco de pagina: " + a_resultado[2]);
                         break;
                     case 'L':
                         proceso = int.Parse(sp_intrucc[1]);
                        Console.WriteLine("Liberar los marcos de pagina ocupados por el proceso " + proceso);
                        P.quitaTableMap(proceso);
                         break;
                    case 'I':
                        proceso = int.Parse(sp_intrucc[1]);
                        P.imprime(proceso);
                        break;
                    case 'F':
                        P.tiempo();
                        break;
                }
             }
            
            Console.ReadLine();
        }
    }
}
