using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RastradorPreco.Models;
using System.Collections.Generic;

namespace RastradorPreco.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly string _connectionString = "Server=DESKTOP-O1775Q7;Database=RastreadorDb;User Id=sa;Password=123456;Trusted_Connection=True;TrustServerCertificate=True;";
        private readonly SqlConnection _connection;

        public ProdutosController()
        {
            _connection = new SqlConnection(_connectionString);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                List<Produto> listaProdutos = new List<Produto>();
                string selecaoQuery = "SELECT * FROM produtos";

                SqlCommand comando = new SqlCommand(selecaoQuery, _connection);
                comando.CommandType = CommandType.Text;

                await _connection.OpenAsync();
                SqlDataReader leitorDados = await comando.ExecuteReaderAsync();

                while (await leitorDados.ReadAsync())
                {
                    Produto produto = new Produto();
                    produto.ProdutoId = Convert.ToInt32(leitorDados["ProdutoId"]);
                    produto.Nome = leitorDados["Nome"].ToString();
                    produto.LinkProduto = leitorDados["LinkProduto"].ToString();
                    listaProdutos.Add(produto);
                }

                return View(listaProdutos);
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

        [HttpGet]
        public IActionResult NovoProduto()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NovoProduto(Produto produto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string insercaoQuery = "INSERT INTO Produtos(Nome, LinkProduto) VALUES(@Nome, @LinkProduto)";
                    SqlCommand comando = new SqlCommand(insercaoQuery, _connection);
                    comando.CommandType = CommandType.Text;

                    comando.Parameters.AddWithValue("@Nome", produto.Nome);
                    comando.Parameters.AddWithValue("@LinkProduto", produto.LinkProduto);

                    await _connection.OpenAsync();
                    await comando.ExecuteNonQueryAsync();

                    return RedirectToAction(nameof(Index));
                }

                return View(produto);
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

        [HttpGet]
        public async Task<IActionResult> AtualizarProduto(int produtoId)
        {
            try
            {
                string selecaoQuery = String.Format("SELECT * FROM Produtos WHERE ProdutoId = {0}", produtoId);
                SqlCommand comando = new SqlCommand(selecaoQuery, _connection);
                Produto produto = new Produto();

                await _connection.OpenAsync();
                SqlDataReader leitorDados = await comando.ExecuteReaderAsync();

                while (await leitorDados.ReadAsync())
                {
                    produto.ProdutoId = Convert.ToInt32(leitorDados["ProdutoId"]);
                    produto.Nome = leitorDados["Nome"].ToString();
                    produto.LinkProduto = leitorDados["LinkProduto"].ToString();
                }

                return View(produto);

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

        [HttpPost]
        public async Task<IActionResult> AtualizarProduto(int produtoId, Produto produto)
        {
            try
            {
                if (produtoId != produto.ProdutoId)
                    return NotFound();

                if (ModelState.IsValid)
                {
                    string atualizacaoQuery = "UPDATE Produtos SET Nome = @Nome, LinkProduto = @LinkProduto WHERE ProdutoId = @ProdutoId";
                    SqlCommand comando = new SqlCommand(atualizacaoQuery, _connection);
                    comando.CommandType = CommandType.Text;

                    comando.Parameters.AddWithValue("@ProdutoId", produto.ProdutoId);
                    comando.Parameters.AddWithValue("@Nome", produto.Nome);
                    comando.Parameters.AddWithValue("@LinkProduto", produto.LinkProduto);

                    await _connection.OpenAsync();
                    await comando.ExecuteNonQueryAsync();

                    return RedirectToAction(nameof(Index));
                }

                return View(produto);
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

        [HttpPost]
        public async Task<IActionResult> ExcluirProduto(int produtoId)
        {
            try
            {
                string exclusãoQuery = "DELETE FROM Produtos WHERE ProdutoId = @ProdutoId";
                SqlCommand comando = new SqlCommand(exclusãoQuery, _connection);
                comando.CommandType = CommandType.Text;

                comando.Parameters.AddWithValue("@ProdutoId", produtoId);

                await _connection.OpenAsync();
                await comando.ExecuteNonQueryAsync();
                return RedirectToAction(nameof(Index));
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

        public async Task<IActionResult> DetalhesProduto(int produtoId)
        {
            try
            {
                ViewData["ProdutoId"] = produtoId;
                List<InformacoesConsulta> listaIC = new List<InformacoesConsulta>();
                string selecaoQuery = String.Format("SELECT Valor, Dia, Mes, Ano FROM InformacoesConsulta WHERE ProdutoId = {0}", produtoId);
                SqlCommand comando = new SqlCommand(selecaoQuery, _connection);

                await _connection.OpenAsync();
                SqlDataReader leitorDados = await comando.ExecuteReaderAsync();

                while (await leitorDados.ReadAsync())
                {
                    InformacoesConsulta ic = new InformacoesConsulta();
                    ic.Valor = Convert.ToDouble(leitorDados["Valor"]);
                    ic.Dia = Convert.ToInt32(leitorDados["Dia"]);
                    ic.Mes = Convert.ToInt32(leitorDados["Mes"]);
                    ic.Ano = Convert.ToInt32(leitorDados["Ano"]);
                    listaIC.Add(ic);
                }

                return View(listaIC);
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

        public async Task<JsonResult> DadosGrafico(int produtoId, int diaInicio, int diaFim, int mes, int ano)
        {
            try
            {
                List<InformacoesConsulta> listaIC = new List<InformacoesConsulta>();
                string selecaoQuery = String.Format("SELECT Valor, Dia, Mes, Ano " +
                    "FROM InformacoesConsulta " +
                    "WHERE (produtoId = {0} AND Ano = {1} AND Mes = {2} AND (Dia >= {3} AND Dia <= {4}))", produtoId, ano, mes, diaInicio, diaFim);
                SqlCommand comando = new SqlCommand(selecaoQuery, _connection);

                await _connection.OpenAsync();
                SqlDataReader leitorDados = await comando.ExecuteReaderAsync();

                while (await leitorDados.ReadAsync())
                {
                    InformacoesConsulta ic = new InformacoesConsulta();
                    ic.Valor = Convert.ToDouble(leitorDados["Valor"]);
                    ic.Dia = Convert.ToInt32(leitorDados["Dia"]);
                    ic.Mes = Convert.ToInt32(leitorDados["Mes"]);
                    ic.Ano = Convert.ToInt32(leitorDados["Ano"]);
                    listaIC.Add(ic);
                }

                return Json(listaIC);
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
