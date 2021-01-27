const v = new Vue({
    el: '#vue',
    name: 'Products',
    data: {
        Tab: 'Products',
        Toast: {
            Message: '',
            Success: false,
            Timeout: null
        },

        Products: {
            EndCols: ["ThicknessID", "WidthID"],
            List: [],
            SortByAsc: false,
            SortBy: '',
            Filter: '',
            Focused: false,
            Previous: null,
            Editing: null,
            Edited: false,
            New: null,
            Thicks: [{ ID: 0, Minimum: 0, Maximum: 0, Nominal: 0 }],
            Widths: [{ ID: 0, Minimum: 0, Maximum: 0, Nominal: 0 }],
            SaveResponse: {
                Message: '',
                ChangedList: []
            },
        },

        Grades: {
            List: [],
            SortByAsc: false,
            SortBy: '',
            Editing: null,
            Edited: false,
            New: null,
            SaveResponse: {
                Message: '',
                ChangedList: []
            },
        },

        Thicks: {
            List: [],
            Cols: ['Nominal', 'Minimum', 'Maximum'],
            SortByAsc: false,
            SortBy: '',
            Editing: null,
            Edited: false,
            New: null,
            SaveResponse: {
                Message: '',
                ChangedList: []
            },
        },

        Widths: {
            List: [],
            Cols: ['Nominal', 'Minimum', 'Maximum'],
            SortByAsc: false,
            SortBy: '',
            Editing: null,
            Edited: false,
            New: null,
            SaveResponse: {
                Message: '',
                ChangedList: []
            },
        },
        Lengths: {
            List: [],
            Cols: ['LengthNominal', 'LengthMin', 'LengthMax'],
            SortByAsc: false,
            SortBy: '',
            Editing: null,
            Edited: false,
            New: null,
            SaveResponse: {
                Message: '',
                ChangedList: []
            },
        },

        PETLengths: {
            List: [],
            Cols: ['SawIndex', 'LengthNominal', 'PETPosition'],
            SortByAsc: false,
            SortBy: '',
            Editing: null,
            Edited: false,
            New: null,
            SaveResponse: {
                Message: '',
                ChangedList: []
            },
        },

        Graders: {
            List: [],
            Cols: ['GraderID', 'GraderDescription'],
            SortByAsc: false,
            SortBy: '',
            Editing: null,
            Edited: false,
            New: null,
            SaveResponse: {
                Message: '',
                ChangedList: []
            },
        },

        NearSawOffSet: 0,

        Security: null,

        Headers: {
            headers: {
                'Content-Type': 'application/json; charset=UTF-8',
                'Accept': 'application/json'
            }
        }
    },
    mounted: function () {
        setTimeout(_ => this.GetSecurity());
        this.Refresh();
    },
    watch: {
        Tab: function () {
            this.RefreshTab()
        }
    },
    computed: {
        FilteredProducts: function () {
            let SortDir = this.Products.SortByAsc ? 'asc' : 'desc';

            if (this.Products.SortBy) {
                [...this.Products.List].sort(this.CompareValues(this.Products.SortBy, SortDir));
            }
            if (this.Products.Filter) {
                return v.Products.List.filter(p => {
                    return (
                        p.ProdLabel.toUpperCase().includes(this.Products.Filter.toUpperCase()) ||
                        p.ProdID.toString().includes(this.Products.Filter) ||
                        this.Grades.List[p.GradeID].GradeLabel.toLowerCase().includes(this.Products.Filter.toLowerCase())
                    )
                });
            } else {
                return v.Products.List
            }
        },
        ProductsList: function () {
            return [...new Set(
                this.Products.List.filter(f => f.ProdLabel != '')
                    .map(item => item.ProdLabel.toLowerCase())
            )];
        },

        ThickSortedByNom: function () {
            if (this.Thicks.List.length) {
                return [...this.Thicks.List].sort((a, b) => a.Nominal - b.Nominal)
            } else {
                return null
            }
        },
        WidthSortedByNom: function () {
            if (this.Widths.List.length) {
                return [...this.Widths.List].sort((a, b) => a.Nominal - b.Nominal)
            } else {
                return null
            }
        },

        GradesSorted: function () {
            if (this.Grades.List.length) {
                let SortDir = this.Grades.SortByAsc ? 'asc' : 'desc';

                if (this.Grades.SortBy) {
                    return [...this.Grades.List].sort(this.CompareValues(this.Grades.SortBy, SortDir));
                } else {
                    return this.Grades.List
                }
            } else {
                return null
            }
        },
        ThicksSorted: function () {
            if (this.Thicks.List.length) {
                let SortDir = this.Thicks.SortByAsc ? 'asc' : 'desc';

                if (this.Thicks.SortBy) {
                    return [...this.Thicks.List].sort(this.CompareValues(this.Thicks.SortBy, SortDir));
                } else {
                    return this.Thicks.List
                }
            } else {
                return null
            }
        },
        WidthsSorted: function () {
            if (this.Widths.List.length) {
                let SortDir = this.Widths.SortByAsc ? 'asc' : 'desc';

                if (this.Widths.SortBy) {
                    return [...this.Widths.List].sort(this.CompareValues(this.Widths.SortBy, SortDir));
                } else {
                    return this.Widths.List
                }
            } else {
                return null
            }
        },
        LengthsSorted: function () {
            if (this.Lengths.List.length) {
                let SortDir = this.Lengths.SortByAsc ? 'asc' : 'desc';

                if (this.Lengths.SortBy) {
                    return [...this.Lengths.List].sort(this.CompareValues(this.Lengths.SortBy, SortDir));
                } else {
                    return this.Lengths.List
                }
            } else {
                return null
            }
        },
        PETLengthsSorted: function () {
            if (this.PETLengths.List.length) {
                let SortDir = this.PETLengths.SortByAsc ? 'asc' : 'desc';

                if (this.PETLengths.SortBy) {
                    return [...this.PETLengths.List].sort(this.CompareValues(this.PETLengths.SortBy, SortDir));
                } else {
                    return this.PETLengths.List
                }
            } else {
                return null
            }
        },
        GradersSorted: function () {
            if (this.Graders.List.length) {
                let SortDir = this.Graders.SortByAsc ? 'asc' : 'desc';

                if (this.Graders.SortBy) {
                    return [...this.Graders.List].sort(this.CompareValues(this.Graders.SortBy, SortDir));
                } else {
                    return this.Graders.List
                }
            } else {
                return null
            }
        },

        SecurityEnabled: function () {
            return this.Security != 1
        },

        ToastClass: function () {
            return {
                'toast-success': this.Toast.Success,
                'toast-error': !this.Toast.Success
            }
        }
    },
    methods: {
        GetProducts: function () {
            axios.post('Products.aspx/GetProducts', this.Headers)
                .then(response => {
                    this.Products.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetGrades: function () {
            axios.post('Products.aspx/GetGrades', this.Headers)
                .then(response => {
                    this.Grades.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetThicknesses: function () {
            axios.post('Products.aspx/GetThicknesses', this.Headers)
                .then(response => {
                    this.Thicks.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetWidths: function () {
            axios.post('Products.aspx/GetWidths', this.Headers)
                .then(response => {
                    this.Widths.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetLengths: function () {
            axios.post('Products.aspx/GetLengths', this.Headers)
                .then(response => {
                    this.Lengths.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetPETLengths: function () {
            axios.post('Products.aspx/GetPETLengths', this.Headers)
                .then(response => {
                    this.GetNearSaw();
                    this.PETLengths.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetGraders: function () {
            axios.post('Products.aspx/GetGraders', this.Headers)
                .then(response => {
                    this.Graders.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetSecurity: function () {
            axios.post('Products.aspx/GetSecurity', this.Headers)
                .then(response => {
                    this.Security = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetNearSaw: function () {
            axios.post('Products.aspx/GetNearSaw', this.Headers)
                .then(response => {
                    this.NearSawOffSet = JSON.parse(response.data.d).NearSaw;
                })
                .catch(error => {
                    console.error(error);
                });
        },

        EditingCell: function (table, col, key) {
            if (this.SecurityEnabled) { return; }
            table.Editing = `${key}_${col}`;
        },
        Update: function (col, val, row, table, key) {
            table.Edited = true
            let edited = row.EditsList.some(e => e.EditedCol === col)

            if (edited) {
                edited.EditedVal = val;
            } else {
                row.EditsList.push({
                    'Key': row[key],
                    'EditedCol': col,
                    'EditedVal': val,
                    'Previous': table.Previous
                });
            }

            if ((col === 'ThicknessID' || col === 'WidthID') && key === 'ProdID') {
                this.FillInThickWidth(row);
            }
        },
        Prev: function (val, table) {
            table.Previous = val;
        },
        CancelNew(table) {
            table.New = null
        },
        Save: function (table, label) {
            let changed = table.List.filter(p => p.EditsList.length)
            if (!changed) { return; }
            let upper = label.charAt(0).toUpperCase() + label.slice(1)

            let d = {}
            d[label] = changed
            let data = JSON.stringify(d)

            axios.post("Products.aspx/Save" + upper, data, this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);
                    console.dir(response);
                    table.SaveResponse = Parsed;
                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.error(error);
                    this.Toast(JSON.parse(error.data.d));
                })
                .finally(() => {
                    this.RefreshTab()

                    this.Loading = false;
                    table.Edited = false;
                    table.Editing = false;
                });
        },
        AddNew: function (table, label) {
            let d = {}
            d[label] = table.New
            let data = JSON.stringify(d)
            let upper = label.charAt(0).toUpperCase() + label.slice(1)

            axios.post("Products.aspx/AddNew" + upper, data, this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);
                    console.dir(Parsed);

                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.dir(error);
                    this.Toast(JSON.parse(error.data.d));
                })
                .finally(_ => {
                    table.New = null
                    this.RefreshTab()
                });
        },

        DeleteProduct: function (product) {
            let message = `Are you sure you would like to delete ${product.ProdLabel}`
            if (!confirm(message)) { return; }

            let data = JSON.stringify({ 'product': product })
            axios.post("Products.aspx/DeleteProduct", data, this.Headers)
                .then(response => {
                    this.GetProducts();

                    let Parsed = JSON.parse(response.data.d);
                    console.dir(Parsed);

                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.dir(error);
                    this.Toast(JSON.parse(error.data.d));
                });
        },
        ShowNewProduct: function () {
            this.Products.New = {
                "Deleted": 0,
                "Active": true,
                "ProdLabel": "",
                "GradeID": 0,
                "MoistureID": 0,
                "SpecID": 0,
                "ThickNominal": 0,
                "ThickMin": 0,
                "ThickMax": 0,
                "WidthNominal": 0,
                "WidthMin": 0,
                "WidthMax": 0,
                "ThicknessID": 0,
                "WidthID": 0
            }
        },
        AddNewProduct: function () {
            let data = JSON.stringify({ 'product': this.Products.New })

            axios.post("Products.aspx/AddNewProduct", data, this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);
                    console.dir(Parsed);

                    this.SetToast(Parsed);
                    this.GetProducts();
                })
                .catch(error => {
                    console.dir(error);
                    this.Toast(JSON.parse(error.data.d));
                })
                .finally(_ => {
                    this.Products.New = null
                });
        },
        FillInThickWidth: function (product) {
            let thick = this.Thicks.List.find(t => t.ID === product.ThicknessID)
            let width = this.Widths.List.find(w => w.ID === product.WidthID)

            if (!thick || !width) { return; }

            product.ThickMin = thick.Minimum
            product.ThickNominal = thick.Nominal;
            product.ThickMax = thick.Maximum;

            product.WidthMin = width.Minimum;
            product.WidthNominal = width.Nominal;
            product.WidthMax = width.Maximum;
        },

        ShowNewGrade: function () {
            this.Grades.New = {
                "GradeID": 0,
                "GradeDescription": "",
                "GradeLabel": "",
                "GradeLabelTicket": null
            }
        },
        ShowNewThickness: function () {
            this.Thicks.New = {
                'Label': '',
                'Nominal': 0,
                'Minimum': 0,
                'Maximum': 0
            }
        },
        ShowNewWidth: function () {
            this.Widths.New = {
                'Label': '',
                'Nominal': 0,
                'Minimum': 0,
                'Maximum': 0
            }
        },
        ShowNewLength: function () {
            this.Lengths.New = {
                "LengthID": 0,
                "LengthLabel": "",
                "LengthNominal": 0,
                "LengthMin": 0,
                "LengthMax": 0,
                "PETFLag": false,
                "PETLengthID": 0
            }
        },
        ShowNewGrader: function () {
            this.Graders.New = {
                "GraderID": 0,
                "GraderDescription": "",
            }
        },
        ShowNewPETLength: function () {
            this.PETLengths.New = {
                "PETLengthID": 0,
                "LengthLabel": "",
                "SawIndex": 0,
                "LengthNominal": 0,
                "PETPosition": 0
            }
        },

        SaveNearSaw: function () {
            let data = JSON.stringify({ NearSawOffSet: this.NearSawOffSet })

            axios.post("Products.aspx/SaveNearSaw", data, this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);
                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.error(error);
                    this.Toast(JSON.parse(error.data.d));
                })
        },

        SetToast: function (response) {
            clearTimeout(this.Toast.Timeout)
            this.Toast.Timeout = null

            this.Toast.Message = response.Message;
            this.Toast.Success = response.Success;

            this.Toast.Timeout = setTimeout(_ => {
                this.Toast.Message = null
            }, 3000)
        },

        Refresh: function () {
            setTimeout(_ => this.GetProducts());
            setTimeout(_ => this.GetGrades(), 10);
            setTimeout(_ => this.GetThicknesses(), 100);
            setTimeout(_ => this.GetWidths(), 120);
            setTimeout(_ => this.GetLengths(), 130);
            setTimeout(_ => this.GetPETLengths(), 140);
            setTimeout(_ => this.GetGraders(), 150);
        },
        RefreshTab: function () {
            if (this.Tab === 'Products') {
                this.GetProducts()
                this.GetGrades()
                this.GetThicknesses()
                this.GetWidths()
            } else if (this.Tab === 'Grades') {
                this.GetGrades()
            } else if (this.Tab === 'Thicknesses') {
                this.GetThicknesses()
            } else if (this.Tab === 'Widths') {
                this.GetWidths()
            } else if (this.Tab === 'Lengths') {
                this.GetLengths()
            } else if (this.Tab === 'PETLengths') {
                this.GetPETLengths()
            } else if (this.Tab == 'Graders') {
                this.GetGraders()
            }
        },

        CompareValues: function (key, order = 'asc') {
            return function innerSort(a, b) {
                if (!a.hasOwnProperty(key) || !b.hasOwnProperty(key)) {
                    // property doesn't exist on either object
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
        Sort: function (Col, table) {
            if (table.SortBy === Col) {
                table.SortByAsc = !table.SortByAsc
            } else {
                table.SortByAsc = true
            }
            table.SortBy = Col;
        },
    },
});

window.addEventListener('keydown', function (e) { if (e.keyIdentifier == 'U+000A' || e.keyIdentifier == 'Enter' || e.keyCode == 13) { if (e.target.nodeName == 'INPUT' && e.target.type == 'text') { e.preventDefault(); return false; } } }, true);