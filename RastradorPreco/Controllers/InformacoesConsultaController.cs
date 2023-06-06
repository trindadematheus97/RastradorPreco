using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using RastradorPreco.Models;
using System.Data.SqlClient;

namespace RastreadorPrecos.Controllers
{
    public class InformacoesConsultaController : Controller
    {
        private readonly string _connectionString = "Server=DESKTOP-O1775Q7;Database=RastreadorDb;User Id=sa;Password=123456;Trusted_Connection=True;TrustServerCertificate=True;";
        private readonly SqlConnection _connection;

        public InformacoesConsultaController()
        {
            _connection = new SqlConnection(_connectionString);
        }

        [HttpGet]
        public IActionResult NovaInformacao(int produtoId)
        {
            InformacoesConsulta ic = new InformacoesConsulta
            {
                ProdutoId = produtoId
            };
            return View(ic);
        }

        [HttpPost]
        public async Task<IActionResult> NovaInformacao(InformacoesConsulta ic)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string insercaoQuery = "INSERT INTO InformacoesConsulta(produtoId, Valor, Dia, Mes, Ano) VALUES(@ProdutoId, @Valor, @Dia, @Mes, @Ano)";
                    SqlCommand comando = new SqlCommand(insercaoQuery, _connection);
                    comando.CommandType = CommandType.Text;

                    comando.Parameters.AddWithValue("@produtoId", ic.ProdutoId);
                    comando.Parameters.AddWithValue("@Valor", ic.Valor);
                    comando.Parameters.AddWithValue("@Dia", ic.Dia);
                    comando.Parameters.AddWithValue("@Mes", ic.Mes);
                    comando.Parameters.AddWithValue("@Ano", ic.Ano);

                    await _connection.OpenAsync();
                    await comando.ExecuteNonQueryAsync();
                    return RedirectToAction("DetalhesProduto", "Produtos", new { produtoId = ic.ProdutoId });
                }

                return View(ic);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
    }
}