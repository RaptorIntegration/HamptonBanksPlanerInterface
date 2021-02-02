function UpdateChart(chart, data) {
    for (var i = 0; i < data.datasets[0].backgroundColor.length; i++) {
        data.datasets[0].backgroundColor[i] = v.StatusColours[data.datasets[0].backgroundColor[i]].colour;
        data.datasets[0].borderColor[i] = v.StatusColours[data.datasets[0].borderColor[i]].colour;

        if (chart.data.datasets.length > 0 && chart.data.datasets[0].data[i] != data.datasets[0].data[i]) {
            chart.data.datasets[0].data[i] = data.datasets[0].data[i];
        }
        if (chart.data.datasets.length > 0 && chart.data.datasets[0].backgroundColor[i] != data.datasets[0].backgroundColor[i]) {
            chart.data.datasets[0].backgroundColor[i] = data.datasets[0].backgroundColor[i];
            chart.data.datasets[0].borderColor[i] = data.datasets[0].borderColor[i];
        }
    }
    if (chart.data.datasets.length == 0) {
        chart.data = data;
    }
    chart.update();
}
function GetChartData() {
    axios.post("WebSort.aspx/GetChartData", { headers: { 'Content-Type': 'application/json' } })
        .then(function (response) {
            if (response.data.d === '') { return; }
            var Parsed = JSON.parse(response.data.d);
            UpdateChart(BinChart, Parsed);
        })
        .catch(function (response) {
            console.dir(response);
        });
}
function GetPieData() {
    axios.post("WebSort.aspx/GetPieData", { headers: { 'Content-Type': 'application/json' } })
        .then(function (response) {
            if (response.data.d === '') { return; }
            var Parsed = JSON.parse(response.data.d);
            for (let i = 0; i < Parsed.datasets[0].backgroundColor.length; i++) {
                Parsed.datasets[0].backgroundColor[i] = v.StatusColours[Parsed.datasets[0].backgroundColor[i]].colour
            }
            PieChart.data = Parsed
            PieChart.data.datasets[0].borderWidth = 0
            PieChart.data.datasets[0].borderColor = '#f0f0f1'
            PieChart.update();
        })
        .catch(function (response) {
            console.dir(response);
        });
}
function GetColours() {
    axios.post("WebSort.aspx/GetColours", { headers: { 'Content-Type': 'application/json' } })
        .then(function (response) {
            var Parsed = JSON.parse(response.data.d);
            for (var i = 0; i < Parsed.length; i++) {
                Colours[i] = '#' + Parsed[i];
            }
            DistributeColours();
            GetChartData();
            GetPieData();
        })
        .catch(function (response) {
            console.dir(response);
        });
}
function ToggleVis(Elem) {
    if (Elem.style.display === '' || Elem.style.display === 'block' || Elem.style.display === 'table') {
        Elem.style.display = 'none';
    } else {
        if (Elem.tagName === 'TABLE') {
            Elem.style.display = 'table';
        } else {
            Elem.style.display = 'block';
        }
    }
}
function DistributeColours(colours) {
    document.getElementById('SpareLabel').childNodes[2].style.backgroundColor = colours[0].colour;
    document.getElementById('ActiveLabel').childNodes[2].style.backgroundColor = colours[1].colour;
    document.getElementById('FullLabel').childNodes[2].style.backgroundColor = colours[2].colour;
    document.getElementById('DisabledLabel').childNodes[2].style.backgroundColor = colours[3].colour;
    document.getElementById('RejectLabel').childNodes[2].style.backgroundColor = colours[4].colour;
    document.getElementById('VirtualLabel').childNodes[2].style.backgroundColor = colours[5].colour;

    GetChartData()
    GetPieData();
}
function rgb2hex(rgb) {
    rgb = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
    function hex(x) {
        return ("0" + parseInt(x).toString(16)).slice(-2);
    }
    return "#" + hex(rgb[1]) + hex(rgb[2]) + hex(rgb[3]);
}
function openView(evt, TabName) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(TabName).style.display = "block";
    evt.currentTarget.className += " active";
    if (TabName === 'Edit') {
        GetChartData()
        GetPieData()
        BinInterval = setInterval(GetChartData, 5 * 1000);
        PieInterval = setInterval(GetPieData, 5 * 1000 + 500);
    } else {
        clearInterval(BinInterval);
        clearInterval(PieInterval);
        BinInterval = null;
        PieInterval = null;
    }
    localStorage.setItem('tab', TabName);
}

