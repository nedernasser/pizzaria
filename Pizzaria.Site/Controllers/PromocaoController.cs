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
    public class PromocaoController : BaseController
    {
        #region [ Atributos ]

        private ProdutoDao produtoDao;

        #endregion

        #region [ Initialize e Dispose ]

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            produtoDao = new ProdutoDao();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (null != produtoDao) produtoDao.Dispose();
            }
        }

        #endregion

        public ActionResult Index()
        {
            ViewBag.ListaBrinde = produtoDao.ListarBrindes();
            return View();
        }

        [HttpPost]
        public JsonResult CarregarDataTable([ModelBinder(typeof(DataTablesFilterBinder<PromocaoModel>))] IDataTablesFilterRequest<PromocaoModel> tableQuery)
        {
            try
            {
                int total = 0;
                IList<PromocaoVO> listaPromocoes = produtoDao.ListarPromocoes(tableQuery, out total);
                return Json(new DataTablesResponse(tableQuery.Draw, listaPromocoes, total, total));
            }
            catch
            {
                return Json(new DataTablesResponse(tableQuery.Draw, new List<PromocaoModel>(), 0, 0));
            }
        }

        [HttpGet]
        public ActionResult ObterPromocao(int id)
        {
            try
            {
                var promocao = produtoDao.ObterPromocao(id);
                return Json(new { ok = promocao != null, promocao = promocao }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GravarPromocao(PromocaoModel model)
        {
            var desconto = float.Parse(model.Desconto.Replace(" %", string.Empty));
            var vo = new PromocaoVO()
            {
                Id = model.Id,
                Descricao = model.Descricao,
                Domingo = model.Domingo,
                Segunda = model.Segunda,
                Terca = model.Terca,
                Quarta = model.Quarta,
                Quinta = model.Quinta,
                Sexta = model.Sexta,
                Sabado = model.Sabado,
                Desconto = desconto,
                PossuiBrinde = model.PossuiBrinde,
                BrindeId = model.BrindeId
            };

            try
            {
                var resultado = produtoDao.GravarPromocao(vo);
                return Json(new { ok = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}