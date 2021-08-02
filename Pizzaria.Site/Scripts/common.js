$(document).ready(function () {

    // desativar enter em todas as páginas
    $('.page-content').on('keyup keypress', function (e) {
        var keyCode = e.keyCode || e.which;
        if (keyCode === 13) {
            e.preventDefault();
            return false;
        }
    });

    // Transformar para maiusculo
    $('input[type=text], input[type=search], textarea').bind('keyup', function (event) {
        var c = String.fromCharCode(event.keyCode);
        var isWordcharacter = c.match(/\w/);
        if (isWordcharacter) {
            var caret = getCaretPosition(this);
            this.value = this.value.toUpperCase();
            setCaretPosition(this, caret);
        }
    });

    // Configurando validator com o boostrap-select
    if ($('form')) {
        if ($('form').data("validator")) {
            $('form').data("validator").settings.ignore = ":not(select:hidden, input:visible, textarea:visible)";
        }
    }

    var formElements = function () {

        var feTimepicker = function () {
            // Default timepicker
            if ($(".timepicker").length > 0)
                $('.timepicker').timepicker();

            // 24 hours mode timepicker
            if ($(".timepicker24").length > 0)
                $(".timepicker24").timepicker({ minuteStep: 5, showSeconds: true, showMeridian: false });

        }

        //Bootstrap file input
        var feBsFileInput = function () {

            if ($("input.fileinput").length > 0)
                $("input.fileinput").bootstrapFileInput();

        }
        //END Bootstrap file input

        return {// Init all form element features
            init: function () {
                feBsFileInput();
            }
        }
    }();

    var tipoMensagem = getParameterByName('tipoMensagem');
    if (tipoMensagem != "") {
        ExibirMensagem(tipoMensagem);
    }

    $(".datepicker").datepicker({
        format: 'dd/mm/yyyy',
        language: 'pt-BR',
        todayHighlight: true,
        autoclose: true
    });
});

function GerarAlerta(type, text) {

    var n = noty({
        text: text,
        type: type,
        dismissQueue: true,
        layout: 'center',
        closeWith: ['button'],
        theme: 'metroui',
        timeout: 2000,
        progressBar: true,
        maxVisible: 10,
        animation: {
            open: { height: 'toggle' },
            close: { height: 'toggle' },
            easing: 'swing',
            speed: 200
        }
    });
    return n;
}

// Tratamento de Ajax
$(document).ajaxStart(function () {
    DisplayAguarde();
});
$(document).ajaxStop(function () {
    HideAguarde();
});
$(document).ajaxError(function (e, xhr) {
    if (xhr.status == 401)
        alert("Sua sessão expirou. Efetue o login novamente.");
    else if (xhr.status == 403)
        alert("Você não possui permissão para acessar esse recurso.");
});

// Retornar o valor da querystring
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

// Complementos do jQuery
$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        }
        else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

// Modal Aguarde
function DisplayAguarde() {
    if (!$("#mb-aguarde").hasClass("open")) $("#mb-aguarde").addClass("open");
}
function HideAguarde() {
    $("#mb-aguarde").removeClass("open");
}

// Resetar formulário
function resetValidation(formId) {
    $("#" + formId).validate().resetForm();
    $("#" + formId + " .form-group").removeClass("has-success").removeClass("has-error");
    $("#" + formId + " .form-group label").find('i.fa').remove();
}

// Alert de formulário
function DisplayAlert(controlIdToPrepend, type, icon, dismissable, message, autoClose) {
    var alertDiv = $("<div class='alert alert-" + type + "' role='alert' style='margin-left: 10px; width:98.2%;'></div>");
    if (icon) {
        alertDiv.append("<span class='fa fa-" + icon + "'></span>");
    }
    if (dismissable) {
        alertDiv.addClass("alert-dismissable");
        alertDiv.append("<button type='button' class='close' data-dismiss='alert'><span aria-hidden='true'>×</span><span class='sr-only'>Fechar</span></button>");
    }
    alertDiv.append(message);
    if (autoClose) {
        alertDiv.prependTo("#" + controlIdToPrepend).show('slow').delay(3000).fadeOut(300);
    } else {
        alertDiv.prependTo("#" + controlIdToPrepend).show('slow');
    }
}

