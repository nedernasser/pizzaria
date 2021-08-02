using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Pizzaria.Components.Web.DataTablesBinder.Interfaces;

namespace Pizzaria.Components.Web.DataTablesBinder
{
    public class DataTablesBinder : IModelBinder
    {
        #region [ Constantes Datatable ]

        private const string DrawFormatting = "draw";
        private const string StartFormatting = "start";
        private const string LengthFormatting = "length";
        private const string ColumnDataFormatting = "columns[{0}][data]";
        private const string ColumnNameFormatting = "columns[{0}][name]";
        private const string OrderColumnFormatting = "order[{0}][column]";
        private const string OrderDirectionFormatting = "order[{0}][dir]";
        private const string ColumnSearchableFormatting = "columns[{0}][searchable]";
        private const string ColumnOrderableFormatting = "columns[{0}][orderable]";
        private const string ColumnSearchValueFormatting = "columns[{0}][search][value]";
        private const string ColumnSearchRegexFormatting = "columns[{0}][search][regex]";
        private const string SearchValueFormatting = "search[value]";
        private const string SearchRegexValueFormatting = "search[regex]";

        #endregion

        #region [ Implementação IModelBinder ]

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // Capturando parâmetros do DataTables
            HttpRequestBase request = controllerContext.RequestContext.HttpContext.Request;
            NameValueCollection requestParameters = GetRequestParameters(request);
            IDataTablesRequest dataTableRequest = new DataTablesRequest
            {
                Draw = GetParam<int>(requestParameters, DrawFormatting),
                Start = GetParam<int>(requestParameters, StartFormatting),
                Length = GetParam<int>(requestParameters, LengthFormatting),
                Search = new DataTablesSearch(GetParam<string>(requestParameters, SearchValueFormatting), GetParam<bool>(requestParameters, SearchRegexValueFormatting)),
                Columns = GetColumns(requestParameters)
            };
            SetColumnOrdering(requestParameters, dataTableRequest.Columns);

            return dataTableRequest;
        }

        #endregion

        #region [ Métodos Datatables ]

        private NameValueCollection GetRequestParameters(HttpRequestBase request)
        {
            var method = request.HttpMethod.ToLower();
            if (method.Equals("get")) return request.QueryString;
            if (method.ToLower().Equals("post")) return request.Form;
            throw new ArgumentException(string.Format("The provided HTTP method ({0}) is not a valid method to use with DataTableBinder. Please, use HTTP GET or POST.", request.HttpMethod), "request");
        }

        private IEnumerable<IDataTablesColumn> GetColumns(NameValueCollection collection)
        {
            var columns = new List<IDataTablesColumn>();

            for (var i = 0; i < collection.Count; i++)
            {
                var columnData = GetParam<string>(collection, string.Format(ColumnDataFormatting, i));
                var columnName = GetParam<string>(collection, string.Format(ColumnNameFormatting, i));

                if (columnData != null && columnName != null)
                {
                    var columnSearchable = GetParam<bool>(collection, string.Format(ColumnSearchableFormatting, i));
                    var columnOrderable = GetParam<bool>(collection, string.Format(ColumnOrderableFormatting, i));
                    var columnSearchValue = GetParam<string>(collection, string.Format(ColumnSearchValueFormatting, i));
                    var columnSearchRegex = GetParam<bool>(collection, string.Format(ColumnSearchRegexFormatting, i));

                    columns.Add(new DataTablesColumn(columnData, columnName, columnSearchable, columnOrderable, columnSearchValue, columnSearchRegex));
                }
                else break;
            }

            return columns;
        }

        protected void SetColumnOrdering(NameValueCollection collection, IEnumerable<IDataTablesColumn> columns)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                var orderColumn = GetParam<int>(collection, String.Format(OrderColumnFormatting, i));
                var orderDirection = GetParam<string>(collection, String.Format(OrderDirectionFormatting, i));

                if (orderColumn <= -1 || string.IsNullOrWhiteSpace(orderDirection)) break;

                var column = columns.ElementAt(orderColumn);

                column.OrderIndex = i;
                column.SortDirection = orderDirection.ToLower().Equals("asc")
                    ? SortDirection.Ascending
                    : SortDirection.Descending;
            }
        }

        private TParam GetParam<TParam>(NameValueCollection collection, string key)
        {
            var collectionItem = collection[key];
            if (collectionItem == null) return default(TParam);
            return (TParam)Convert.ChangeType(collectionItem, typeof(TParam));
        }

        #endregion
    }
}
