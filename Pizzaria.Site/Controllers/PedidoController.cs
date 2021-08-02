using System;
using System.Web.Mvc;
using Pizzaria.Components.Web.DataTablesBinder;
using Pizzaria.Components.Web.DataTablesBinder.Interfaces;
using Pizzaria.Core.VO;
using System.Collections.Generic;
using Pizzaria.Core.Model;
using Pizzaria.Data;

namespace Pizzaria.Site.Controllers
{
    public class PedidoController : BaseController
    {
        #region [ Atributos ]

        private PedidoDao pedidoDao;

        #endregion

        #region [ Initialize e Dispose ]

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            pedidoDao = new PedidoDao();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (null != pedidoDao) pedidoDao.Dispose();
            }
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CarregarDataTable([ModelBinder(typeof(DataTablesFilterBinder<PedidoPesquisaModel>))] IDataTablesFilterRequest<PedidoPesquisaModel> tableQuery)
        {
            try
            {
                int total = 0;
                var totais = string.Empty;
                IList<PedidoModel> listaPedidos = pedidoDao.ListarPedidos(tableQuery, out total, out totais);
                return Json(new DataTablesResponse(tableQuery.Draw, listaPedidos, total, total, totais));
            }
            catch
            {
                return Json(new DataTablesResponse(tableQuery.Draw, new List<CardapioModel>(), 0, 0));
            }
        }

        [HttpGet]
        public ActionResult ObterPedido(int id)
        {
            try
            {
                var pedido = pedidoDao.Obter(id);
                return Json(new { ok = pedido != null, pedido = pedido }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GravarResumo(
            string dataInicial, 
            string dataFinal, 
            string totalPizza, 
            string totalBebida, 
            string totalDinheiro, 
            string totalDebito, 
            string totalCredito, 
            string totalValeRefeicao,
            string totalGeral)
        {
            try
            {
                pedidoDao.GravarResumo(
                    new ResumoPedidoVO()
                    {
                        DataInicial = dataInicial,
                        DataFinal = dataFinal,
                        TotalPizza = totalPizza,
                        TotalBebida = totalBebida,
                        TotalDinheiro = totalDinheiro,
                        TotalDebito = totalDebito,
                        TotalCredito = totalCredito,
                        TotalValeRefeicao = totalValeRefeicao,
                        TotalGeral = totalGeral
                    });
                return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}