// Limpar Campo do Formulario
function LimparCampoFormulario(nomeCampo) {
    $("#" + nomeCampo).closest('.form-group').removeClass('has-error').removeClass('has-success');
    $("label[for='" + $("#" + nomeCampo).attr('id') + "']").find('i.fa').remove();
    $("#" + nomeCampo).closest('.form-group').find('span.has-error').remove();
}

// Apresentar sucesso no fomrulário
function ApresentarSucesso(nomeCampo) {
    $("#" + nomeCampo).closest('.form-group').removeClass('has-error').addClass('has-success');
    $("label[for='" + $("#" + nomeCampo).attr('id') + "']").find('i.fa').remove();
    $("label[for='" + $("#" + nomeCampo).attr('id') + "']").prepend('<i class="fa fa-check">&nbsp;</i>');
    $("#" + nomeCampo).closest('.form-group').find('span.has-error').remove();
}

// Apresentar erro no formulário
function ApresentarErro(nomeCampo, mensagemErro) {
    $("#" + nomeCampo).closest('.form-group').addClass('has-error').removeClass('has-success');
    $("#" + nomeCampo).closest('.form-group').find('span.has-error').remove();
    $("#" + nomeCampo).closest('.form-group').append("<span class='has-error'>" + mensagemErro + "</span>");
    $("label[for='" + $("#" + nomeCampo).attr('id') + "']").find('i.fa').remove();
    $("label[for='" + $("#" + nomeCampo).attr('id') + "']").prepend('<i class="fa fa-times-circle-o">&nbsp;</i>');
}

// formatar data em JSON
function FormatarDataJson(dataJson, somenteData) {
    if (dataJson != undefined && dataJson != "") {
        var date = new Date(parseInt(dataJson.substr(6)));
        if (somenteData && somenteData == true)
            return Globalize.formatDate(date, { date: "short" });
        else
            return Globalize.formatDate(date, { datetime: "short" });
    }
    else
        return "";
}

function FormatarDataHoraJson(dataJson, somenteData) {
    if (dataJson != undefined && dataJson != "") {
        var date = new Date(parseInt(dataJson.substr(6)));
        return Globalize.formatDate(date, { time: "short" });
    }
    else
        return "";
}

// Funções para o AllCaps
function getCaretPosition(ctrl) {
    var CaretPos = 0;    // IE Support
    if (document.selection) {
        ctrl.focus();
        var Sel = document.selection.createRange();
        Sel.moveStart('character', -ctrl.value.length);
        CaretPos = Sel.text.length;
    }
        // Firefox support
    else if (ctrl.selectionStart || ctrl.selectionStart == '0') {
        CaretPos = ctrl.selectionStart;
    }

    return CaretPos;
}

function setCaretPosition(ctrl, pos) {
    if (ctrl.setSelectionRange) {
        ctrl.focus();
        ctrl.setSelectionRange(pos, pos);
    }
    else if (ctrl.createTextRange) {
        var range = ctrl.createTextRange();
        range.collapse(true);
        range.moveEnd('character', pos);
        range.moveStart('character', pos);
        range.select();
    }
}

function replaceAll(valor, caracterParaSubstituir, novoCaractere) {
    while (valor.indexOf(caracterParaSubstituir) != -1) {
        valor = valor.replace(caracterParaSubstituir, novoCaractere);
    }
    return valor;
}

// Função para obter a data atual sem hora
function Today() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }

    today = dd + '/' + mm + '/' + yyyy;

    return today;
}

function NowHour() {
    var today = new Date();
    var d = new Date();
    var h = d.getHours();
    var m = d.getMinutes();
    var s = d.getSeconds();

    today = h + ':' + m + ':00';

    return today;
}

