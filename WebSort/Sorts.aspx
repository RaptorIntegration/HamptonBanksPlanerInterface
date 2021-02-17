<%@ Page Language="C#" MasterPageFile="~/WebSort.Master" MaintainScrollPositionOnPostback="true"  AutoEventWireup="true" CodeBehind="Sorts.aspx.cs" Inherits="WebSort.Sorts" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <link href="CSS/Sorts.css?v= <%= GetVersion() %>" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="vue-wrapper">
        <div id="vue-content" class="content" @keyup.enter="Table.Editing=false" tabindex="0">
            <transition name="fade">
                <div class="toast" v-bind:class="ToastClass" v-if="Toast.Message">{{ Toast.Message }}</div>
            </transition>
            <transition name="fade">
                <div class="overlay" v-if="Loading">
                    <div class="loader" >Loading...</div>
                </div>                
            </transition>
            <div class="top-row">
                <div class="accordion-container mb-3">
                    <button type="button" class="accordion" v-on:click="ToggleRecipe">
                        <div class="d-flex align-items-center">
                            <i v-if="!Recipes.Visible" class="gg-math-plus"></i>
                            <i v-else class="gg-math-minus"></i>
                            <span style="padding-left:5px;">Recipes</span>
                        </div>                        
                        <span v-if="ActiveRecipe"><strong>Editing: </strong>{{EditingRecipe.RecipeLabel}}</span>
                        <span v-if="ActiveRecipe"><strong>Active: </strong>{{ActiveRecipe.RecipeLabel}}</span>
                    </button>
                    <div class="panel" v-if="Recipes.Visible">
                        <div class="panel-grid">
                            <div class="recipe-btn-grid" v-for="Recipe in Recipes.List" >
                                <button type="button" 
                                    v-on:click="Recipes.Activating = Recipe" 
                                    v-bind:disabled="SecurityEnabled"
                                >
                                    Activate
                                </button>
                                <button type="button" 
                                    v-on:click="CopyRecipe(Recipe)" 
                                    v-bind:disabled="SecurityEnabled"                                     
                                >
                                    Copy
                                </button>                                
                                <button type="button" 
                                    v-on:click="ChangeEditingRecipe(Recipe)"                                    
                                    v-bind:class="{active: Recipe.Editing == 1}">
                                    {{Recipe.RecipeID == ActiveRecipe.RecipeID ? Recipe.RecipeLabel + ' - ACTIVE' : Recipe.RecipeLabel}}                            
                                </button>
                                <button type="button" 
                                    v-on:click="EditRecipe(Recipe)" 
                                    v-bind:disabled="SecurityEnabled" 
                                    class="btn-icon">
                                    <i class="gg-pen"></i>
                                </button>
                                <button type="button" 
                                    style="color:var(--error);" 
                                    v-bind:disabled="SecurityEnabled" 
                                    class="btn-icon">
                                    <i class="gg-trash-empty" v-on:click="DeleteRecipe(Recipe)"></i>
                                </button>                                
                            </div>

                            <div v-if="Recipes.Activating" class="confirm-recipe">
                                <span>Are you sure you want to activate <u><strong>{{Recipes.Activating.RecipeLabel}}</strong></u>?</span>
                                <label>
                                    <input type="checkbox" v-model="Recipes.Reset" />
                                    Reset Statistics
                                </label>                                
                                <label>
                                    <input type="checkbox" v-model="Recipes.Full"/>
                                    Set All Active Bays Full
                                </label>                                
                                <input type="button" class="btn-raptor mt-3" value="Save" v-on:click="ActivateRecipe()" />
                                <input type="button" class="btn-raptor" value="Cancel" v-on:click="Recipes.Activating = null" />
                            </div>
                            
                            <button type="button" 
                                class="btn-raptor btn-add-recipe mt-3" 
                                v-on:click="Recipes.AddNew = !Recipes.AddNew" 
                                v-bind:disabled="SecurityEnabled">
                                <span class="mr-2">Add New Recipe</span>
                                <i class="gg-math-plus"></i>
                            </button>     
                            <div v-if="Recipes.AddNew">
                                <div class="auto-grid">
                                    <label class="mb-1">New Name</label>
                                    <input type="text" class="form-control" v-model="Recipes.NewName" />
                                </div>
                                <div class="mt-3">
                                    <input type="button" class="btn-raptor" value="Save" v-on:click="SaveNewRecipe" />
                                    <input type="button" class="btn-raptor" value="Cancel" v-on:click="Recipes.AddNew = false" />
                                </div>                                
                            </div>
                        </div>
                    </div>
                </div>                

                <input type="button" 
                    class="btn-raptor btn-matrix mb-3" 
                    v-bind:value="GradeMatrixButton" 
                    v-on:click="ToggleGradeMatrix" />

                <div class="card sort-controls mb-3" v-show="ShowSortControls">
                    <span class="card-title">Sort Controls</span>
                    <div class="controls-grid mt-3">
                        <div>
                            <input type="button" class="btn-raptor" value="Add New Row At" v-on:click="AddNewSort" />
                        </div>
                        <div class="d-flex align-items-center justify-content-center">
                            <i class="gg-arrow-long-right"></i>
                        </div>
                        <div class="d-flex align-items-center">
                            <select class="form-control" style="width:5vw" v-model="SortControls.AddNew">
                                <option v-for="index in 75">{{index}}</option>
                            </select>
                        </div>
                        <div>
                            <input type="button" class="btn-raptor" value="Move Row" v-on:click="MoveSort" />
                        </div>
                        <div class="d-flex align-items-center justify-content-center">
                            <span class="and-or">From</span>
                        </div>
                        <div class="d-flex align-items-center justify-content-between">                                
                            <select class="form-control" style="width:5vw" v-model="SortControls.From">
                                <option v-for="index in 75">{{index}}</option>
                            </select>
                            <span class="and-or">To</span>
                            <select class="form-control" style="width:5vw" v-model="SortControls.To">
                                <option v-for="index in 75">{{index}}</option>
                            </select>
                        </div>
                    </div>
                </div>      
            </div>
            <div class="row" v-if="GradeMatrix.Visible">
                <div class="col-12">
                    <input type="button" class="btn-raptor mb-3" value="Save" v-on:click="SaveGradeMatrix" />
                    <table class="table" v-if="GradeMatrix.List.length > 0" style="max-width:1200px; margin:auto;">
                        <thead>
                            <tr>
                                <th style="width:10%;" scope="col">Input Grade ID</th>
                                <th style="width:40%;" scope="col">Sorted Grade</th>
                                <th style="width:10%;" scope="col">WEBSort Grade ID</th>
                                <th style="width:40%;" scope="col">Stamps</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="grade in GradeMatrix.List">
                                <td scope="row">{{grade.PLCGradeID}}</td>
                                <td @click="EditingGradeMatrixCell(grade, 'WebSortGradeID')">
                                    <select 
										v-on:focus="Prev(grade, 'WebSortGradeID')"
                                        v-if="Table.Editing == grade.PLCGradeID + '_WebSortGradeID'"
                                        v-on:change="UpdateGradeMatrix('WebSortGradeID', grade.WebSortGradeID, grade)"
                                        v-on:blur="Table.Editing = false"
                                        v-model="grade.WebSortGradeID" 
                                        class="form-control">
                                        <option v-for="g in GradeMatrix.Grades" v-bind:value="g.GradeID">{{g.GradeLabel}}</option>
                                    </select>
                                    <div v-else>
                                        <label>{{ GradeMatrix.Grades.find(e => e.GradeID == grade.WebSortGradeID)?.GradeLabel }}</label>
                                    </div>
                                </td>
                                <td>{{grade.WebSortGradeID}}</td>
                                <td @click="EditingGradeMatrixCell(grade, 'GradeStamps')">
                                    <select 
										v-on:focus="Prev(grade, 'GradeStamps')"
                                        v-if="Table.Editing == grade.PLCGradeID + '_GradeStamps'"
                                        v-on:change="UpdateGradeMatrix('GradeStamps', grade.GradeStamps, grade)"
                                        v-on:blur="Table.Editing = false"
                                        v-model="grade.GradeStamps" 
                                        class="form-control">
                                        <option v-for="s in Stamps" v-bind:value="s.ID">{{ s.Label }}</option>
                                    </select>
                                    <div v-else>
                                        <label>{{ Stamps.find(e => e.ID == grade.GradeStamps)?.Label }}</label>
                                    </div>                          
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div v-if="Table.Visible">
                <div class="above-table mb-3 align-items-center">
                     <div>
                        <transition name="component-fade" mode="out-in">
                            <div v-if="Table.Edited">
                                <input class="btn-save" type="button" v-on:click="Save" value="Save" />
                                <input class="btn-cancel" type="button" v-on:click="Cancel" value="Cancel" />
                            </div>                            
                        </transition>
                    </div>
                    <div class="table-sm table-edits">
                        <transition name="component-fade" mode="out-in">
                        <table class="table" v-if="SaveResponse.ChangedList">
                            <caption v-if="SaveResponse.ChangedList.length">{{ SaveResponse.ChangedList.length }} Changes Made</caption>
                            <thead>
                            <tr>
                                <th scope="col" style="width: 25%">Sort ID</th>
                                <th scope="col">Changed Column</th>
                                <th scope="col">Changed To</th>
                                <th scope="col">Previous</th>
                            </tr>
                            </thead>
                            <tbody>
                            <tr v-for="Row in SaveResponse.ChangedList">
                                <td>{{ Row.Key }}</td>
                                <td>{{ Row.EditedCol }}</td>
                                <td>{{ Row.EditedVal }}</td>
                                <td>{{ Row.Previous }}</td>
                            </tr>
                            </tbody>
                        </table>
                        </transition>
                    </div>
                    <div v-if="DropDown" style="min-height:34px;">
                        <auto-complete :data="DropDown" v-model="Table.Filter"></auto-complete>
                    </div>
                </div>
                <div class="row">
                    <div class="col-12 d-flex justify-content-center">
                        <table class="table">
                        <thead>
                            <tr>
                                <th scope="col" style="width: 2%;" @click="Sort('SortID')">ID</th>
                                <th scope="col" @click="Sort('SortLabel')">Label</th>
                                <th scope="col" v-for="Col in Table.Columns" style="width: 2%;" @click="Sort(Col.DataSource)">{{ Col.Header }}</th>
                                <th scope="col" @click="Sort('BinID')" style="width: 2%;">CN2 Override</th>
                                <th scope="col" @click="Sort('CN2Frequency')" style="width: 2%;">CN2 Frequency %</th>
                                <th scope="col" @click="Sort('SecProd')">Secondary Product</th>
                                <th scope="col" style="width:2%;" @click="Sort('SecSize')">Secondary Size %</th>
                                <th scope="col" style="width:5%;" @click="Sort('SortStamps')">Stamps</th>
                                <th scope="col" style="width:5%;" @click="Sort('SortSprays')">Premium Stamp</th>
                                <th scope="col" @click="Sort('ProductsLabel')">Products</th>
                            </tr>
                        </thead>
                        <tbody>
                            <template v-for="Row in FilterSorts">
                            <tr>
                                <th scope="row">{{ Row.SortID }}</th>
                                <td @click="EditingCell(Row, 'SortLabel')" style="white-space:nowrap;">
                                    <input
                                        v-if="Table.Editing == Row.SortID + '_SortLabel'"
                                        v-model="Row.SortLabel"
                                        v-on:blur="Update('SortLabel', Row.SortLabel, Row);"
                                        v-on:focus="Prev(Row, 'SortLabel')"
                                        type="text"
                                        spellcheck="false"
                                        class="form-control">
                                    <div v-else>
                                        <label>{{ Row.SortLabel }}</label>
                                    </div>
                                </td>
                                <td v-for="Col in Table.Columns" @click="EditingCell(Row, Col.DataSource)">
                                    <input
                                        v-if="Table.Editing == Row.SortID + '_' + Col.DataSource && typeof(Row[Col.DataSource]) != 'boolean'"
                                        v-model="Row[Col.DataSource]"
                                        v-on:blur="Update(Col.DataSource, Row[Col.DataSource], Row);"
                                        v-on:focus="Prev(Row, Col.DataSource)"
                                        type="text"
                                        spellcheck="false"
                                        class="form-control">
                                    <input
                                        v-if="typeof(Row[Col.DataSource]) === 'boolean'"
                                        v-model="Row[Col.DataSource]"
                                        v-on:blur="Table.Editing = false"
                                        v-on:change="Update(Col.DataSource, Row[Col.DataSource], Row);"
                                        type="checkbox"
                                        class="check">
                                    <div v-if="Table.Editing != Row.SortID + '_' + Col.DataSource && typeof(Row[Col.DataSource]) != 'boolean'">
                                        <label>{{ Row[Col.DataSource] }}</label>
                                    </div>
                                </td>                                
                                <td @click="EditingCell(Row, 'BinID')">
                                    <select class="form-control" 
                                        v-model.number="Row.BinID" 
                                        v-if="Table.Editing == Row.SortID + '_BinID'" 
                                        v-on:focus="Prev(Row, 'BinID')"
                                        v-on:blur="Table.Editing = false"
                                        v-on:change="Update('BinID', Row.BinID, Row);">
                                        <option v-for="val in CN2">{{val}}</option>
                                    </select>
                                    <div v-else>
                                        <label>{{ Row.BinID }}</label>
                                    </div>
                                </td>
                                <td @click="EditingCell(Row, 'CN2Frequency')">
                                     <input
                                        v-if="Table.Editing == Row.SortID + '_CN2Frequency'"
                                        v-model="Row.CN2Frequency"
                                        v-on:blur="Update('CN2Frequency', Row.CN2Frequency, Row);"
                                        v-on:focus="Prev(Row, 'CN2Frequency')"
                                        type="text"
                                        spellcheck="false"
                                        class="form-control">
                                    <div v-else>
                                        <label>{{ Row.CN2Frequency }}</label>
                                    </div>
                                </td>
                                <td @click="EditingCell(Row, 'SecProdID')">
                                    <select class="form-control" 
                                        v-model.number="Row.SecProdID" 
                                        v-if="Table.Editing == Row.SortID + '_SecProdID'" 
                                        v-on:blur="Table.Editing = false"
                                        v-on:change="Update('SecProdID', Row.SecProdID, Row);">
                                        <option value="0">None</option>
                                        <option v-for="prod in Table.Products" v-bind:value="prod.ID">{{prod.Label}}</option>
                                    </select>
                                    <div v-else>
                                        <label>{{ Row.SecProdID == 0 ? 'None' : Table.Products.find(f => f.ID == Row.SecProdID).Label }}</label>
                                    </div>
                                </td>
                                <td @click="EditingCell(Row, 'SecSize')">
                                    <input
                                        v-if="Table.Editing == Row.SortID + '_SecSize'"
                                        v-model.number="Row.SecSize"
                                        v-on:blur="Row.SecSize = Math.round(Row.SecSize); Update('SecSize', Row.SecSize, Row);"
                                        v-on:focus="Prev(Row, 'SecSize')"
                                        type="number"
                                        spellcheck="false"
                                        class="form-control">
                                    <div v-else>
                                        <label>{{ Row.SecSize }}</label>
                                    </div>
                                </td>
                                <td @click="EditingCell(Row, 'SortStamps')">
                                    <select 
										v-on:focus="Prev(Row, 'SortStamps')"
                                        v-if="Table.Editing == Row.SortID + '_SortStamps'"
                                        v-on:change="UpdateGradeMatrix('SortStamps', Row.SortStamps, Row)"
                                        v-on:blur="Table.Editing = false"
                                        v-model="Row.SortStamps" 
                                        class="form-control">
                                        <option v-for="s in Stamps" v-bind:value="s.ID">{{ s.Label }}</option>
                                    </select>
                                    <div v-else>
                                        <label>{{ Stamps.find(e => e.ID == Row.SortStamps)?.Label }}</label>
                                    </div>                          
                                </td>
                                <td @click="EditingCell(Row, 'SortSprays')">
                                    <input
                                        v-model="Row.SortSprays"
                                        v-on:blur="Table.Editing = false"
                                        v-on:change="Update('SortSprays', Row.SortSprays, Row);"
                                        type="checkbox"
                                        class="check">
                                </td>
                                <td @click="EditingCell(Row, 'ProductsLabel'); ">
                                    <div>
                                        <label>{{ Row.ProductsLabel }}</label>
                                    </div>
                                </td>
                            </tr>
                            <tr class="no-hover" v-if="Table.Editing === Row.SortID + '_ProductsLabel'">
                                <td colspan="12" class="no-hover">
                                    <div class="row" style="padding:5px;" v-on:click="Table.Editing = null">
                                        <div class="col-10">
                                        <p style="font-size:14px; font-weight:700;">Products</p>
                                        </div>
                                        <div class="col-1">
                                        <p style="font-size:14px; font-weight:700;">Lengths</p>
                                        </div>
                                    </div>
                                    <div class="row" v-if="Row.ProdLen">
                                        <div class="col-10 product-grid">
                                            <div class="checkboxlist"
                                                v-for="Prod in Row.ProdLen.ProductsList"
                                                v-bind:key="Row.SortID + '_' + Prod.ID">
                                                <input
                                                type="checkbox"
                                                class="check"
                                                v-bind:id="'prod_' + Prod.Label"
                                                v-model="Prod.Selected"
                                                @change="ProductsChange(Prod, Row)">
                                                <label :for="'prod_' + Prod.Label">
                                                {{ Prod.Label }}
                                                </label>
                                            </div>
                                        </div>
                                        <div class="form-group col-2 flex-col length-grid">
                                            <div class="checkboxlist"
                                                v-for="Length in Row.ProdLen.LengthsList"
                                                v-bind:key="Length.ID">
                                                <input
                                                type="checkbox"
                                                class="check"
                                                v-bind:id="'len_' + Length.Label"
                                                v-model="Length.Selected"
                                                @change="ProductsChange(Length, Row)">
                                                <label :for="'len_' + Length.Label">
                                                {{ Length.Label }}
                                                </label>
                                            </div>      
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            </template>
                        </tbody>
                        </table>
                    </div>
               </div>
               
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="JavaScript" runat="server">
    <script type="text/javascript" src="Scripts/axios/axios.min.js"></script>
    <script type="text/javascript" src="Scripts/vue/vue.js"></script>
    <script type="text/javascript" src="Scripts/Sorts.js?v= <%= GetVersion() %>"></script>
</asp:Content>