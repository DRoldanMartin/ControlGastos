using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlGastos
{
    public interface IGastos
    {
        List<CategoriaGastosModel> CategoriaSelect();

        void InsertCategoria(string? CategoryName);

        List<RegistroGastos> RegistroGastos(DateTime fechaInicio, DateTime fechaFin);

        void InsertarRegistrosGastos(int? categoryID, string? concepto, float? amount, DateTime date);

        List<ResumenGastosPorCategoria> ResumenGastosPorCategoria();

        void InsertarRegistrosIngresos(int? categoryID, string? concepto, float? amount, DateTime date);

        public float ObtenerSaldoTotal();
    }
}
