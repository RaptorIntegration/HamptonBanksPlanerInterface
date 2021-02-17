const v = new Vue({
    el: '#app',
    name: 'Dashboard',
    data: {
        Alarm: '',
        Severity: { low: false, medium: false, high: true },
        Bins: [],
        VPH: 0,
        PPH: 0,
        SmallCard: null,
        TopLeft: null,
        FBM: null,
        ProductMix: null,
        Pie: null
    },
    mounted: function () {
        this.GetPie()
        this.GetSmallCardData()
        this.GetRatesData()
        //this.GetProductMix()

        setInterval(this.GetPie, 5000)
        setInterval(this.GetSmallCardData, 1000)
        setInterval(this.GetRatesData, 5000)
        //setInterval(this.GetProductMix, 5000)
    },
    computed: {
        Targets: function () {
            return {
                trim: [0, ~~(this.SmallCard.trimLossExcess * 0.8), ~~(this.SmallCard.trimLossExcess), ~~(this.SmallCard.trimLossExcess * 1.2)],
                lugfill: [0, ~~(this.SmallCard.lugFillTarget * 0.6), ~~(this.SmallCard.lugFillTarget * 0.8), ~~(this.SmallCard.lugFillTarget)]
            }
        },
        OnRateTarget: function () {
            if (this.TopLeft && this.CurrentFBM) {
                if (this.CurrentFBM / this.CurrentVolumeTarget >= 1) {
                    return 'rgba(15, 220, 99, 0.8)'
                } else {
                    return 'rgba(255, 113, 67, 0.8)'
                }
            } else {
                return null
            }
        },
        CurrentFBM: function () {
            if (this.FBM) {
                return this.FBM[this.FBM.length - 1]
            } else {
                return null
            }
        },
        CurrentVolumeTarget: function () {
            if (this.TopLeft && this.FBM) {
                return (this.TopLeft.targetVPH / 60) * this.FBM.length
            } else {
                return null
            }
        }
    },
    methods: {
        UpdateTable: function () {
            fetch('home/GetTableData', { method: 'POST', headers: { 'Content-Type': 'application/json' } })
                .then(response => {
                    if (response.ok) {
                        return response.json()
                    } else {
                        throw response
                    }
                })
                .then(data => v.Bins = data)
                .catch(error => {
                    console.error('Get Table Data Failed:', error);
                });
        },
        GetSmallCardData: function () {
            fetch('home/GetSmallCard', { method: 'POST', headers: { 'Content-Type': 'application/json' } })
                .then(response => {
                    if (response.ok) {
                        return response.json()
                    } else {
                        throw response
                    }
                })
                .then(data => {
                    if (!this.CompareData(data)) {
                        this.SmallCard = data

                        UpdateSmallCards(TrimLossChart, data.trimLoss, this.Targets.trim);
                        UpdateSmallCards(LugFillChart, data.lugFill, this.Targets.lugfill);
                    }
                    
                    this.PPH = data.currentPPH
                    this.VPH = data.currentVPH

                    if ([1000, 1002, 1005, 1010].includes(data.alarmID) || data.alarmID < 1000) {
                        this.Alarm = data.alarm
                    }

                    this.Severity.low = data.severity == "0"
                    this.Severity.medium = data.severity == "1"
                    this.Severity.high = data.severity == "2"                    
                })
                .catch(error => {
                    console.error('Get Small Card Data Failed:', error);
                });
        },
        GetRatesData: function () {
            fetch('home/GetTopLeft', { method: 'POST', headers: { 'Content-Type': 'application/json' } })
                .then(response => {
                    if (response.ok) {
                        return response.json()
                    } else {
                        throw response
                    }
                })
                .then(data => {
                    this.TopLeft = data

                    let TimeSegment = data.rates.map(r => r.timeSegment)
                    let vph = data.rates.map(r => parseInt(r.vph))
                    let TargetVPH = data.targetVPH

                    //let b = []
                    //vph.reduce(function (i, j, k) { return b[k] = parseInt(i + j / 60); }, 0);
                    //this.FBM = b

                    TotalChart.data.datasets[0].data = vph
                    TotalChart.data.datasets[1].data = [...Array(vph.length).fill(TargetVPH)]
                    //TotalChart.data.datasets[2].data = b
                    //TotalChart.data.datasets[2].borderColor = this.OnRateTarget
                    TotalChart.data.labels = TimeSegment
                    TotalChart.update();
                })
                .catch(error => {
                    console.error('Get Top Left Data Failed:', error);
                });
        },
        GetProductMix: function () {
            fetch('home/GetProductMix', { method: 'POST', headers: { 'Content-Type': 'application/json' } })
                .then(response => {
                    if (response.ok) {
                        return response.json()
                    } else {
                        throw response
                    }
                })
                .then(data => {
                    this.ProductMix = data

                    let labels = data.map(r => r.label)
                    let percent = data.map(r => (r.percent * 100).toFixed(1))

                    ProductsChart.data.labels = labels
                    ProductsChart.data.datasets[0].data = percent
                    ProductsChart.update()
                })
                .catch(error => {
                    console.error('Get Top Left Data Failed:', error);
                });
        },
        GetPie: function () {
            fetch('home/GetPie', { method: 'POST', headers: { 'Content-Type': 'application/json' } })
                .then(response => {
                    if (response.ok) {
                        return response.json()
                    } else {
                        throw response
                    }
                })
                .then(data => {
                    this.Pie = data

                    let d = data.map(d => d.count)
                    let colours = data.map(c => '#' + c.colour)

                    PieChart.data.datasets[0].data = d
                    PieChart.data.datasets[0].backgroundColor = colours
                    PieChart.update()
                })
                .catch(error => {
                    console.error('Get Top Left Data Failed:', error);
                });
        },
        CompareData(data) {
            if (data && this.SmallCard) {
                return data.trimLoss == this.SmallCard.trimLoss && data.lugFill == this.SmallCard.lugFill;
            } else {
                false;
            }
        }
    }
})

