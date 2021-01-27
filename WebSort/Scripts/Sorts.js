Vue.component('auto-complete', {
    template: `
<div class="col-12">
    <input placeholder="Filter..." class="form-control" type="text" v-bind:value="value" v-on:input="updateValue($event.target.value)" @keydown.tab.prevent="complete()" @focus="focus(true); Clear()" @blur="focus(false)" spellcheck="false">
   	<div style="max-height:18px;">
        <div v-if="focused && SelectionList" class="selection-container">
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

const v = new Vue({
    el: '#vue-wrapper',
    name: 'SortsTable',
    data: {
        Table: {
            Visible: true,
            Sorts: [],
            Columns: [
                { DataSource: 'Active', Header: 'Active' },
                { DataSource: 'SortSize', Header: 'Size' },
                { DataSource: 'Zone1Start', Header: 'Zone1 Start' },
                { DataSource: 'Zone1Stop', Header: 'Zone1 Stop' },
                { DataSource: 'Zone2Start', Header: 'Zone2 Start' },
                { DataSource: 'Zone2Stop', Header: 'Zone2 Stop' },
                { DataSource: 'PkgsPerSort', Header: 'Pkgs /Sort' },
                { DataSource: 'RW', Header: 'RW' },
                { DataSource: 'OrderCount', Header: 'Order Count' }
            ],
            Products: [],

            Previous: '',
            Filter: '',
            SortBy: '',
            SortByAsc: true,
            Edited: false,
            Editing: false,
            Timer: '',
        },

        Security: null,

        Recipes: {
            Visible: false,
            List: [],
            Activating: null,
            Full: false,
            Reset: false,
            ToEdit: null,
            AddNew: false,
            NewName: '',
        },

        SortControls: {
            AddNew: 1,
            From: 1,
            To: 1
        },

        GradeMatrix: {
            Visible: false,
            List: [],
            Stamps: [],
            Grades: []
        },

        Toast: {
            Timeout: null,
            Success: false,
            Message: null
        },

        SaveResponse: {
            Message: '',
            ChangeList: [{
                Key: '',
                EditedCol: '',
                EditedVal: ''
            }]
        },

        Loading: false,
        Headers: {
            headers: {
                'Content-Type': 'application/json; charset=UTF-8',
                'Accept': 'application/json'
            }
        }
    },
    mounted: function () {
        this.GetSecurity()
        this.SetAutoUpdate();
        this.GetRecipes();
        this.GetProductGrades();
    },
    computed: {
        FilterSorts: function () {
            let v = this
            let SortDir = v.Table.SortByAsc ? 'asc' : 'desc';

            if (v.Table.SortBy && v.ChangedList.length == 0) {
                v.Table.Sorts.sort(v.CompareValues(v.Table.SortBy, SortDir));
            }
            if (v.Table.Filter) {
                return v.Table.Sorts.filter(s => {
                    return (s.SortLabel.toUpperCase().includes(v.Table.Filter.toUpperCase())
                        | (s.Active ? 'active' : 'inactive') === v.Table.Filter
                        | s.SortID.toString().includes(v.Table.Filter)
                        | v.ChangedList.some(e => e.SortID == s.SortID))
                });
            } else {
                return v.Table.Sorts
            }
        },
        ChangedList: function () {
            return this.Table.Sorts.filter(s => {
                return s.Changed
            });
        },

        DropDown: function () {
            let v = this
            if (v.Table.Sorts.length > 0) {
                let unique = [...new Set(
                    v.Table.Sorts.filter(
                        f => f.SortLabel != ''
                    ).map(
                        item => item.SortLabel.split(" ")[0].toLowerCase()
                    )
                )];
                if (unique) {
                    unique.push('active');
                    unique.push('inactive')
                }
                return unique.sort()
            }
            return null
        },

        EditingRecipe: function () {
            if (this.Recipes.List.length > 0) {
                return this.Recipes.List.find(r => r.Editing == 1)
            } else {
                return null;
            }
        },
        ActiveRecipe: function () {
            if (this.Recipes.List.length > 0) {
                return this.Recipes.List.find(r => r.Online == 1)
            } else {
                return null;
            }
        },
        ShowSortControls: function () {
            return this.EditingRecipe && this.ActiveRecipe && !this.GradeMatrix.Visible && this.EditingRecipe.RecipeID != this.ActiveRecipe.RecipeID
        },

        GradeMatrixButton: function () {
            return this.GradeMatrix.Visible ? 'Hide Grade Matrix' : 'Show Grade Matrix'
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
        GetData: function () {
            let v = this;
            axios.post('Sorts.aspx/GetData', v.Headers)
                .then(response => {
                    v.Table.Sorts = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.dir(error);
                });
        },
        GetProductList: function (Row) {
            let v = this;
            let data = { SortID: Row.SortID.toString() };

            axios.post("Sorts.aspx/GetProductList", data, v.Headers)
                .then(response => {
                    let ParsedJson = JSON.parse(response.data.d);
                    v.$set(Row, "ProdLen", ParsedJson);
                })
                .catch(error => {
                    console.dir(error);
                })
        },
        GetRecipes: function () {
            axios.post('Sorts.aspx/GetRecipes', this.Headers)
                .then(response => {
                    this.Recipes.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetGradeMatrix: function () {
            axios.post('Sorts.aspx/GetGradeMatrix', this.Headers)
                .then(response => {
                    this.GradeMatrix.List = JSON.parse(response.data.d);
                    this.GetGrades();
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetGrades: function () {
            axios.post('Sorts.aspx/GetGrades', this.Headers)
                .then(response => {
                    this.GradeMatrix.Grades = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetSecurity: function () {
            axios.post('Sorts.aspx/GetSecurity', this.Headers)
                .then(response => {
                    this.Security = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetProductGrades: function () {
            axios.post('Sorts.aspx/GetProductGrades', this.Headers)
                .then(response => {
                    this.Table.Products = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },

        ToggleGradeMatrix: function () {
            if (!this.GradeMatrix.Visible) {
                this.GetGradeMatrix();
                this.CancelAutoUpdate();
            } else {
                this.SetAutoUpdate();
                this.Editing = false;
                this.Table.Edited = false;
            }

            this.GradeMatrix.Visible = !this.GradeMatrix.Visible
            this.Table.Visible = !this.GradeMatrix.Visible
        },
        SaveGradeMatrix: function () {
            let data = JSON.stringify({ changes: this.GradeMatrix.List.filter(g => g.Changed) })
            axios.post("Sorts.aspx/SaveGradeMatrix", data, v.Headers)
                .then(response => {
                    console.dir(response)
                    this.SetToast(JSON.parse(response.data.d))
                })
                .catch(error => {
                    this.SetToast(JSON.parse(response.data.d))
                    console.error(error);
                })
                .finally(_ => {
                    this.Editing = null
                    this.Recipes.AddNew = false
                });
        },

        ChangeEditingRecipe: function (recipe) {
            axios.post('Sorts.aspx/ChangeEditingRecipe', JSON.stringify({ recipe: recipe }), this.Headers)
                .then(response => {
                    this.Recipes.List = JSON.parse(response.data.d);
                    this.GetData();
                    if (this.GradeMatrix.Visible) {
                        this.GetGradeMatrix()
                    }
                })
                .catch(error => {
                    this.SetToast(JSON.parse(error.data.d))
                    console.error(error);
                });
        },
        ToggleRecipe: function () {
            this.Recipes.Visible = !this.Recipes.Visible
        },
        EditRecipe: function (recipe) {
            this.Recipes.ToEdit = recipe
            this.Recipes.AddNew = true
        },
        DeleteRecipe: function (recipe) {
            if (recipe.RecipeID == this.ActiveRecipe.RecipeID) {
                alert('Can not delete active recipe!');
                return;
            }

            let message = `Are you sure you would like to delete ${recipe.RecipeLabel}`
            if (!confirm(message)) { return; }

            let data = JSON.stringify({ 'recipe': recipe })
            axios.post("Sorts.aspx/DeleteRecipe", data, v.Headers)
                .then(response => {
                    console.dir(response.data.d)
                    this.GetRecipes();
                    this.SetToast(JSON.parse(response.data.d))
                })
                .catch(error => {
                    this.SetToast(JSON.parse(error.data.d))
                    console.error(error);
                });
            this.GetData();
        },
        SaveNewRecipe: function () {
            let data = JSON.stringify({ NewName: this.Recipes.NewName, recipe: this.Recipes.ToEdit })
            axios.post("Sorts.aspx/SaveNewRecipe", data, v.Headers)
                .then(response => {
                    console.dir(response)
                    this.GetRecipes();
                    this.SetToast(JSON.parse(response.data.d))
                })
                .catch(error => {
                    this.SetToast(JSON.parse(error.data.d))
                    console.error(error);
                })
                .finally(_ => {
                    this.Recipes.AddNew = false
                    this.Recipes.ToEdit = null
                    this.Recipes.NewName = null
                });

            this.GetData();
        },
        CopyRecipe: function (recipe) {
            let data = JSON.stringify({ 'recipe': recipe })
            axios.post('Sorts.aspx/CopyRecipe', data, this.Headers)
                .then(response => {
                    console.dir(response.data.d)
                    this.GetRecipes();
                    this.SetToast(JSON.parse(response.data.d))
                })
                .catch(error => {
                    this.SetToast(JSON.parse(error.data.d))
                    console.error(error);
                });
        },
        ActivateRecipe: function () {
            this.Loading = true
            let data = JSON.stringify({
                'recipe': this.Recipes.Activating,
                'full': this.Recipes.Full,
                'reset': this.Recipes.Reset
            })

            axios.post('Sorts.aspx/ActivateRecipe', data, this.Headers)
                .then(response => {
                    console.dir(response)
                    this.GetRecipes();
                    this.SetToast(JSON.parse(response.data.d))
                })
                .catch(error => {
                    this.SetToast(JSON.parse(error.data.d))
                    console.error(error);
                })
                .finally(_ => {
                    this.Recipes.Activating = null;
                    this.Loading = false
                });
        },

        MoveSort: function () {
            if (this.EditingRecipe.RecipeID == this.ActiveRecipe.RecipeID) {
                return;
            }
            let data = JSON.stringify({ from: this.SortControls.From, to: this.SortControls.To })
            axios.post('Sorts.aspx/MoveSort', data, this.Headers)
                .then(response => {
                    console.dir(response);
                    this.GetData();
                })
                .catch(error => {
                    console.error(error);
                });
        },
        AddNewSort: function () {
            if (this.EditingRecipe.RecipeID == this.ActiveRecipe.RecipeID) {
                return;
            }
            axios.post('Sorts.aspx/AddNewSort', JSON.stringify({ ID: this.SortControls.AddNew }), this.Headers)
                .then(response => {
                    console.dir(response);
                    this.GetData();
                })
                .catch(error => {
                    console.error(error);
                });
        },

        EditingCell: function (Row, Col) {
            if (this.Table.Editing && this.Table.Editing.includes("ProductsLabel") && this.Table.Editing === Row.SortID + '_' + Col) {
                this.Table.Editing = null
                return;
            }
            this.Table.Editing = (Row.SortID + '_' + Col);
            this.CancelAutoUpdate();
            if (Col == 'ProductsLabel') {
                this.GetProductList(Row);
            }
            if (Col === 'SecProdID') {
                this.Table.Previous = Row[Col]
            }
        },
        EditingGradeMatrixCell: function (Grade, Col) {
            this.Editing = (Grade.PLCGradeID + '_' + Col);
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
            v.Table.Edited = true;
        },
        UpdateGradeMatrix: function (EditedCol, EditedVal, ChangedRow) {
            let v = this;

            ChangedRow.Changed = true;
            if (ChangedRow.Changed && ChangedRow.EditsList.some(e => e.EditedCol === EditedCol)) {
                ChangedRow.EditsList.find(e => e.EditedCol === EditedCol).EditedVal = EditedVal;
            } else {
                if (typeof (EditedVal) === 'boolean') {
                    v.Table.Previous = !EditedVal;
                }
                ChangedRow.EditsList.push({
                    'Key': ChangedRow.SortID,
                    'EditedCol': EditedCol,
                    'EditedVal': EditedVal,
                    'Previous': v.Table.Previous
                });
            }

            v.Table.Edited = true;
            setTimeout(_ => v.Editing = false, 50);
        },
        Update: function (EditedCol, EditedVal, ChangedRow) {
            let v = this;

            ChangedRow.Changed = true;
            if (ChangedRow.Changed && ChangedRow.EditsList.some(e => e.EditedCol === EditedCol)) {
                ChangedRow.EditsList.find(e => e.EditedCol === EditedCol).EditedVal = EditedVal;
            } else {
                if (typeof (EditedVal) === 'boolean') {
                    v.Table.Previous = !EditedVal;
                }
                ChangedRow.EditsList.push({
                    'Key': ChangedRow.SortID,
                    'EditedCol': EditedCol,
                    'EditedVal': EditedVal,
                    'Previous': v.Table.Previous
                });
            }

            v.Table.Edited = true;
            v.Table.Editing = false;
        },
        Prev: function (Row, Col) {
            this.Table.Previous = Row[Col];
        },
        Save: function () {
            this.Loading = true;
            let data = JSON.stringify({
                Changed: this.ChangedList,
                'ActiveRecipe': this.EditingRecipe.RecipeID == this.ActiveRecipe.RecipeID
            });

            axios.post("Sorts.aspx/Save", data, this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);
                    console.dir(response);
                    this.SetAutoUpdate();
                    this.SaveResponse = Parsed;
                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.dir(error);
                    this.Toast(JSON.parse(error.data.d));
                }).
                finally(_ => {
                    this.Loading = false;
                    this.Table.Edited = false;
                    this.Table.Editing = false;
                });
        },
        Cancel: function () {
            this.SetAutoUpdate();
            this.Loading = false;
            this.Table.Edited = false;
            this.Table.Editing = false;
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
        Sort: function (Col) {
            let v = this
            if (v.Table.SortBy === Col) {
                v.Table.SortByAsc = !v.Table.SortByAsc
            } else {
                v.Table.SortByAsc = true
            }
            v.Table.SortBy = Col;
        },

        CancelAutoUpdate: function () {
            clearInterval(this.Table.Timer);
        },
        SetAutoUpdate: function () {
            this.CancelAutoUpdate();
            this.GetData();
            this.Table.Timer = setInterval(this.GetData, 1000 * 5);
        }
    },
    beforeDestroy() {
        clearInterval(this.Table.timer)
    }
});

function UpdateTable() {
    setTimeout(v.GetData, 500);
}

window.addEventListener('keydown', function (e) { if (e.keyIdentifier == 'U+000A' || e.keyIdentifier == 'Enter' || e.keyCode == 13) { if (e.target.nodeName == 'INPUT' && e.target.type == 'text') { e.preventDefault(); return false; } } }, true);