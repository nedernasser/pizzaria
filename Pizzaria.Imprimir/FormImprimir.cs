using Microsoft.Win32;
using Pizzaria.Core.Model;
using Pizzaria.Core.VO;
using Pizzaria.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pizzaria.Imprimir
{
    public partial class FormImprimir : Form
    {
        public FormImprimir()
        {
            InitializeComponent();
        }

        private enum TipoDocumento
        {
            Cupom = 1,
            Canhoto = 2,
            Resumo = 3
        }

        private TipoDocumento Tipo { get; set; }
        private StringBuilder Cupom { get; set; }
        private RectangleF CupomLayout { get; set; }
        private StringBuilder CanhotoCupom { get; set; }
        private RectangleF CanhotoCupomLayout { get; set; }
        private StringBuilder Resumo { get; set; }
        private RectangleF ResumoCupomLayout { get; set; }
        private PrintDocument pd { get; set; }

        private void FormImprimir_Load(object sender, EventArgs e)
        {
            var pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);
            PrintHelper.SetDefaultPrinter("POS-58");

            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(500);
            this.Hide();

            CupomLayout = new RectangleF(5, 5, 180, 500);
            CanhotoCupomLayout = new RectangleF(5, 5, 180, 180);
            ResumoCupomLayout = new RectangleF(5, 5, 180, 250);

            VerificarRegistro();

            var service = new PedidoDao();
            while (true)
            {
                var pedidos = service.ListarCuponsPendentes();
                if (pedidos.Count > 0)
                {
                    foreach (var pedido in pedidos)
                    {
                        Tipo = TipoDocumento.Cupom;
                        FormatarCupom(pedido);
                        pd.Print();
                        //DialogResult dialogResult = MessageBox.Show("Deseja imprimir o Canhoto do Pedido?", "Canhoto do Pedido", MessageBoxButtons.YesNo);
                        //if (dialogResult == DialogResult.Yes)
                        //{
                        //    ImprimindoCupom = false;
                        //    pd.Print();
                        //}
                        Tipo = TipoDocumento.Canhoto;
                        pd.Print();
                        service.AtualizarImpressaoCupom(pedido.Id);
                    }
                }

                var resumos = service.ListarResumosPendentes();
                if (resumos.Count > 0)
                {
                    Tipo = TipoDocumento.Resumo;
                    foreach (var resumo in resumos)
                    {
                        FormatarResumo(resumo);
                        pd.Print();
                        service.AtualizarImpressaoResumo(resumo.Id);
                    }
                }

                Thread.Sleep(2000);
            }
        }

        private void VerificarRegistro()
        {
            var currentUserRegistry = Registry.CurrentUser;
            var runRegistryKey = currentUserRegistry.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (runRegistryKey != null)
            {
                if (runRegistryKey.GetValue("PrintClient") == null)
                    runRegistryKey.SetValue("PrintClient", Application.ExecutablePath);
            }
        }

        private void FormatarCupom(PedidoModel model)
        {
            Cupom = new StringBuilder();

            //var pizzaria = ConfigurationManager.AppSettings["Pizzaria"].ToString();
            //Cupom.Append("------------------------------------");
            //Cupom.AppendFormat("\r\n{0}", pizzaria);
            //Cupom.Append("\r\n------------------------------------");

            Cupom.AppendFormat("\r\nPEDIDO Nº {0}", model.Id);
            Cupom.Append("\r\n_____________________");
            foreach (var item in model.Itens)
            {
                Cupom.AppendFormat("\r\n{0}", item.Detalhes);
                Cupom.AppendFormat("\r\n{0}", item.ItensAdicionais);
            }
            Cupom.Append("\r\n_____________________");
            Cupom.Append("\r\nForma de Pagamento:");
            Cupom.AppendFormat("\r\n{0}", model.FormaPagamento);
            Cupom.AppendFormat("\r\n\r\nTotal: {0}", model.ValorTotalFormatado);

            Cupom.Append("\r\n_____________________");
            Cupom.AppendFormat("\r\n{0}", model.Cliente);
            Cupom.AppendFormat("\r\n{0}, {1}", model.Endereco.Logradouro, model.Endereco.Numero);
            Cupom.AppendFormat("\r\n{0}, {1}", model.Endereco.Complemento, model.Endereco.Bairro);
            Cupom.AppendFormat("\r\nContato: {0}", model.Celular);
            Cupom.AppendLine("\r\n\r\n\r\n\r\n\r\n------------------------------------");

            CanhotoCupom = new StringBuilder();
            CanhotoCupom.Append("------------------------------------");
            CanhotoCupom.AppendFormat("\r\nPEDIDO Nº {0}", model.Id);
            CanhotoCupom.Append("\r\n------------------------------------");
            CanhotoCupom.Append("\r\nForma de Pagamento:");
            CanhotoCupom.AppendFormat("\r\n{0}", model.FormaPagamento);
            CanhotoCupom.AppendFormat("\r\n\r\nTotal: {0}", model.ValorTotalFormatado);
            CanhotoCupom.Append("\r\n------------------------------------");
        }

        private void FormatarResumo(ResumoPedidoVO vo)
        {
            Resumo = new StringBuilder();

            Resumo.Append("\r\nRESUMO DE PEDIDOS");
            Resumo.Append("\r\n_____________________");
            
            Resumo.AppendFormat("\r\nData Inicial: {0}", vo.DataInicial);
            Resumo.AppendFormat("\r\nData Final: {0}", vo.DataFinal);
            Resumo.AppendFormat("\r\nTotal Pizzas: {0}", vo.TotalPizza);
            Resumo.AppendFormat("\r\nTotal Bebidas: {0}", vo.TotalBebida);
            Resumo.AppendFormat("\r\nTotal Dinheiro: {0}", vo.TotalDinheiro);
            Resumo.AppendFormat("\r\nTotal Débito: {0}", vo.TotalDebito);
            Resumo.AppendFormat("\r\nTotal Crédito: {0}", vo.TotalCredito);
            Resumo.AppendFormat("\r\nTotal Vale Refeição: {0}", vo.TotalValeRefeicao);
            Resumo.AppendFormat("\r\nTotal Geral: {0}", vo.TotalGeral);
            Resumo.Append("\r\n------------------------------------");
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            try
            {
                var fontDoc = new Font("Arial", 10);
                var formato = new StringFormat();
                formato.LineAlignment = StringAlignment.Center;

                switch (Tipo)
                {
                    case TipoDocumento.Cupom:
                        ev.Graphics.DrawString(Cupom.ToString(), fontDoc, Brushes.Black, CupomLayout, formato);
                        break;
                    case TipoDocumento.Canhoto:
                        ev.Graphics.DrawString(CanhotoCupom.ToString(), fontDoc, Brushes.Black, CanhotoCupomLayout, formato);
                        break;
                    case TipoDocumento.Resumo:
                        ev.Graphics.DrawString(Resumo.ToString(), fontDoc, Brushes.Black, ResumoCupomLayout, formato);
                        break;
                }
            }
            catch
            {

            }
        }

        private void FormImprimir_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(500);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon.Visible = false;
            }
        }
    }
}
