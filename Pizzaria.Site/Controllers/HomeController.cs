using System;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.Web.Mvc;
using Pizzaria.Core.Model;
using Pizzaria.Core.VO;
using Pizzaria.Data;
using System.Transactions;
using System.Drawing;
using System.IO.Ports;
using System.Management;
using System.IO;
using System.Text;
using Pizzaria.Core.Util;

namespace Pizzaria.Site.Controllers
{
    public class HomeController : BaseController
    {
        #region [ Atributos ]

        private ClienteDao clienteDao;
        private ProdutoDao produtoDao;
        private PedidoDao pedidoDao;
        private Logger<HomeController> log;

        #endregion

        #region [ Initialize e Dispose ]

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            clienteDao = new ClienteDao();
            produtoDao = new ProdutoDao();
            pedidoDao = new PedidoDao();

            log = new Logger<HomeController>();
            log.init();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (null != clienteDao) clienteDao.Dispose();
                if (null != produtoDao) produtoDao.Dispose();
                if (null != pedidoDao) pedidoDao.Dispose();
            }
        }

        #endregion

        public ActionResult Index()
        {
            ViewBag.Brinde = false;
            ViewBag.ItensAdicionais = produtoDao.ListarItensAdicionais();
            ViewBag.ProdutosCondicionais = produtoDao.ListarProdutosCondicionais().ToArray();
            Session["Carrinho"] = new List<OrderItemModel>();
            Session["Cliente"] = new ClienteVO();

            return View();
        }

        [HttpPost]
        public ActionResult PesquisarCliente(string telefone)
        {
            var filtro = LimparTelefone(telefone);
            var cliente = clienteDao.PesquisarTelefone(filtro);
            if (cliente != null)
            {
                Session["Cliente"] = cliente;
                return Json(new { ok = true, cliente = cliente, numero = filtro }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { ok = false, numero = filtro }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GravarCliente(HomeModel model)
        {
            ClienteVO cliente = null;
            var vo = new ClienteVO() 
            { 
                Nome = model.Nome,
                Telefone = model.Telefone,
                Celular = model.Celular
            };
            if (!string.IsNullOrEmpty(model.DataNascimento))
            {
                vo.DataNascimento = Convert.ToDateTime(model.DataNascimento);
            }

            using (var scope = new TransactionScope())
            {
                try
                {
                    var idCliente = clienteDao.InserirCliente(vo);
                    if (idCliente > 0)
                    {
                        var voEndereco = new EnderecoVO
                        {
                            ClienteId = (int)idCliente,
                            Logradouro = model.Logradouro,
                            Numero = model.Numero,
                            Complemento = model.Complemento,
                            Bairro = model.Bairro,
                            Cidade = model.Cidade,
                            Estado = model.Estado,
                            CEP = model.CEP,
                            EnderecoPadrao = true
                        };
                        voEndereco.Id = (int)clienteDao.InserirEndereco(voEndereco);
                        cliente = clienteDao.Obter((int)idCliente);
                        cliente.EnderecoPadrao = voEndereco;
                        Session["Cliente"] = cliente;
                    }
                    scope.Complete();
                }
                catch
                {
                    scope.Dispose();
                }
            }

            return Json(new { ok = cliente != null, cliente = cliente }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GravarEndereco(HomeModel model)
        {
            var cliente = (ClienteVO)Session["Cliente"];

            var vo = new EnderecoVO
            {
                ClienteId = cliente.Id,
                Logradouro = model.NovoLogradouro,
                Numero = model.NovoNumero,
                Complemento = model.NovoComplemento,
                Bairro = model.NovoBairro,
                Cidade = model.NovoCidade,
                Estado = model.NovoEstado,
                CEP = model.NovoCEP,
                EnderecoPadrao = true
            };
            var idEndereco = clienteDao.InserirEndereco(vo);
            vo.Id = (int)idEndereco;

            cliente = clienteDao.Obter(cliente.Id);
            cliente.EnderecoPadrao = vo;

            Session["Cliente"] = cliente;

            var carrinho = (List<OrderItemModel>)Session["Carrinho"];
            var taxaEntrega = pedidoDao.ObterTaxaEntrega(cliente.EnderecoPadrao.Bairro.ToUpper());
            var descricaoCarrinho = produtoDao.MontarDescricaoCarrinho(carrinho);
            var totalCarrinho = produtoDao.CalcularTotal(carrinho, taxaEntrega).ToString("C");

            return Json(
                new
                {
                    ok = cliente != null,
                    cliente = cliente,
                    totalCarrinho = totalCarrinho,
                    descricaoCarrinho = descricaoCarrinho,
                    taxaEntrega = taxaEntrega.ToString("C")
                }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ListarTipos(string filtro, string tamanho)
        {
            var result = produtoDao.ListarTipos(filtro, false, tamanho == "BROTO");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ListarSubTipos(int idTipo)
        {
            try
            {
                var result = produtoDao.ListarSubTipos(idTipo);
                return Json(new { ok = true, lista = result }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListarProdutos(string filtro, int subtipo)
        {
            try
            {
                var result = produtoDao.ListarProdutos(filtro, true, subtipo);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListarIngredientes()
        {
            try
            {
                var result = produtoDao.ListarIngredientes();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListarBebidas(string filtro)
        {
            var result = produtoDao.ListarProdutos(filtro, false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RepetirUltimoPedido(int idCliente, int idEndereco, int idPedido, string formaPagamento)
        {
            var result = pedidoDao.RepetirUltimoPedido(UserId, idCliente, idEndereco, idPedido, formaPagamento);
            Session["Carrinho"] = new List<OrderItemModel>();

            return Json(new { ok = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LimparCarrinho()
        {
            Session["Carrinho"] = new List<OrderItemModel>();
            return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FecharPedido(int idCliente, int idEndereco, string bairro, string formaPagamento, string observacao, bool removerTaxa)
        {
            try
            {
                var result = pedidoDao.FecharPedido(UserId, idCliente, idEndereco, bairro, formaPagamento, (List<OrderItemModel>)Session["Carrinho"], observacao, removerTaxa);
                Session["Carrinho"] = new List<OrderItemModel>();
                return Json(new { ok = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return Json(new { ok = false, mensagem = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult InserirCarrinho(OrderItemModel model)
        {
            try
            {
                var cliente = (ClienteVO)Session["Cliente"];

                var carrinho = (List<OrderItemModel>)Session["Carrinho"];

                if (model.IdTipo == 0 || model.Quantidade == 0 || model.SubTipos.Count == 0)
                {
                    return Json(
                    new
                    {
                        ok = false,
                        mensagem = "Informe os dados da pizza corretamente!"
                    }, JsonRequestBehavior.AllowGet);
                }

                carrinho.Add(model);

                var brinde = string.Empty;
                var possuiBrinde = produtoDao.PossuiBrinde(carrinho, out brinde);
                var taxaEntrega = pedidoDao.ObterTaxaEntrega(cliente.EnderecoPadrao.Bairro.ToUpper());
                var descricaoCarrinho = produtoDao.MontarDescricaoCarrinho(carrinho);
                var totalCarrinho = produtoDao.CalcularTotal(carrinho, taxaEntrega).ToString("C");
                Session["Carrinho"] = carrinho;

                return Json(
                    new
                    {
                        ok = true,
                        totalCarrinho = totalCarrinho,
                        descricaoCarrinho = descricaoCarrinho,
                        taxaEntrega = taxaEntrega.ToString("C"),
                        possuiBrinde = possuiBrinde,
                        brinde = brinde
                    }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                log.Error(ex.ToString());
                return Json(new { ok = false, mensagem = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
