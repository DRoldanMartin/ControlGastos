using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ControlGastos
{

    public class CategoriaGastosModel
    {
        public int? ID { get; set; }
        public string? CategoryName { get; set; }
    }

    public class InsertCategoriaGastos
    {
        public string? CategoryName { get; set; }
    }

    public class RegistroGastos
    {
        public int? ID { get; set; }
        public int? CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public string? Concepto { get; set; }
        public float? Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class InsertarRegistrosGastos
    {
        public int? CategoryID { get; set; }
        public string? Concepto { get;set; }
        public float? Amount { get; set;}
        public DateTime Date { get; set; }
    }

    public class ResumenGastosPorCategoria
    {
        public string? CategoryName { get; set; }
        public float? Amount { get; set; }
    }

    public class InsertarRegistrosIngresos
    {
        public int? CategoryID { get; set; }
        public string? Concepto { get; set; }
        public float? Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class RangoFechas
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }




    public class ControlGastos : IGastos
    {
        public List<CategoriaGastosModel> CategoriaSelect()
        {
            List<CategoriaGastosModel> categoriaGastos = new List<CategoriaGastosModel>();
            string connectionString = "server=localhost;database=control_gastos;uid=root;password=Qw3rtyu1";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ID, CategoryName FROM categorias";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CategoriaGastosModel categorygastos = new CategoriaGastosModel
                                {
                                    ID = reader.GetInt32("ID"),
                                    CategoryName = reader.GetString("CategoryName"),
                                };

                                categoriaGastos.Add(categorygastos);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            return categoriaGastos;
        }

        public void InsertCategoria(string CategoryName)
        {
            string connectionString = "server=localhost;database=control_gastos;uid=root;password=Qw3rtyu1";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO categorias (CategoryName) values (@CategoryName)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@CategoryName", CategoryName);
                    cmd.ExecuteNonQuery();


                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }

                finally { connection.Close(); }
            }
        }

        public List<RegistroGastos> RegistroGastos(DateTime fechaInicio, DateTime fechaFin)
        {
            List<RegistroGastos> registrosGastos = new List<RegistroGastos>();
            string connectionString = "server=localhost;database=control_gastos;uid=root;password=Qw3rtyu1";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT a.ID, a.CategoryID, b.CategoryName, a.Concepto, a.Amount, a.Date FROM gastos a " +
                                   "INNER JOIN categorias b ON a.CategoryID = b.ID " +
                                   "WHERE a.Date BETWEEN @FechaInicio AND @FechaFin " +
                                   "ORDER BY a.Date ASC";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RegistroGastos registroGastos = new RegistroGastos
                                {
                                    ID = reader.GetInt32("ID"),
                                    CategoryID = reader.GetInt32("CategoryID"),
                                    CategoryName = reader.GetString("CategoryName"),
                                    Concepto = reader.GetString("Concepto"),
                                    Amount = reader.GetFloat("Amount"),
                                    Date = reader.GetDateTime("Date")
                                };
                                registrosGastos.Add(registroGastos);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            return registrosGastos;
        }


        public void InsertarRegistrosGastos(int? CategoryID, string? Concepto, float? Amount, DateTime Date)
        {
            string connectionString = "server=localhost;database=control_gastos;uid=root;password=Qw3rtyu1";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string queryUltimoSaldo = "SELECT SaldoTotal FROM movimientos ORDER BY ID DESC LIMIT 1";
                    MySqlCommand cmdUltimoSaldo = new MySqlCommand(queryUltimoSaldo, connection);
                    object ultimoSaldoObj = cmdUltimoSaldo.ExecuteScalar();
                    float ultimoSaldo = 0; // Valor predeterminado
                    if (ultimoSaldoObj != null && !DBNull.Value.Equals(ultimoSaldoObj))
                    {
                        ultimoSaldo = Convert.ToSingle(ultimoSaldoObj);
                    }

                    // Insertar el gasto en la tabla 'gastos'
                    string queryGastos = "INSERT INTO gastos (CategoryID, Concepto, Amount, Date) VALUES (@CategoryID, @Concepto, @Amount, @Date)";
                    MySqlCommand cmdGastos = new MySqlCommand(queryGastos, connection);
                    cmdGastos.Parameters.AddWithValue("@CategoryID", CategoryID);
                    cmdGastos.Parameters.AddWithValue("@Concepto", Concepto);
                    cmdGastos.Parameters.AddWithValue("@Amount", Amount);
                    cmdGastos.Parameters.AddWithValue("@Date", Date);
                    cmdGastos.ExecuteNonQuery();

                    // Insertar el mismo gasto en la tabla 'movimientos'
                    string queryMovimientos = "INSERT INTO movimientos (CategoryID, Concepto, Amount, Date, Tipo, SaldoTotal) VALUES (@CategoryID, @Concepto, @Amount, @Date, 'Gasto', @NuevoSaldo)";
                    MySqlCommand cmdMovimientos = new MySqlCommand(queryMovimientos, connection);
                    cmdMovimientos.Parameters.AddWithValue("@CategoryID", CategoryID);
                    cmdMovimientos.Parameters.AddWithValue("@Concepto", Concepto);
                    cmdMovimientos.Parameters.AddWithValue("@Amount", Amount);
                    cmdMovimientos.Parameters.AddWithValue("@Date", Date);
                    float nuevoSaldo = ultimoSaldo - (Amount ?? 0); // Calcular el nuevo saldo total
                    cmdMovimientos.Parameters.AddWithValue("@NuevoSaldo", nuevoSaldo);
                    cmdMovimientos.ExecuteNonQuery();
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
                finally { connection.Close(); }
            }
        }

        public List<ResumenGastosPorCategoria> ResumenGastosPorCategoria()
        {
            List<ResumenGastosPorCategoria> ResumenGastosPorCategoria = new List<ResumenGastosPorCategoria>();
            string connectionStrig = "server=localhost;database=control_gastos;uid=root;password=Qw3rtyu1";

            using (MySqlConnection connection = new MySqlConnection(connectionStrig))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT b.CategoryName, SUM(a.Amount) AS TotalAmount FROM gastos a " +
                        "INNER JOIN categorias b ON a.CategoryID = b.ID " +
                        "GROUP BY b.CategoryName";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ResumenGastosPorCategoria ResumenGastos = new ResumenGastosPorCategoria
                                {
                                    CategoryName = reader.GetString("CategoryName"),
                                    Amount = reader.GetFloat("TotalAmount"),
                                };
                                ResumenGastosPorCategoria.Add(ResumenGastos);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally { connection.Close(); }
            }
            return ResumenGastosPorCategoria;
        }

        public void InsertarRegistrosIngresos(int? CategoryID, string? Concepto, float? Amount, DateTime Date)
        {
            string connectionString = "server=localhost;database=control_gastos;uid=root;password=Qw3rtyu1";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Obtener el último saldo total
                    string queryUltimoSaldo = "SELECT SaldoTotal FROM movimientos ORDER BY ID DESC LIMIT 1";
                    MySqlCommand cmdUltimoSaldo = new MySqlCommand(queryUltimoSaldo, connection);
                    object ultimoSaldoObj = cmdUltimoSaldo.ExecuteScalar();
                    float ultimoSaldo = 0; // Valor predeterminado
                    if (ultimoSaldoObj != null && !DBNull.Value.Equals(ultimoSaldoObj))
                    {
                        ultimoSaldo = Convert.ToSingle(ultimoSaldoObj);
                    }

                    // Insertar el mismo gasto en la tabla 'movimientos'
                    string queryMovimientos = "INSERT INTO movimientos (CategoryID, Concepto, Amount, Date, Tipo, SaldoTotal) VALUES (@CategoryID, @Concepto, @Amount, @Date, 'Ingreso', @NuevoSaldo)";
                    MySqlCommand cmdMovimientos = new MySqlCommand(queryMovimientos, connection);
                    cmdMovimientos.Parameters.AddWithValue("@CategoryID", CategoryID);
                    cmdMovimientos.Parameters.AddWithValue("@Concepto", Concepto);
                    cmdMovimientos.Parameters.AddWithValue("@Amount", Amount);
                    cmdMovimientos.Parameters.AddWithValue("@Date", Date);
                    float nuevoSaldo = ultimoSaldo + (Amount ?? 0); // Calcular el nuevo saldo total
                    cmdMovimientos.Parameters.AddWithValue("@NuevoSaldo", nuevoSaldo);
                    cmdMovimientos.ExecuteNonQuery();
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
                finally { connection.Close(); }
            }
        }

        public float ObtenerSaldoTotal()
        {
            float SaldoTotal = 0;
            string connectionString = "server=localhost;database=control_gastos;uid=root;password=Qw3rtyu1";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT SaldoTotal FROM movimientos ORDER BY ID DESC LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            SaldoTotal = Convert.ToSingle(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener el último saldo total: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            return SaldoTotal;
        }


    }
}