Vue.component('auto-complete', {
    template: `
<div>
    <input placeholder="Filter..." class="form-control filter" type="text" v-bind:value="value" v-on:input="updateValue($event.target.value)" @keydown.tab.prevent="complete()" @focus="focus(true)" @blur="focus(false)" @click="Clear" spellcheck="false">
   	<div style="max-height: 1px;">
        <div v-if="focused && SelectionList" class="scroll1 selection-container">
            <div class="selection-list" v-for="Dim in SelectionList" @mousedown="complete(Dim)">{{ Dim }}</div>
        </div>
    </div>
</div>
  `,
    name: 'AutoComplete',
    props: {
        value: { type: String, required: false },
        data: { type: Array, required: true },
    },
    computed: {
        SelectionList: function () {
            let L = this.data;
            if (this.value && L) {
                return L.filter(e => e.includes(this.value));
            }
            else {
                return L;
            }
        }
    },
    methods: {
        complete(row) {
            this.select(row);
        },
        select(row) {
            this.selected = true;
            this.$emit('input', row);
        },
        focus(f) {
            this.focused = f
        },
        updateValue(value) {
            this.$emit('input', value);
        },
        Clear() {
            this.value = ''
            this.$emit('input', this.value);
        }
    },

    data() {
        return {
            focused: false
        }
    },

    created() {
        const self = this;
        self.value = self.value || '';
    }
});

Vue.component('sorter', {
    name: 'sorter-view',
    props: {
        value: { type: Object, required: true },
        colours: { type: Array, required: true },
        header: { type: Object, required: true },
        incdec: { type: Number, required: true }
    },
    data() {
        return {
            Editing: false,
            Previous: null
        }
    },
    watch: {
        'value.BinStatus': function () {
            this.Update('BinStatus')
        }
    },
    computed: {
        RadioIDs: function () {
            if (this.value) {
                return [
                    'radio-1-' + this.value.BinID,
                    'radio-2-' + this.value.BinID,
                    'radio-3-' + this.value.BinID,
                    'radio-4-' + this.value.BinID,
                    'radio-5-' + this.value.BinID
                ]
            } else {
                return null
            }
        },
        Background() { return this.colours[this.value.BinStatus].colour },
        Bar() {
            return {
                'width': this.value.BinPercent + '%',
                'background-color': this.Background
            }
        }
    },
    methods: {
        ToggleEdit: function () {
            this.Editing = !this.Editing
            this.Editing ? this.$emit('editing') : this.$emit('resume')
        },
        Increase: function (val) {
            this.value[val] += this.incdec
            this.Update(val)
        },
        Decrease: function (val) {
            if (this.value[val] - this.incdec < 0) {
                return;
            }
            this.value[val] -= this.incdec
            this.Update(val)
        },
        Update: function (val) {
            this.value.Changed = true;
            if (this.value.Changed && this.value.EditsList.some(e => e.EditedCol === val)) {
                this.value.EditsList.find(e => e.EditedCol === val).EditedVal = this.value[val];
            } else {
                this.value.EditsList.push({
                    'Key': this.value.SortID,
                    'EditedCol': val,
                    'EditedVal': this.value[val],
                    'Previous': this.Previous[val]
                });
            }
        },
        Save: function () {
            if (this.value.EditsList.length < 1) {
                this.Editing = false
                this.$emit('resume')
                return;
            } else {
                let data = JSON.stringify({ Changed: [this.value] });

                axios.post("WebSort.aspx/Save", data, this.header)
                    .then(response => {
                        let Parsed = JSON.parse(response.data.d);
                        console.dir(response);

                        this.$emit('resume')
                        this.Editing = false

                        this.SaveResponse = Parsed;
                        setTimeout(function () {
                            v.SaveResponse.Message = null;
                        }, 5000);

                        GetChartData();
                        GetPieData();
                    })
                    .catch(error => {
                        console.error(error);
                        v.SaveResponse.Message = error;
                    });
            }
        }
    },

    created() {
        this.Previous = JSON.parse(JSON.stringify(this.value))
    }
});

