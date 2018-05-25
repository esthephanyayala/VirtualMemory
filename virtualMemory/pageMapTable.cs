using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtualMemory
{
    class pageMapTable
    {
        /* private int virtualAddress;
         public int virtual_Address {
         get{return virtualAddress; }
             set { virtualAddress = value; }
         }
         */

        private DataTable pageTable;

        public DataTable getPageTable()
        {
            return this.pageTable;
        }



        //constructor
        public pageMapTable(int proceso)
        {
            pageTable = new DataTable();
            pageTable.TableName = "" + proceso;
            DataColumn pagina = new DataColumn();
            pagina.ColumnName = "Pagina";
            pagina.DataType = System.Type.GetType("System.Int32");
            DataColumn va_low = new DataColumn();
            va_low.ColumnName = "Virtual Address low";
            va_low.DataType = System.Type.GetType("System.Int32");
            DataColumn va_high = new DataColumn();
            va_high.ColumnName = "Virtual Address high";
            va_high.DataType = System.Type.GetType("System.Int32");
            DataColumn pa_low = new DataColumn();
            pa_low.ColumnName = "Physical Address low";
            pa_low.DataType = System.Type.GetType("System.Int32");
            DataColumn pa_high = new DataColumn();
            pa_high.ColumnName = "Physycal Address high";
            pa_high.DataType = System.Type.GetType("System.Int32");
            DataColumn is_on_memory = new DataColumn();
            is_on_memory.ColumnName = "Is on Memory";
            is_on_memory.DataType = typeof(bool);
            DataColumn is_modified = new DataColumn();
            is_modified.ColumnName = "Is modified";
            is_modified.DataType = typeof(bool);

            //agrega las columnas a la tabla 
            pageTable.Columns.Add(pagina);
            pageTable.Columns.Add(va_low);
            pageTable.Columns.Add(va_high);
            pageTable.Columns.Add(pa_low);
            pageTable.Columns.Add(pa_high);
            pageTable.Columns.Add(is_on_memory);
            pageTable.Columns.Add(is_modified);
        }

        public void agregaPaginas(int cantPag)
        {
            //dependendiendo de las paginas asignar los bytes de 16 en 16
            //paginas va de 0 a cantpag-1
            //ej:
            //table.Rows.Add("cat", DateTime.Now);
            int va_low = 0;
            int va_high = va_low + 15;

            for (int i = 0; i < cantPag; i++)
            {
                pageTable.Rows.Add(i, va_low, va_high);
                va_low = va_high + 1;
                va_high = va_low + 15;
            }

        }
        public void agregaPagina(int Pag)
        {
            pageTable.Rows.Add(Pag);
        }
        public void agregaDirPA(int pagina, int pa_low, int pa_high, bool is_on_memory)
        {
            string query = "Pagina=" + pagina;
            DataRow row = pageTable.Select(query).FirstOrDefault();
            row["Physical Address low"] = pa_low;
            row["Physycal Address high"] = pa_high;
            row["Is on Memory"] = is_on_memory;

        }

        public int[] getDirPA(int pagina)
        {
            int[] dir_pas = new int[2];
            string query = "Pagina=" + pagina;
            DataRow row = pageTable.Select(query).FirstOrDefault();


            dir_pas[0] = (int)row["Physical Address low"];
            dir_pas[1] = (int)row["Physycal Address high"];
            return dir_pas;

        }
    }
}
