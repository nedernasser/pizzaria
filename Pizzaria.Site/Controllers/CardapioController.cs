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
    public class CardapioController : BaseController
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
            ViewBag.ListaPromocao = produtoDao.ListarPromocoes();
            return View();
        }

        [HttpPost]
        public JsonResult CarregarDataTable([ModelBinder(typeof(DataTablesFilterBinder<CardapioModel>))] IDataTablesFilterRequest<CardapioModel> tableQuery)
        {
            try
            {
                int total = 0;
                IList<ProdutoVO> listaProdutos = produtoDao.ListarProdutos(tableQuery, out total);
                return Json(new DataTablesResponse(tableQuery.Draw, listaProdutos, total, total));
            }
            catch
            {
                return Json(new DataTablesResponse(tableQuery.Draw, new List<CardapioModel>(), 0, 0));
            }
        }

        [HttpGet]
        public ActionResult ObterProduto(int id)
        {
            try
            {
                var produto = produtoDao.ObterProduto(id);
                return Json(new { ok = produto != null, produto = produto }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GravarProduto(CardapioModel model)
        {
            var valor = float.Parse(model.Valor.Replace("R$ ", string.Empty));
            var broto = float.Parse(model.Broto.Replace("R$ ", string.Empty));
            var vo = new ProdutoVO()
            {
                Id = model.Id,
                Nome = model.Nome,
                Descricao = model.Descricao,
                Valor = valor,
                Broto = broto,
                Promocao = model.Promocao
            };

            try
            {
                var resultado = produtoDao.AtualizarProduto(vo);
                return Json(new { ok = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }

            
        }
    }
}