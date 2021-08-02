using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using log4net.Config;

namespace Pizzaria.Core.Util
{
    public static class CustomLogging
    {
        public static readonly log4net.Core.Level logLevel = new log4net.Core.Level(1000, "LOG");
        public static void Log(this ILog log, string message)
        {
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, logLevel, message, null);
        }
    }

    public class Logger<T>
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(T));

        public void init()
        {
            LogManager.GetRepository().LevelMap.Add(CustomLogging.logLevel);
            XmlConfigurator.Configure();
        }

        private void EnviarEmail(string mensagem)
        {
            EmailUtil.EnviarEmail(new string[] { "nedernasser@gmail.com" }, "ERRO NO SITE", mensagem, true);
        }

        public void Information(string mensagem)
        {
            Trace.TraceInformation(mensagem);
            logger.Log(mensagem);
        }

        public void Information(string formatoParametros, params object[] valoresParametros)
        {
            Trace.TraceInformation(formatoParametros, valoresParametros);
            logger.Log(string.Format(formatoParametros, valoresParametros));
        }

        public void Information(Exception exception, string formatoParametros, params object[] valoresParametros)
        {
            Trace.TraceInformation(FormatExceptionmensagem(exception, formatoParametros, valoresParametros));
            logger.Log(FormatExceptionmensagem(exception, formatoParametros, valoresParametros));
        }

        public void Warning(string mensagem)
        {
            Trace.TraceWarning(mensagem);
            logger.Log(mensagem);
        }

        public void Warning(string formatoParametros, params object[] valoresParametros)
        {
            Trace.TraceWarning(formatoParametros, valoresParametros);
            logger.Log(string.Format(formatoParametros, valoresParametros));
        }

        public void Warning(Exception exception, string formatoParametros, params object[] valoresParametros)
        {
            Trace.TraceWarning(FormatExceptionmensagem(exception, formatoParametros, valoresParametros));
            logger.Log(FormatExceptionmensagem(exception, formatoParametros, valoresParametros));
        }

        public void Error(string mensagem)
        {
            Trace.TraceError(mensagem);
            logger.Log(mensagem);
            EnviarEmail(mensagem);
        }

        public void Error(string formatoParametros, params object[] valoresParametros)
        {
            Trace.TraceError(formatoParametros, valoresParametros);
            logger.Log(string.Format(formatoParametros, valoresParametros));
            EnviarEmail(string.Format(formatoParametros, valoresParametros));
        }

        public void Error(Exception exception, string formatoParametros, params object[] valoresParametros)
        {
            Trace.TraceError(FormatExceptionmensagem(exception, formatoParametros, valoresParametros));
            logger.Log(FormatExceptionmensagem(exception, formatoParametros, valoresParametros));
            EnviarEmail(FormatExceptionmensagem(exception, formatoParametros, valoresParametros));
        }

        public void Debug(string metodo, string mensagem)
        {
            Debug(null, metodo, mensagem, null, null);
        }

        public void Debug(string metodo, string mensagem, TimeSpan tempoExecucao)
        {
            Debug(null, metodo, mensagem, tempoExecucao, null);
        }

        public void Debug(string componente, string metodo, string mensagem, TimeSpan tempoExecucao)
        {
            Debug(componente, metodo, mensagem, tempoExecucao, null);
        }

        public void Debug(string componente, string metodo, string mensagem, TimeSpan tempoExecucao, string formatoParametros, params object[] valoresParametros)
        {
            Debug(componente, metodo, mensagem, tempoExecucao, string.Format(formatoParametros, valoresParametros));
        }
        public void Debug(string componente, string metodo, string mensagem, TimeSpan? tempoExecucao, string parametros)
        {
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrEmpty(componente))
                builder.AppendFormat("[{0}]", componente);
            else
                builder.AppendFormat("[{0}]", typeof(T).ToString());
            builder.AppendFormat("[{0}]", metodo);
            if (tempoExecucao.HasValue)
                builder.AppendFormat(" ({0})", tempoExecucao.Value.ToString());
            if (!string.IsNullOrEmpty(parametros))
                builder.AppendFormat(" -> ", parametros);
            if (!string.IsNullOrEmpty(mensagem))
                builder.AppendFormat(" -> {0}", mensagem);

            var messageError = builder.ToString();
            Trace.TraceInformation(messageError);
            logger.Log(messageError);
        }

        private string FormatExceptionmensagem(Exception exception, string formatoParametros, object[] valoresParametros)
        {
            var sb = new StringBuilder();
            sb.Append(string.Format(formatoParametros, valoresParametros));
            sb.Append(" Exception: ");
            sb.Append(exception.ToString());
            return sb.ToString();
        }
    }
}
