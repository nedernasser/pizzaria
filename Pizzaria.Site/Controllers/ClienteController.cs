using System;
using System.Web.Mvc;
using Pizzaria.Components.Web.DataTablesBinder;
using Pizzaria.Components.Web.DataTablesBinder.Interfaces;
using Pizzaria.Core.VO;
using System.Collections.Generic;
using Pizzaria.Core.Model;
using Pizzaria.Data;
using PassOn;

namespace Pizzaria.Site.Controllers
{
    public class ClienteController : BaseController
    {
        #region [ Atributos ]

        private ClienteDao clienteDao;

        #endregion

        #region [ Initialize e Dispose ]

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            clienteDao = new ClienteDao();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (null != clienteDao) clienteDao.Dispose();
            }
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CarregarDataTable([ModelBinder(typeof(DataTablesFilterBinder<ClienteModel>))] IDataTablesFilterRequest<ClienteModel> tableQuery)
        {
            try
            {
                int total = 0;
                IList<ClienteVO> listaClientes = clienteDao.ListarClientes(tableQuery, out total);
                return Json(new DataTablesResponse(tableQuery.Draw, listaClientes, total, total));
            }
            catch
            {
                return Json(new DataTablesResponse(tableQuery.Draw, new List<CardapioModel>(), 0, 0));
            }
        }

        public ActionResult Edit(int id)
        {
            var cliente = clienteDao.Obter(id);
            Session["Enderecos"] = cliente.ListaEnderecos;
            var model = Pass.On<ClienteModel>(cliente);
            return View(model);
        }

        [HttpPost]
        public JsonResult CarregarEnderecos([ModelBinder(typeof(DataTablesFilterBinder<ClienteModel>))] IDataTablesFilterRequest<ClienteModel> tableQuery)
        {
            try
            {
                int total = 0;
                IList<EnderecoVO> listaEnderecos = clienteDao.ListarEnderecos(tableQuery, out total);
                return Json(new DataTablesResponse(tableQuery.Draw, listaEnderecos, total, total));
            }
            catch
            {
                return Json(new DataTablesResponse(tableQuery.Draw, new List<CardapioModel>(), 0, 0));
            }
        }

        [HttpPost]
        public ActionResult Index(ClienteModel model)
        {
            DateTime? dtNascimento = null;
            if (!string.IsNullOrEmpty(model.DataNascimento))
            {
                dtNascimento = Convert.ToDateTime(model.DataNascimento);
            }

            var vo = new ClienteVO()
            {
                Id = model.Id,
                Nome = model.Nome,
                Celular = !string.IsNullOrEmpty(model.Celular) ? LimparTelefone(model.Celular) : string.Empty,
                Telefone = !string.IsNullOrEmpty(model.Telefone) ? LimparTelefone(model.Telefone) : string.Empty,
                DataNascimento = dtNascimento
            };

            try
            {
                var resultado = clienteDao.AtualizarCliente(vo);
                return Json(new { ok = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }

            
        }

        public ActionResult ObterEndereco(int id)
        {
            try
            {
                var result = clienteDao.ObterEndereco(id);
                return Json(new { ok = true, endereco = result }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GravarEndereco(ClienteModel model)
        {
            try
            {
                var vo = new EnderecoVO
                {
                    Id = model.EnderecoId,
                    Logradouro = model.Logradouro,
                    Numero = model.Numero,
                    Complemento = model.Complemento,
                    Bairro = model.Bairro,
                    Cidade = model.Cidade,
                    Estado = model.Estado,
                };
                var result = clienteDao.AlterarEndereco(vo);

                return Json(new { ok = result }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}