using ControlGastos;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class GastosController : ControllerBase
{
    private readonly IGastos _gastosCategoria;
    private readonly IGastos _registroGastos;
    private readonly IGastos _resumenPorCategorias;
    private readonly IGastos _registroIngresos;
    private readonly IGastos _saldoTotal;


    public GastosController(IGastos categoriaGastos, IGastos registroGastos, IGastos resumenPorCategorias, IGastos registroIngresos, IGastos saltoTotal)
    {
        _gastosCategoria = categoriaGastos;
        _registroGastos = registroGastos;
        _resumenPorCategorias = resumenPorCategorias;
        _registroIngresos = registroIngresos;
        _saldoTotal = saltoTotal;
    }

    [HttpGet("CategoriasGastos")]
    public IActionResult ConnectionSelect()
    {
        var categoriaGastos = _gastosCategoria.CategoriaSelect();
        return Ok(categoriaGastos);
    }

    [HttpPost("CrearCategoria")]
    public IActionResult InsertCategoria([FromBody] InsertCategoriaGastos request)
    {
        _gastosCategoria.InsertCategoria(request.CategoryName);
        return Ok(new { message = "Category created successfully." });
    }


    [HttpPost("RegistrosGastos")]
    public IActionResult Registogastos([FromBody] RangoFechas rangoFechas)
    {
        var registroGastos = _registroGastos.RegistroGastos(rangoFechas.FechaInicio, rangoFechas.FechaFin);
        return Ok(registroGastos);
    }

    [HttpPost("InsertarRegistroGastos")]
    public IActionResult InsertarRegistrosGastos([FromBody] InsertarRegistrosGastos request)
    {
        _registroGastos.InsertarRegistrosGastos(request.CategoryID, request.Concepto, request.Amount, (DateTime)request.Date);
        return Ok(new { message = "Category created successfully." });
    }

    [HttpGet("ResumenGastosPorCategoria")]
    public IActionResult ResumenGastosPorCategoria()
    {
        var resumenPorCategorias = _resumenPorCategorias.ResumenGastosPorCategoria();
        return Ok(resumenPorCategorias);
    }

    [HttpPost("InsertarRegistrosIngresos")]
    public IActionResult InsertarRegistrosIngresos([FromBody] InsertarRegistrosIngresos request)
    {
        _registroIngresos.InsertarRegistrosIngresos(request.CategoryID, request.Concepto, request.Amount, (DateTime)request.Date);
        return Ok(new { message = "Category created successfully." });
    }

    [HttpGet("ultimoSaldo")]
    public IActionResult ObtenerSaldoTotal()
    {
        var saldoTotal = _saldoTotal.ObtenerSaldoTotal();
        return Ok(saldoTotal);
    }

}