const v = new Vue({
    el: '#vue-wrapper',
    name: 'SortsTable',
    data: {
        Bays: [],
        Columns: [
            { DataSource: 'BinSize', Header: 'Size' },
            { DataSource: 'BinCount', Header: 'Count' },
        ],
        ProductGrades: [],

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
            "#03032E",
        ],
        StatusColours: [
            { status: 'Spare', colour: '#00796B' },
            { status: 'Active', colour: '#00796B' },
            { status: 'Full', colour: '#00796B' },
            { status: 'Disabled', colour: '#00796B' },
            { status: 'Reject', colour: '#00796B' },
            { status: 'Virtual', colour: '#00796B' },
        ],
        ShowColours: false,

        StatusList: [
            'Spare', 'Active', 'Full', 'Disabled', 'Reject', 'Virtual'
        ],

        Previous: null,
        Filter: null,
        SortBy: 'Sort By...',
        SortByAsc: true,
        Edited: false,
        Editing: false,
        SaveResponse: {
            Message: null,
            ChangeList: [{
                Key: null,
                EditedCol: null,
                EditedVal: null
            }]
        },

        IncDec: 1,

        Timer: null,

        Loading: false,

        Headers: {
            headers: {
                'Content-Type': 'application/json; charset=UTF-8',
                'Accept': 'application/json'
            }
        }
    },
    mounted: function () {
        this.SetAutoUpdate();
        this.GetColours();
        this.GetIncDec();
        this.GetProductGrades();
    },
    computed: {
        FilterBays: function () {
            let v = this
            let SortDir = v.SortByAsc ? 'asc' : 'desc';

            if (v.SortBy && !v.Edited) {
                v.Bays.sort(v.CompareValues(v.SortBy, SortDir));
            }
            if (v.Filter) {
                return v.Bays.filter(s => {
                    return (s.BinLabel.toUpperCase().includes(v.Filter.toUpperCase())
                        | s.BinStatusLabel.toUpperCase().includes(v.Filter.toUpperCase()))
                });
            } else {
                return v.Bays
            }
        },
        Sorter: function() {
            if(this.Bays.length){
                return this.Bays.slice(0, 38)
            } else {
                return null
            }
        },
        FilteredSorter: function() {
            if (!this.Sorter) {return null}
            let v = this
            let SortDir = v.SortByAsc ? 'asc' : 'desc';

            if (v.SortBy && !v.Edited) {
                v.Sorter.sort(v.CompareValues(v.SortBy, SortDir));
            }
            if (v.Filter) {
                return v.Sorter.filter(s => {
                    return (s.BinLabel.toUpperCase().includes(v.Filter.toUpperCase())
                        | s.BinStatusLabel.toUpperCase().includes(v.Filter.toUpperCase()))
                });
            } else {
                return v.Sorter
            }
        },
        ChangedList: function () {
            return this.Bays.filter(s => {
                return s.Changed
            });
        },
        DropDown: function () {
            let v = this
            if (v.Bays.length > 0) {
                let unique = [...new Set(
                    v.Bays.filter(
                        f => f.BinLabel != ''
                    ).map(
                        item => item.BinLabel.split(" ")[0].toLowerCase()
                    )
                )];
                if (unique) {
                    unique.push('spare');
                    unique.push('active')
                    unique.push('full')
                    unique.push('disabled')
                    unique.push('reject')
                    unique.push('virtual')
                }
                return unique.sort()
            }
            return null
        },
        ShowColoursValue: function () {
            return this.ShowColours ? 'Hide Colours' : 'Show Colours'
        },
        StatusListShort: function () {
            return this.StatusList.slice(0, 4);
        }
    },
    methods: {
        Update: function (EditedCol, EditedVal, ChangedRow) {
            let v = this;

            ChangedRow.Changed = true;
            if (ChangedRow.Changed && ChangedRow.EditsList.some(e => e.EditedCol === EditedCol)) {
                ChangedRow.EditsList.find(e => e.EditedCol === EditedCol).EditedVal = EditedVal;
            } else {
                ChangedRow.EditsList.push({
                    'Key': ChangedRow.SortID,
                    'EditedCol': EditedCol,
                    'EditedVal': EditedVal,
                    'Previous': v.Previous
                });
            }

            v.Edited = true;
            v.Editing = false;
        },
        Prev: function (Row, Col) {
            this.Previous = Row[Col];
        },
        Save: function () {
            let v = this;
            v.Loading = true;

            let data = JSON.stringify({ Changed: v.ChangedList });

            axios.post("WebSort.aspx/Save", data, v.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);
                    console.dir(response);
                    v.SetAutoUpdate();
                    v.SaveResponse = Parsed;
                    v.Loading = false;
                    setTimeout(function () {
                        v.SaveResponse.Message = null;
                    }, 5000);
                    v.Edited = false;
                    v.Editing = false;
                    GetChartData();
                    GetPieData();
                })
                .catch(error => {
                    console.dir(error);
                    v.SaveResponse.Message = error;
                });
            v.Loading = false;
        },
        Cancel: function () {
            this.SetAutoUpdate();
            this.Loading = false;
            this.Edited = false;
            this.Editing = false;
        },
        EditingCell: function (Row, Col) {
            if (this.Editing && this.Editing.includes("ProductsLabel") && this.Editing === Row.BinID + '_' + Col) {
                this.Editing = null
                return;
            }
            this.Editing = (Row.BinID + '_' + Col);
            this.CancelAutoUpdate();
            if (Col == 'ProductsLabel') {
                this.GetProductList(Row);
            }
        },
        ProductsChange: function (Product, Row) {
            let v = this
            Row.Changed = true;

            if (Row.EditsList.length > 0 && Row.EditsList.some(e => e.EditedVal.includes(Product.Label))) {
                Row.EditsList.find(e => e.EditedVal.includes(Product.Label)).EditedVal = Product.Selected ? Product.Label : 'De-Selected ' + Product.Label;
            } else {
                Row.EditsList.push({
                    'EditedCol': 'Products',
                    'EditedVal': Product.Selected ? Product.Label : 'De-Selected ' + Product.Label
                });
            }
            v.Edited = true;
        },
        CompareValues: function (key, order = 'asc') {
            return function innerSort(a, b) {
                if (!a.hasOwnProperty(key) || !b.hasOwnProperty(key)) {
                    return 0;
                }

                const varA = (typeof a[key] === 'string')
                    ? a[key].toUpperCase() : a[key];
                const varB = (typeof b[key] === 'string')
                    ? b[key].toUpperCase() : b[key];

                let comparison = 0;
                if (varA > varB) {
                    comparison = 1;
                } else if (varA < varB) {
                    comparison = -1;
                }
                return (
                    (order === 'desc') ? (comparison * -1) : comparison
                );
            };
        },
        Sort: function (Col) {
            let v = this
            if (v.SortBy === Col) {
                v.SortByAsc = !v.SortByAsc
            } else {
                v.SortByAsc = true
            }
            v.SortBy = Col;
        },

        GetData: function () {
            let v = this;
            axios.post('WebSort.aspx/GetData', v.Headers)
                .then(response => {
                    v.$set(v, 'Bays', JSON.parse(response.data.d))
                })
                .catch(error => {
                    console.dir(error);
                });
        },
        GetProductList: function (Row) {
            let v = this;
            let data = { BinID: Row.BinID.toString() };

            axios.post("WebSort.aspx/GetProductList", data, v.Headers)
                .then(response => {
                    let ParsedJson = JSON.parse(response.data.d);
                    v.$set(Row, "ProdLen", ParsedJson);
                })
                .catch(error => {
                    console.dir(error);
                })
        },
        GetProductGrades: function () {
            axios.post('WebSort.aspx/GetProductGrades', this.Headers)
                .then(response => {
                    this.ProductGrades = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },

        CancelAutoUpdate: function () {
            clearInterval(this.Timer);
        },
        SetAutoUpdate: function () {
            this.CancelAutoUpdate();
            this.GetData();
            this.Timer = setInterval(this.GetData, 1000 * 2);
        },

        GetIncDec: function () {
            axios.post('WebSort.aspx/GetIncDec', this.Headers)
                .then(response => {
                    this.IncDec = parseInt(response.data.d)
                })
                .catch(error => {
                    console.dir(error);
                });
        },
        IncreaseInc: function () {
            this.IncDec++
            this.SaveIncDec()
        },
        DecreaseInc: function () {
            if (this.IncDec <= 1) {
                this.IncDec = 1
                return
            }
            this.IncDec--
            this.SaveIncDec()
        },
        SaveIncDec: function () {
            axios.post("WebSort.aspx/SaveIncDec", JSON.stringify({ IncDec: this.IncDec }), this.Headers)
                .then(response => {
                })
                .catch(error => {
                    console.dir(error);
                });
        },

        GetColours: function () {
            axios.post("WebSort.aspx/GetColours", this.Headers)
                .then(res => {
                    var Parsed = JSON.parse(res.data.d);
                    for (var i = 0; i < Parsed.length; i++) {
                        this.StatusColours[i].colour = '#' + Parsed[i];
                    }
                    DistributeColours(this.StatusColours);
                })
                .catch(function (response) {
                    console.error(response);
                });
        },
        ToggleColours: function () {
            this.ShowColours = !this.ShowColours
        },
        SaveColours: function () {
            let data = JSON.stringify({ C: this.StatusColours.map(c => c.colour) })
            axios.post("WebSort.aspx/SaveColours", data, this.Headers)
                .then(res => {
                    DistributeColours(this.StatusColours);
                })
                .catch(function (response) {
                    console.dir(response);
                });
        }
    },
    beforeDestroy() {
        clearInterval(this.timer)
    }
});

