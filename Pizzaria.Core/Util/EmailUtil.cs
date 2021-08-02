using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pizzaria.Core.Util
{
    /// <summary>
    /// Classe utilitária para envio de e-mails
    /// </summary>
    public class EmailUtil
    {
        public static bool EnviarEmail(string[] destinatarios, string assunto, string texto, bool html, string[] copiaDestinatarios = null, string[] copiaOcultaDestinatarios = null, string[] anexos = null)
        {
            var log = new Logger<EmailUtil>();
            log.init();

            try
            {
                Enviar(destinatarios, assunto, texto, html, null, copiaDestinatarios, copiaOcultaDestinatarios, anexos);

                return true;
            }
            catch (SmtpException ex)
            {
                log.Error("EnviarEmail => {0}", string.Format("Erro de SMTP => ", ex.Message));
                return false;
            }
            catch (ApplicationException ex)
            {
                log.Error("EnviarEmail => {0}", string.Format("Erro de Aplicação => ", ex.Message));
                return false;
            }
            catch (Exception ex)
            {
                log.Error("EnviarEmail => {0}", string.Format("Erro não tratado => ", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Método para envio de e-mail
        /// </summary>
        /// <param name="destinatarios">Array com os destinatários</param>
        /// <param name="assunto">Assunto do e-mail</param>
        /// <param name="corpo">Corpo do e-mail</param>
        /// <param name="isHtml">Indica se o e-mail é no formato HTML ou não</param>
        /// <param name="remetente">Remetente do e-mail (opcional)</param>
        /// <param name="copiaDestinatarios">Array com os destinatários em cópia (opcional)</param>
        /// <param name="copiaOcultaDestinatarios">Array com os destinatários em cópia oculta (opcional)</param>
        /// <param name="anexos">Array com os anexos (opcional)</param>
        private static bool Enviar(string[] destinatarios, string assunto, string corpo, bool isHtml, string remetente = null, string[] copiaDestinatarios = null, string[] copiaOcultaDestinatarios = null, string[] anexos = null)
        {
            // Declaração de variáveis:
            string servidorSMTP;
            int portaSMTP;
            string usuarioSMTP;
            string senhaSMTP;
            string remetentePadrao;
            string aliasRemetentePadrao;

            SmtpClient cliente;
            MailAddress enderecoRemetente;
            MailMessage mensagem;

            if ((destinatarios == null) && (copiaDestinatarios == null) && (copiaOcultaDestinatarios == null))
                throw new ApplicationException("É necessário informar ao menos um destinatário");

            try
            {
                // Obtendo configurações
                servidorSMTP = ConfigurationManager.AppSettings.Get("smtp_host");
                portaSMTP = int.Parse(ConfigurationManager.AppSettings.Get("smtp_port"));
                usuarioSMTP = ConfigurationManager.AppSettings.Get("smtp_user");
                senhaSMTP = ConfigurationManager.AppSettings.Get("smtp_pass");
                remetentePadrao = ConfigurationManager.AppSettings.Get("smtp_from");
                aliasRemetentePadrao = ConfigurationManager.AppSettings.Get("smtp_from_alias");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erro ao obter as configurações de SMTP", ex);
            }

            try
            {
                using (cliente = new SmtpClient())
                {
                    cliente.Host = servidorSMTP;
                    cliente.Port = portaSMTP;
                    cliente.EnableSsl = false;
                    cliente.UseDefaultCredentials = true;
                    cliente.Credentials = new NetworkCredential(usuarioSMTP, senhaSMTP);

                    if (remetente == null)
                        enderecoRemetente = new MailAddress(remetentePadrao, aliasRemetentePadrao);
                    else
                        enderecoRemetente = new MailAddress(remetente, "Clube de Compras");

                    mensagem = new MailMessage();
                    mensagem.From = enderecoRemetente;
                    mensagem.Subject = assunto;

                    mensagem.IsBodyHtml = isHtml;
                    if (isHtml)
                        mensagem.BodyEncoding = Encoding.GetEncoding("iso-8859-1");
                    mensagem.Body = corpo;

                    foreach (string email in destinatarios)
                    {
                        if (ValidarEnderecoEmail(email))
                        {
                            MailAddress dest = new MailAddress(email);
                            mensagem.To.Add(dest);
                        }
                        else
                            throw new ApplicationException("Endereço de e-mail inválido: " + email);
                    }
                    if (copiaDestinatarios != null)
                    {
                        foreach (string email in copiaDestinatarios)
                        {
                            if (ValidarEnderecoEmail(email))
                            {
                                MailAddress dest = new MailAddress(email);
                                mensagem.CC.Add(dest);
                            }
                            else
                                throw new ApplicationException("Endereço de e-mail inválido (CC): " + email);
                        }
                    }
                    if (copiaOcultaDestinatarios != null)
                    {
                        foreach (string email in copiaOcultaDestinatarios)
                        {
                            if (ValidarEnderecoEmail(email))
                            {
                                MailAddress dest = new MailAddress(email);
                                mensagem.Bcc.Add(dest);
                            }
                            else
                                throw new ApplicationException("Endereço de e-mail inválido (CCO): " + email);
                        }
                    }

                    if (anexos != null)
                    {
                        foreach (string arquivoAnexo in anexos)
                        {
                            try
                            {
                                Attachment item = new Attachment(arquivoAnexo);
                                mensagem.Attachments.Add(item);
                            }
                            catch (Exception ex)
                            {
                                throw new ApplicationException("Erro ao anexar o arquivo: " + arquivoAnexo, ex);
                            }
                        }
                    }

                    if ((mensagem.To.Count == 0) && (mensagem.CC.Count == 0) && (mensagem.Bcc.Count == 0))
                        throw new ApplicationException("É necessário informar ao menos um destinatário");

                    try
                    {
                        cliente.Send(mensagem);
                        return true;
                    }
                    catch (SmtpException ex)
                    {
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Houve um erro ao enviar o e-mail", ex);
                    }
                }
            }
            catch (SmtpException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erro ao instanciar o componente de e-mail", ex);
            }
        }

        /// <summary>
        /// Verificar se um endereço de e-mail é valido
        /// </summary>
        /// <param name="enderecoEmail">endereço a ser validado</param>
        /// <returns>TRUE se um endereço foi validado, FALSE se não</returns>
        public static bool ValidarEnderecoEmail(string enderecoEmail)
        {
            try
            {
                //define a expressão regulara para validar o email
                string texto_Validar = enderecoEmail;

                Regex expressaoRegex = new Regex("^[\\w\\-]+(\\.[\\w\\-]+)*@([A-Za-z0-9-]+\\.)+[A-Za-z]{2,4}$");

                // testa o email com a expressão
                if (expressaoRegex.IsMatch(texto_Validar))
                {
                    // o email é valido
                    return true;
                }

                else
                {
                    // o email é inválido
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
