// Configurar localização do jQuery
$.when(
    $.get(urlBase + "Scripts/cldr/main/pt/ca-gregorian.json"),
    $.get(urlBase + "Scripts/cldr/main/pt/numbers.json"),
    $.get(urlBase + "Scripts/cldr/main/pt/timeZoneNames.json"),
    $.get(urlBase + "Scripts/cldr/supplemental/likelySubtags.json"),
    $.get(urlBase + "Scripts/cldr/supplemental/numberingSystems.json"),
    $.get(urlBase + "Scripts/cldr/supplemental/timeData.json"),
    $.get(urlBase + "Scripts/cldr/supplemental/weekData.json")
).then(function () {
    return [].slice.apply(arguments, [0]).map(function (result) {
        return result[0];
    });
}).then(Globalize.load).then(function () {
    Globalize.locale("pt");
});