// Setup Variables
let FontSize = Math.floor(window.innerWidth * 0.01)
Chart.defaults.global.defaultFontColor = '#fafafa'
Chart.defaults.global.defaultFontSize = FontSize * 1.2
Chart.Title.prototype.afterFit = function () {
    this.height = this.height + FontSize * 0.7;
};
let SmallColours = {
    green: "#ff7143",
    orange: '#fd9704',
    red: '#0fdc63'
}

// Chart options
var OptionsProducts = {
    responsive: true,
    maintainAspectRatio: false,
    events: [],
    legend: {
        display: false
    },
    scales: {
        yAxes: [{
            ticks: {
                fontSize: FontSize,
                fontColor: 'white',
                defaultFontFamily: "'Roboto', sans-serif"
            },
            gridLines: {
                display: false,
            }
        }],
        xAxes: [{
            ticks: {
                min: 0,
                stepSize: 5,
                fontSize: FontSize,
                fontColor: 'white',
                defaultFontFamily: "'Roboto', sans-serif",
                callback: function (value) {
                    return value + "%"
                }
            },
            gridLines: {
                display: true,
                color: "#1e202f",
                lineWidth: 2,
                drawBorder: false
            }
        }]
    },
    layout: {
        padding: {
            right: FontSize * 2.5
        }
    },
    animation: {
        duration: 1,
        onComplete: function () {
            var chartInstance = this.chart,
                ctx = chartInstance.ctx;
            ctx.font = Chart.helpers.fontString(FontSize, Chart.defaults.global.defaultFontStyle, Chart.defaults.global.defaultFontFamily);
            ctx.fillStyle = 'white';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'top';

            var dataset = this.data.datasets[0];
            var meta = chartInstance.controller.getDatasetMeta(0);
            meta.data.forEach(function (bar, index) {
                var data = dataset.data[index];
                var percent = Math.round(data * 10) / 10 + '%';
                var x = bar._model.x + (percent.length);
                ctx.fillText(percent, x + FontSize, bar._model.y - 10);
            });
        }
    }
};
let TotalOptions = {
    responsive: true,
    maintainAspectRatio: false,
    events: [],
    legend: {
        display: true
    },
    title: {
        text: "Volume/Hour",
        display: false
    },
    layout: {
        padding: {
            right: FontSize * 2.5
        }
    },
    scales: {
        yAxes: [{
            id: 'V',
            type: 'linear',
            position: 'left',
            ticks: {
                fontSize: FontSize,
                fontColor: 'white',
                defaultFontFamily: "'Roboto', sans-serif",
            },
            gridLines: {
                display: true,
                color: "#1e202f",
                lineWidth: 2,
                drawBorder: false
            }
        },
            //{
            //    id: 'T',
            //    type: 'linear',
            //    position: 'right',
            //    ticks: {
            //        display: false,
            //        fontSize: FontSize,
            //        fontColor: 'white',
            //        defaultFontFamily: "'Roboto', sans-serif",
            //    },
            //    gridLines: {
            //        display: true,
            //        color: "#1e202f",
            //        lineWidth: 2,
            //        drawBorder: false
            //    }
            //    }
        ],
        xAxes: [{
            display: false,
            gridLines: {
                display: false
            },
        }],
    },
}
let BinsOptions = {
    responsive: true,
    maintainAspectRatio: false,
    events: [],
    legend: {
        display: false
    },
    scales: {
        yAxes: [{
            ticks: {
                fontSize: FontSize,
                fontColor: 'white',
                defaultFontFamily: "'Roboto', sans-serif"
            },
            gridLines: {
                display: false,
            }
        }],
        xAxes: [{
            ticks: {
                min: 0,
                stepSize: 5,
                fontSize: FontSize,
                fontColor: 'white',
                defaultFontFamily: "'Roboto', sans-serif",
                callback: function (value) {
                    return value + "%"
                }
            },
            gridLines: {
                display: true,
                color: "#1e202f",
                lineWidth: 2,
                drawBorder: false
            }
        }]
    },
}
let BaseSmallCardOptions = {
    responsive: true,
    maintainAspectRatio: false,
    events: [],
    legend: {
        display: false
    },
    tooltips: {
        enabled: false
    },
    scales: {
        yAxes: [{
            display: false,
            gridLines: {
                display: false
            },
        }],
        xAxes: [{
            display: false,
            gridLines: {
                display: false
            },
        }]
    },
    title: {
        display: true,
        fontSize: FontSize * 1.3,
        fontColor: '#fafafa'
    },
    animation: {
        duration: 0,
        animateRotate: false
    },
    hover: {
        animationDuration: 0
    },
    responsiveAnimationDuration: 0,
    showMarkers: true,
    indicatorColor: '#fafafa'
}

