using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtualMemory
{
    class Procesador
    {
        private DataSet mainDataTables;
       private Dictionary<int,double> timestamp= new Dictionary<int,double>();
        private Dictionary<int, double> pageFaults = new Dictionary<int, double>();
        private Memoria M;
        private Memoria S;
        private SpecialQueue<int[]> FIFO;
        const int SIZE_MEMREAL= 128;
        const int SIZE_MEMCONT = 256;
        const int TIME_CARGA = 1;
        const double TIME_ACCESO = 0.1;
        private double contProcesos = 0.0;
        //public double[] timestamp= new double[];
       

        public Procesador()
        {
           // mainTableMap = new Dictionary<int, pageMapTable>(); //int es el proceso,
            mainDataTables = new DataSet();
            M = new Memoria(SIZE_MEMREAL); //memoria real
            S = new Memoria(SIZE_MEMCONT); //memoria coontigua
            FIFO = new SpecialQueue<int[]>();

        }
        private int convertirPag(int bytes)
        {
            double d = bytes / 16.0;
            int num = (int)(Math.Ceiling(d));
            return num; 
        }

        public void asignaProceso(int bytes, int proceso)
        {
            contProcesos = contProcesos + 1;
            int paginas = convertirPag(bytes);
            agregaTableMap(paginas, proceso);
            timestamp[proceso]= paginas*TIME_CARGA;
            pageFaults[proceso] = 0;
        }

        public void agregaTableMap(int paginas, int proceso)
        {
            pageMapTable tablaProce = new pageMapTable(proceso);

            tablaProce.agregaPaginas(paginas);
            for (int i = 0; i < paginas; i++)
            {
                tablaProce= cargaAmemoria(i, tablaProce, proceso);
            }
            mainDataTables.Tables.Add(tablaProce.getPageTable());

        }

        private  pageMapTable cargaAmemoria(int pagina, pageMapTable tablaProce, int proceso)
        {
            int[] arr = new int[2];
      
                int[] paraFifo = new int[2];
                paraFifo[0] = proceso;
                arr = M.getNextFreePF(true);
                if (arr[0] == -1) //La memoria se lleno, por lo tanto tenemos que hacer swap out
                {
                    swapOutFIFO();
                    arr = M.getNextFreePF(true);
                }
                // Si existe marco libre en Memoria Real 
                tablaProce.agregaDirPA(pagina, arr[0], arr[1], true); // Se asigna physical address low y high, e indicador que se encuentra en memoria M
                paraFifo[1] = pagina; 
                FIFO.Enqueue(paraFifo); //agregamos el proceso a la cola para despues poderlos remover

            return tablaProce;


        }

        public void swapOutFIFO()
        {
            int[] delFifo = new int[2];
            int[] libreContig = new int[2];//pa_low y pa_High

            delFifo = FIFO.Dequeue();
            DataTable tableProceso;
            timestamp[delFifo[0]] += TIME_CARGA;
            //buscas el dataTable con la infrmacion del proceso
            tableProceso = mainDataTables.Tables["" + delFifo[0]];
            foreach (DataRow dataRow in tableProceso.Rows)
            {

                //busca la pagina que quieres sacar en la tabla 
                if ((int)dataRow["Pagina"] == delFifo[1])
                {
                    libreContig = S.getNextFreePF(false);
                    decimal d = libreContig[0] / 16;
                    int marcoPaginaS = Decimal.ToInt32(Math.Ceiling(d));

                    d= (int) dataRow["Physical Address low"] / 16;
                    int marcoPaginaM = Decimal.ToInt32(Math.Ceiling(d));
                    M.eliminaMarco(marcoPaginaM);

                    Console.WriteLine("Spaw de proceso: " + delFifo[0] + " Pagina: " + delFifo[1]
                        + "al Marco: " + marcoPaginaS + " del area Swapping (Memoria contigua)");


                    dataRow["Physical Address low"] = libreContig[0]; //Physical address low del S
                    dataRow["Physycal Address high"] =libreContig[1]; //PA_hIGH DEL S
                    dataRow["Is on Memory"] = false;

                    
                    
                }
            }

            }

        public int[] leeDireccion(int virtualAddres,int numProceso,int tipo_accion)
        {
            DataTable tableProceso;
            int[] arrResultado=new int[3]; //arr[0]= pagina N arr[1]=direccion real arr[2]=marcoPagina
            int va_low, va_high, pa_low,direccion;
            tableProceso = mainDataTables.Tables["" + numProceso];
            if (tableProceso == null)
            {
                Console.WriteLine("El proceso " + numProceso + " no existe");
                return arrResultado;
            }

            foreach (DataRow dataRow in tableProceso.Rows)
            {
                va_low = (int)dataRow["Virtual Address low"];
                va_high = (int)dataRow["Virtual Address high"];
                pa_low = (int)dataRow["Physical Address low"];
                int pagina = (int)dataRow["Pagina"];

                if (virtualAddres>=va_low && virtualAddres <= va_high)
                {
                    if ((bool)dataRow["Is on Memory"] == true)
                    {
                        int posicion = virtualAddres - (int)dataRow["Virtual Address low"];
                        decimal d = pa_low / 16;
                        int marcoPagina = Decimal.ToInt32(Math.Ceiling(d));
                        direccion = marcoPagina * 16 + posicion;
                        arrResultado[0] = pagina;
                        arrResultado[1] = direccion;
                        arrResultado[2] = marcoPagina;
                    }
                    else
                    {   //Proceso de swap-in 
                        timestamp[numProceso] += TIME_CARGA;
                        pageFaults[numProceso]+=1;
                        pageMapTable dummyMap = new pageMapTable(-1);
                        dummyMap.agregaPagina(pagina);
                        dummyMap = cargaAmemoria(pagina, dummyMap, numProceso);
                        DataTable tempTable= dummyMap.getPageTable();
                        dataRow["Physical Address low"] = dummyMap.getDirPA(pagina)[0];
                        dataRow["Physycal Address high"] = dummyMap.getDirPA(pagina)[1];
                        dataRow["Is on Memory"] = true;
                        int posicion= virtualAddres- (int)dataRow["Virtual Address low"];
                        decimal d = (int) dataRow["Physical Address low"] / 16;
                        int marcoPagina = Decimal.ToInt32(Math.Ceiling(d));
                        direccion = marcoPagina * 16 + posicion;
                        arrResultado[0] = pagina;
                        arrResultado[1] = direccion;
                        arrResultado[2] = marcoPagina;

                        Console.WriteLine("Se localizó la pagina " + arrResultado[0] + " del proceso " + numProceso);
                    }

                    if (tipo_accion == 1)
                    {
                        dataRow["Is modified"] = true;
                    }
                    else if(tipo_accion==0)
                    {
                        dataRow["Is modified"] = false;
                    }
                }
                
            }

            timestamp[numProceso] += TIME_ACCESO;
            return arrResultado;
        }

        public void quitaTableMap(int numProceso)
        {
            DataTable tableProceso;
            int[] arrResultado = new int[3];
            tableProceso = mainDataTables.Tables["" + numProceso]; //saca la tabla del proceso
            int va_low;

            if( tableProceso== null)
            {
                Console.WriteLine("El proceso " + numProceso + " no existe");
                return;
            }

            foreach (DataRow dataRow in tableProceso.Rows)
            {
                va_low = (int)dataRow["Virtual Address low"];
                arrResultado=leeDireccion(va_low, numProceso,-1);
                if (arrResultado == null)
                {
                    return;
                }
                //mando a llamar m para eliminar un marco de pagina de memoria real
                M.eliminaMarco(arrResultado[2]);
                Console.WriteLine("Se libera el marco de pagina de memoria real:" + arrResultado[2]);
            }
                //elimino la tabla del proceso (Toda la info de direcciones)
                mainDataTables.Tables.Remove(""+numProceso);

            //Elimina referencia de proceso en FIFO
            FIFO.Remove(numProceso);
                
             }
        
        public void tiempo()
        {
            double turnaroundtotal = 0;
            double pagefaultTot = 0;
            foreach(int key in timestamp.Keys)
            {
                foreach (KeyValuePair<int,double> pair in timestamp)
                {
                    turnaroundtotal = (double)timestamp[key] + turnaroundtotal;
                    pagefaultTot = (double)pageFaults[key] + pagefaultTot;
                }
                
            }

             Console.WriteLine("TURNAROUND TOTAL: " + turnaroundtotal);
            Console.WriteLine("TURNAROUND PROMEDIO:" + turnaroundtotal / contProcesos);
            Console.WriteLine("PAGEFAULTS: " + pagefaultTot);
                
                }
        

        public void imprime(int numProceso)
        {
             DataTable tableAux;
            tableAux=mainDataTables.Tables[""+numProceso];

            foreach(DataRow dataRow in tableAux.Rows)
            {
                Console.Write(dataRow["Pagina"]+ " ");
                Console.Write(dataRow["Virtual Address low"]+ " ");
                Console.Write(dataRow["Virtual Address high"]+ " ");
                Console.Write(dataRow["Physical Address low"]+" ");
                Console.Write(dataRow["Physycal Address high"]+" ");
                Console.WriteLine(dataRow["Is on Memory"]);
                
            }
        }
    }
}
