$(document).ready(function () {
    var containerID = '#curgraph';
    var chart = $(containerID).highcharts();
    var curSelected = "";
    var monthSelected = "";
    $("#CurrencyNames option:selected").each(function () {
        curSelected = $(this)[0].value;
    });
    $("#Months option:selected").each(function () {
        monthSelected = $(this)[0].value;
    });
    var chart_url = "/Home/UpdateChart";

    post_data({ Month: monthSelected, CurrencyCode: curSelected });

    $('select').change(function () {
        $("#CurrencyNames option:selected").each(function () {
            curSelected = $(this)[0].value;
        });
        $("#Months option:selected").each(function () {
            monthSelected = $(this)[0].value;
        });
        post_data({ Month: monthSelected, CurrencyCode: curSelected })
    });

    function OnSuccess(response) {
        //console.log(response);

        //http://api.highcharts.com/highcharts/ to change existing graph
        chart.setTitle({ text: response.title });
        chart.xAxis[0].setTitle({ text: response.month });
        chart.xAxis[0].setCategories(response.days);
        var rates = response.rates.map(function (item) { return parseFloat(item); });
        chart.series[0].setData(rates, true, true, false);
    }

    function OnErrorCall(response) { console.log(response); }

    function post_data(data_) {
        jsonData = JSON.stringify(data_);
        $.ajax({
            type: "POST",
            url: chart_url,
            data: jsonData,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccess,
            error: OnErrorCall
        });
    }
});