let PieOptions = {
    responsive: true,
    maintainAspectRatio: false,
    title: {
        display: true,
        position: 'top',
        padding: 0,
        text: 'Bin Status'
    },
    legend: {
        display: true,
        position: 'left'
    },
    tooltips: {
        enabled: false
    },
    animation: {
        duration: 0,
        animateRotate: false
    },
    hover: {
        animationDuration: 0
    },
}

let TrimLossOptions = JSON.parse(JSON.stringify(BaseSmallCardOptions))
TrimLossOptions.title.text = "TrimLoss %";

let LugFillOptions = JSON.parse(JSON.stringify(BaseSmallCardOptions))
LugFillOptions.title.text = "LugFill %";

// Chart context variables
var ctxProducts = document.getElementById('Grades').getContext("2d");
var ctxTotal = document.getElementById('Total').getContext('2d')

var ctxTrimLoss = document.getElementById('TrimLoss').getContext("2d");
var ctxLugFill = document.getElementById('LugFill').getContext("2d");

var ctxPie = document.getElementById('Pie').getContext("2d");

// Chart data objects
var DataProducts = {
    labels: ['2x4 Economy', '2x6 #1', '2x6 Utility', '2x12', '3x6 #2'],
    datasets: [
        {
            data: [35, 25, 20, 12, 8],
            backgroundColor: '#2676c7',
            borderColor: '#2676c7',
            datalabels: {
                labels: {
                    title: null
                }
            }
        }
    ]
};
var DataTotal = {
    labels: [...Array(100).keys()],
    datasets: [{
        backgroundColor: 'rgba(38, 199, 199, 0.2)',
        borderColor: 'rgba(38, 199, 199, 1)',
        data: [],
        pointRadius: 0,
        yAxisID: 'V',
        label: "Volume/Hour",
        datalabels: {
            labels: {
                title: null
            }
        }
    },
    {
        backgroundColor: 'rgba(153, 102, 255, 0)',
        borderColor: 'rgba(153, 102, 255, 1)',
        data: [],
        pointRadius: 0,
        yAxisID: 'V',
        label: "Volume/Hour Target",
        datalabels: {
            labels: {
                title: null
            }
        }
    },
    ]
};
var DataTrimLoss = {
    labels: [],
    datasets: [{
        backgroundColor: [SmallColours.red, SmallColours.orange, SmallColours.green],
        borderWidth: 0,
        gaugeData: {
            value: 7777,
            valueColor: "#ff7143"
        },
        gaugeLimits: [0, 8, 12, 20],
        datalabels: {
            labels: {
                title: null
            }
        }
    }]
};
var DataLugFill = {
    labels: [],
    datasets: [{
        backgroundColor: [SmallColours.green, SmallColours.orange, SmallColours.red],
        borderWidth: 0,
        gaugeData: {
            value: 7777,
            valueColor: "#ff7143"
        },
        gaugeLimits: [0, 30, 50, 100],
        datalabels: {
            labels: {
                title: null
            }
        }
    }]
};

var DataPie = {
    labels: ["Spare", "Active", "Full", "Disabled", "Reject", "Virtual"],
    datasets: [{
        data: [],
        backgroundColor: [],
        borderColor: []
    }]
}

// Chart variables
var ProductsChart = new Chart(ctxProducts, {
    type: 'horizontalBar',
    data: DataProducts,
    options: OptionsProducts
});
var TotalChart = new Chart(ctxTotal, {
    type: 'line',
    data: DataTotal,
    options: TotalOptions
})
var TrimLossChart = new Chart(ctxTrimLoss, {
    type: 'tsgauge',
    data: DataTrimLoss,
    options: TrimLossOptions
});
var LugFillChart = new Chart(ctxLugFill, {
    type: 'tsgauge',
    data: DataLugFill,
    options: LugFillOptions
});

var PieChart = new Chart(ctxPie, {
    type: 'doughnut',
    data: DataPie,
    options: PieOptions
})

// Update small card chart and counter
function UpdateSmallCards(Chart, Data, Targets) {
    Chart.data.datasets[0].gaugeLimits = Targets
    let c = Chart.data.datasets[0].backgroundColor

    Chart.data.datasets[0].gaugeData.value = Data
    Chart.data.datasets[0].gaugeData.valueColor =
        Data < Targets[1] ? c[0] :
            Data < Targets[2] && Data >= Targets[1] ? c[1] :
                c[2]

    Chart.update();
}