function ExibirMensagem(controller) {
    var tipoMensagem;
    var icon;
    var tipo = getParameterByName('resultado');

    switch (tipo) {
        case "1":
            tipoMensagem = "success";
            icon = "check";
            break;
        case "2":
            tipoMensagem = "info";
            icon = "info";
            break;
        case "3":
            tipoMensagem = "danger";
            icon = "exclamation-triangle";
            break;
    }

    var ehFeminino = controller.substr(controller.length - 2) == "ão";
    if (!ehFeminino) {
        ehFeminino = controller.substr(controller.length - 1) == "a";
    }
    var tipoAcao = getParameterByName('tipoAcao');

    var auxMensagem = "inserido";
    switch (tipoAcao) {
        case "1":
            if (ehFeminino && controller != "Sistema") {
                auxMensagem = "inserida";
            }
            break;
        case "2":
            auxMensagem = "alterado";
            if (ehFeminino && controller != "Sistema") {
                auxMensagem = "alterada";
            }
            break;
        case "3":
            auxMensagem = "excluído";
            if (ehFeminino && controller != "Sistema") {
                auxMensagem = "excluída";
            }
            break;
        case "4":
            auxMensagem = "enviado";
            if (ehFeminino && controller != "Sistema") {
                auxMensagem = "enviada";
            }
            break;
    }

    var mensagem = controller + " " + auxMensagem + " com sucesso!"
    var resultadoProcessamento = getParameterByName('resultado');

    if (resultadoProcessamento == "3") {
        mensagem = "Ocorreu um erro no processamento do seu pedido.";
    }

    //$("#" + idMensagem).html(mensagem);
    //if (!$("#" + tipoMensagem).hasClass("open")) $("#" + tipoMensagem).addClass("open");
    DisplayAlert("bodyContent", tipoMensagem, icon, true, mensagem, true);
}

function SubstituirCaracterReservado(texto) {
    var reservedCharacters = ["!", "#", "$", "&", "'", "(", ")", "*", "+", ",", ":", ";", "@", "[", "]"];
    var reservedPercentEncoding = ["%21", "%23", "%24", "%26", "%27", "%28", "%29", "%2A", "%2B", "%2C", "%3A", "%3B", "%40", "%5B", "%5D"];

    for (i = 0; i < reservedCharacters.length; i++) {
        texto = replaceAll(texto, reservedCharacters[i], reservedPercentEncoding[i]);
    }
    return texto;
}

function AbrirTrocarSenha() {
    $("#SenhaAtual").val('');
    $("#SenhaNova").val('');
    $("#ConfirmacaoSenhaNova").val('');
    $("#popupTrocaSenha").modal({ show: true, keyboard: false });
}

function TrocarSenha() {
    var resultado = "3";
    var senhaAtual = $("#SenhaAtual").val();
    var senhaNova = $("#SenhaNova").val();
    var confirmacaoSenhaNova = $("#ConfirmacaoSenhaNova").val();

    if (senhaNova != confirmacaoSenhaNova) {

    }

    var url = '/Usuario/AlterarSenha/';

    $.ajax({
        url: url,
        data: { SenhaAtual: senhaAtual, SenhaNova: senhaNova },
        cache: false,
        success: function (retorno) {
            $("#popupTrocaSenha").modal('hide');
            if (retorno.ok) {
                resultado = "1";
            }
            url = '/Home/Index/';
            url += '?tipoMensagem=Senha';
            url += '&resultado=' + resultado;
            url += '&tipoAcao=2';
            document.location.href = url;
        }
    });
}

function AbrirNovaMensagem() {
    $("#MensagemContato").val('');
    $("#popupContato").modal({ show: true, keyboard: false });
}

function EnviarContato() {
    var resultado = "3";
    var mensagemContato = $("#MensagemContato").val();

    if (mensagemContato.trim() == "") {

    }

    var url = '/Base/EnviarContato/';

    $.ajax({
        url: url,
        data: { MensagemContato: mensagemContato },
        cache: false,
        success: function (retorno) {
            $("#popupContato").modal('hide');
            if (retorno.ok) {
                resultado = "1";
            }
            url = '/Home/Index/';
            url += '?tipoMensagem=Contato';
            url += '&resultado=' + resultado;
            url += '&tipoAcao=4';
            document.location.href = url;
        }
    });
}