var BinInterval;
var PieInterval;
var LineInterval;

var CTX;
var BinChart;

var CTXPie;
var PieChart;

ChangeChartFont()

function InitializeCharts() {
    CTX = document.getElementById('BinChart').getContext('2d');
    BinChart = new Chart(CTX, {
        type: 'bar',
        data: {},
        options: {
            maintainAspectRatio: false,
            responsive: true,
            scales: {
                xAxes: [{
                    gridLines: {
                        display: false
                    }
                }],
                yAxes: [{
                    ticks: {
                        min: 0,
                        max: 100,
                        callback: function (value) {
                            return value + "%"
                        }
                    }
                }]
            },
            title: {
                display: false
            },
            legend: {
                display: false
            },
        }
    });

    CTXPie = document.getElementById('PieChart').getContext('2d');
    PieChart = new Chart(CTXPie, {
        type: 'pie',
        data: {},
        options: {
            maintainAspectRatio: false,
            responsive: true,
            legend: {
                display: false
            },
            animation: {
                duration: 1,
                easing: "easeOutQuart",
                onComplete: function () {
                    var ctx = this.chart.ctx;
                    ctx.font = "14px Arial";
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'bottom';

                    this.data.datasets.forEach(function (dataset) {
                        for (var i = 0; i < dataset.data.length; i++) {
                            var model = dataset._meta[Object.keys(dataset._meta)[0]].data[i]._model,
                                total = dataset._meta[Object.keys(dataset._meta)[0]].total,
                                mid_radius = model.innerRadius + (model.outerRadius - model.innerRadius) / 2,
                                start_angle = model.startAngle,
                                end_angle = model.endAngle,
                                mid_angle = start_angle + (end_angle - start_angle) / 2;

                            var x = mid_radius * Math.cos(mid_angle);
                            var y = mid_radius * Math.sin(mid_angle);

                            ctx.fillStyle = '#fff';
                            if (i == 3) { // Darker text color for lighter background
                                ctx.fillStyle = '#444';
                            }
                            var percent = String(Math.round(dataset.data[i] / total * 100)) + "%";
                            // Display percent in another line, line break doesn't work for fillText
                            ctx.fillText(percent, model.x + x, model.y + y);
                        }
                    });
                }
            }
        }
    });

    BinInterval = setInterval(GetChartData, (5 * 1000));
    PieInterval = setInterval(GetPieData, (5 * 1000) + 500);
}

window.addEventListener('keydown', function (e) { if (e.keyIdentifier == 'U+000A' || e.keyIdentifier == 'Enter' || e.keyCode == 13) { if (e.target.nodeName == 'INPUT' && e.target.type == 'text') { e.preventDefault(); return false; } } }, true);

const CurrentTab = localStorage.getItem('tab');
if (CurrentTab) {
    if (CurrentTab == 'Edit') {
        document.getElementById('edit-btn').click();
    } else {
        document.getElementById('sort-btn').click();
    }
}
InitializeCharts()