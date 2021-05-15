let trs = document.getElementsByTagName("tbody")[0].getElementsByTagName('tr');
let stocks = [];
for (let tr of trs) {
    let symbol = tr.getAttribute('data-current-symbol');
    let spans = tr.querySelectorAll("span[data-ng-bind='cell']");

    stocks.push({
        symbol: symbol,
        name: spans[0].outerText,
        last: spans[1].outerText,
        change: spans[2].outerText,
        changePercent: spans[3].outerText,
        gapUp: spans[4].outerText,
        gapUpPercent: spans[5].outerText,
        high: spans[6].outerText,
        low: spans[7].outerText,
        volume: spans[8].outerText,
    })
}

JSON.stringify(stocks);