using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pizzaria.Core.Model;
using Pizzaria.Core.Util;
using Pizzaria.Core.VO;
using System.Diagnostics;
using Pizzaria.Components.Web.DataTablesBinder.Interfaces;
using userModel = Pizzaria.Core.Model.UserViewModels;
using roleModel = Pizzaria.Core.Model.RoleViewModels;
using Pizzaria.Security.Data;

namespace Pizzaria.Data
{
    public class UsuarioDao : BaseDao
    {
        Logger<UsuarioDao> log;

        public UsuarioDao()
        {
            log = new Logger<UsuarioDao>();
            log.init();
        }

        public IList<userModel.DetailsViewModel> ListarUsuarios(List<userModel.DetailsViewModel> users, IDataTablesFilterRequest<userModel.IndexViewModel> tableQuery, out int total)
        {
            try
            {
                var filtros = tableQuery.FilterModel;
                var retorno = new List<userModel.DetailsViewModel>();

                if (string.IsNullOrEmpty(filtros.Filtro))
                {
                    retorno.AddRange(users);
                }
                else
                {
                    retorno.AddRange(users.Where(u => u.Email.Contains(filtros.Filtro) || u.Nome.Contains(filtros.Filtro) || u.Username.Contains(filtros.Filtro)).ToList());
                }

                total = retorno.Count();
                var ret = retorno.AsQueryable();
                ret = AplicarPaginacaoOrdenacaoDataTable(ret, tableQuery);

                return ret.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex, "ListarUsuarios");
                throw ex;
            }
        }

        public IList<ApplicationRole> ListarRoles(List<ApplicationRole> users, IDataTablesFilterRequest<roleModel.IndexViewModel> tableQuery, out int total)
        {
            try
            {
                var filtros = tableQuery.FilterModel;
                var retorno = new List<ApplicationRole>();

                if (string.IsNullOrEmpty(filtros.Filtro))
                {
                    retorno.AddRange(users);
                }
                else
                {
                    retorno.AddRange(users.Where(u => u.NameWithoutApplication.Contains(filtros.Filtro)).ToList());
                }

                total = retorno.Count();
                var ret = retorno.AsQueryable();
                ret = AplicarPaginacaoOrdenacaoDataTable(ret, tableQuery);

                return ret.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex, "ListarRoles");
                throw ex;
            }
        }
    }
}
