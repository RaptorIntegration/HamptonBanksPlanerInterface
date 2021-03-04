const headers = {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
}
let FirstCTX, FirstChart, SecondCTX, SecondChart
ChangeChartFont()

const v = new Vue({
    name: "Boards",
    el: "#app",
    data() {
        return {
            boards: [],
            colHeaders: [
                { source: "LugNum", text: "Lug" },
                { source: "BinCount", text: "Bay Count" },
                { source: "ProdLabel", text: "Product" },
                { source: "LengthLabel", text: "Length" },
                { source: "ThickActual", text: "Thickness" },
                { source: "WidthActual", text: "Width" },
                { source: "LengthInLabel", text: "Length In" },
                { source: "Saws", text: "Saws" },
                { source: "NETLabel", text: "NET" },
                { source: "FETLabel", text: "FET" },
                { source: "CN2Label", text: "CN2" },
                { source: "FenceLabel", text: "Fence" },
            ],
            prodStats: null,
            BoardsInterval: null,
            ChartsInterval: null,
            ProdInterval: null,
            ShowStats: true,
            ShowRejects: false,
            Rejects: null,
            Colours: [
                "#AA3939",
                "#FFD7D7",
                "#EB8A8A",
                "#690808",
                "#290000",

                "#AA6A39",
                "#FFE9D7",
                "#EBB48A",
                "#693208",
                "#291200",

                "#236863",
                "#B0D0CE",
                "#55908C",
                "#05413C",
                "#001917",

                "#297B48",
                "#BBDDC8",
                "#64AA7E",
                "#064C20",
                "#001E0B",

                "#343477",
                "#AAAAC4",
                "#65659C",
                "#121252",
                "#03032E",]
        };
    },
    mounted() {
        this.GetBoards()
        setTimeout(() => this.GetRejects(), 50)
        setTimeout(() => this.GetProdStats(), 100)
        setTimeout(() => this.InitCharts(), 200)
        setTimeout(() => this.GetCharts(), 200)

        this.SetBoardsInterval()
        setTimeout(() => this.SetProdInterval(), 500)
        this.ChartInterval = setInterval(this.GetCharts, (60 * 1000));
    },
    computed: {
        BoardsIntervalStatus: function () {
            if (this.BoardsInterval) {
                return "Pause"
            } else {
                return "Resume"
            }
        },
        StatsStatus: function () {
            return this.ShowStats ? "Hide Stats" : "Show Stats"
        },
        RejectsStatus: function () {
            return this.ShowRejects ? "Hide Rejects" : "Show Rejects"
        }
    },
    methods: {
        GetBoards: function () {
            fetch('Boards.aspx/GetBoards', { method: 'POST', headers: headers })
                .then(response => response.json())
                .then(data => this.boards = JSON.parse(data.d))
                .catch(error => console.error(error));
        },
        GetCharts: function () {
            fetch('Boards.aspx/GetCharts', { method: 'POST', headers: headers })
                .then(response => response.json())
                .then(data => this.UpdateCharts(JSON.parse(data.d)))
                .catch(error => console.error(error));
        },
        GetProdStats: function () {
            fetch('Boards.aspx/GetProdstats', { method: 'POST', headers: headers })
                .then(response => response.json())
                .then(data => this.prodStats = JSON.parse(data.d))
                .catch(error => console.error(error));
        },
        GetRejects: function () {
            fetch('Boards.aspx/GetRejects', { method: 'POST', headers: headers })
                .then(response => response.json())
                .then(data => this.Rejects = JSON.parse(data.d))
                .catch(error => console.error(error));
        },

        UpdateCharts: function (Json) {
            FirstChart.data.datasets[0].data = Json.PPH;
            FirstChart.data.datasets[1].data = Json.VPH;
            SecondChart.data.datasets[0].data = Json.LF;

            FirstChart.options.annotation.annotations[0].value = Json.TPPH;
            FirstChart.options.annotation.annotations[1].value = Json.TVPH;
            SecondChart.options.annotation.annotations[0].value = Json.TLF;

            FirstChart.options.scales.yAxes[0].suggestedMax = (1 * Json.TPPH);
            FirstChart.options.scales.yAxes[1].suggestedMax = (1 * Json.TVPH);
            SecondChart.options.scales.yAxes[0].suggestedMax = (1.1 * Json.TLF);

            FirstChart.data.labels = [...Array(Json.PPH.length).keys()];
            SecondChart.data.labels = [...Array(Json.PPH.length).keys()];

            FirstChart.update();
            SecondChart.update();
        },
        InitCharts: function () {
            FirstCTX = document.getElementById('FirstChart').getContext('2d');
            FirstChart = new Chart(FirstCTX, {
                type: 'line',
                data: {
                    labels: [],
                    datasets: [{
                        label: 'Pieces/HR',
                        backgroundColor: 'rgba(54, 162, 235, 0.2)',
                        borderColor: 'rgba(54, 162, 235, 1)',
                        data: [],
                        yAxisID: '1',
                    },
                    {
                        label: 'Volume/HR',
                        backgroundColor: 'rgba(75, 192, 192, 0.2)',
                        borderColor: 'rgba(75, 192, 192, 1)',
                        data: [],
                        yAxisID: '2',
                    }]
                },
                options: {
                    maintainAspectRatio: false,
                    scales: {
                        xAxes: [{
                            autoSkip: true,
                            maxTicksLimit: 5,
                        }],
                        yAxes: [
                            {
                                id: '1',
                                ticks: {
                                    beginAtZero: true,
                                },
                                position: 'left',
                                type: 'linear',
                            },
                            {
                                id: '2',
                                ticks: {
                                    beginAtZero: true,
                                },
                                position: 'right',
                                gridLines: { display: false },
                                stacked: false,
                                type: 'linear',
                            }
                        ]
                    },
                    annotation: {
                        annotations: [
                            {
                                type: 'line',
                                mode: 'horizontal',
                                scaleID: '1',
                                value: 5,
                                borderColor: 'rgba(27, 54, 101, 0.4)',
                                borderWidth: 2,
                                label: {
                                    enabled: true,
                                    backgroundColor: 'rgba(27, 54, 101, 0.4)',
                                    content: 'Target PPH',
                                    yPadding: 0,
                                    position: 'center'
                                }
                            },
                            {
                                type: 'line',
                                mode: 'horizontal',
                                scaleID: '2',
                                value: 5,
                                borderColor: 'rgba(27, 54, 101, 0.4)',
                                borderWidth: 2,
                                label: {
                                    enabled: true,
                                    backgroundColor: 'rgba(27, 54, 101, 0.4)',
                                    content: 'Target VPH',
                                    yPadding: 0,
                                    position: 'right'
                                }
                            },
                        ]
                    },
                    animation: {
                        duration: 0
                    },
                    tooltips: { enabled: false },
                    hover: {
                        animationDuration: 0,
                        mode: null
                    },
                    responsiveAnimationDuration: 0,
                    elements: {
                        line: {
                            tension: 0,
                        },
                        point: {
                            radius: 0
                        }
                    }
                }
            });
            SecondCTX = document.getElementById('SecondChart').getContext('2d');
            SecondChart = new Chart(SecondCTX, {
                type: 'line',
                data: {
                    labels: [],
                    datasets: [{
                        label: 'LugFill',
                        backgroundColor: 'rgba(255,193,7, 0.2)',
                        borderColor: 'rgba(255,193,7, 1)',
                        data: []
                    }]
                },
                options: {
                    maintainAspectRatio: false,
                    scales: {
                        xAxes: [{
                            autoSkip: true,
                            maxTicksLimit: 5,
                        }],
                        yAxes: [{
                            ticks: {
                                beginAtZero: true
                            }
                        }]
                    },
                    annotation: {
                        annotations: [{
                            type: 'line',
                            mode: 'horizontal',
                            scaleID: 'y-axis-0',
                            value: 5,
                            borderColor: 'rgba(27, 54, 101, 0.4)',
                            borderWidth: 2,
                            label: {
                                enabled: true,
                                backgroundColor: 'rgba(27, 54, 101, 0.4)',
                                content: 'Target',
                                yPadding: 0,
                                position: 'center'
                            }
                        }]
                    },
                    animation: {
                        duration: 0
                    },
                    tooltips: { enabled: false },
                    hover: {
                        animationDuration: 0,
                        mode: null
                    },
                    responsiveAnimationDuration: 0,
                    elements: {
                        line: {
                            tension: 0,
                        },
                        point: {
                            radius: 0
                        }
                    }
                }
            });
        },

        ToggleBoardsInterval: function () {
            this.BoardsInterval ? this.ClearBoardsInterval() : this.SetBoardsInterval()
        },
        ToggleStats: function () {
            this.ShowStats = !this.ShowStats

            this.ShowStats ? this.SetProdInterval() : this.ClearProdInterval()
        },
        ToggleRejects: function () {
            this.GetRejects();
            this.ShowRejects = !this.ShowRejects
        },

        SaveRejects: function (reject) {
            let data = JSON.stringify({ 'rejects': reject })
            fetch('Boards.aspx/SaveRejects', { method: 'POST', headers: headers, body: data })
                .then(response => response.json())
                .then(data => console.log(data))
                .catch(error => console.error(error));
        },

        SetBoardsInterval() {
            this.BoardsInterval = setInterval(this.GetBoards, (1000));
        },
        SetProdInterval() {
            this.ProdInterval = setInterval(this.GetProdStats, (2 * 1000));
        },

        ClearBoardsInterval() {
            clearInterval(this.BoardsInterval)
            this.BoardsInterval = null
        },
        ClearProdInterval() {
            clearInterval(this.ProdInterval)
            this.ProdInterval = null
        },
    }
});