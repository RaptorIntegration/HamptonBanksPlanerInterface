<%@ Page Title="" Language="C#" MaintainScrollPositionOnPostback="true" MasterPageFile="~/WebSort.Master" AutoEventWireup="true" CodeBehind="Products.aspx.cs" Inherits="WebSort.WebForm2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="CSS/Products.css?v= <%= GetVersion() %>" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="vue">
        <div>
            <transition name="fade">
                <div class="toast" v-bind:class="ToastClass" v-if="Toast.Message">{{ Toast.Message }}</div>
            </transition>
            <div class="tab-grid tab mb-5">
                <input type="button" class="tablinks" v-bind:class="{active: Tab == 'Products'}" value="Products" v-on:click="Tab = 'Products'" />
                <input type="button" class="tablinks" v-bind:class="{active: Tab == 'Grades'}" value="Grades" v-on:click="Tab = 'Grades'" />
                <input type="button" class="tablinks" v-bind:class="{active: Tab == 'Thicknesses'}" value="Thicknesses" v-on:click="Tab = 'Thicknesses'" />
                <input type="button" class="tablinks" v-bind:class="{active: Tab == 'Widths'}" value="Widths" v-on:click="Tab = 'Widths'" />
                <input type="button" class="tablinks" v-bind:class="{active: Tab == 'Lengths'}" value="Lengths" v-on:click="Tab = 'Lengths'" />
                <input type="button" class="tablinks" style="display: none" v-bind:class="{active: Tab == 'PETLengths'}" value="PET Lengths" v-on:click="Tab = 'PETLengths'" />
                <input type="button" class="tablinks" v-bind:class="{active: Tab == 'Graders'}" value="Graders" v-on:click="Tab = 'Graders'" />
            </div>
            <transition name="fade" mode="out-in">
                <div v-if="Tab === 'Products'" key="0" >
                    <div class="above-table mb-3">
                        <div>
                            <input type="button" class="btn-raptor mr-4" value="Add New Product(s)" v-on:click="ShowNewProduct" v-bind:disabled="SecurityEnabled" />
                            <input type="button" class="btn-raptor" value="Save Change(s)" v-on:click="Save(Products, 'products')" v-if="Products.Edited" />
                        </div>
                        <div>
                            <div class="table-sm" v-if="Products.SaveResponse.ChangedList.length">
                                <transition name="fade" mode="out-in">
                                    <table class="table table-edits" v-if="Products.SaveResponse.ChangedList">
                                        <caption v-if="Products.SaveResponse.ChangedList.length">{{ Products.SaveResponse.ChangedList.length }} Changes Made</caption>
                                        <thead>
                                        <tr>
                                            <th scope="col" style="width: 25%">Product ID</th>
                                            <th scope="col">Changed Column</th>
                                            <th scope="col">Changed To</th>
                                            <th scope="col">Previous</th>
                                        </tr>
                                        </thead>
                                        <tbody>
                                        <tr v-for="Row in Products.SaveResponse.ChangedList">
                                            <td>{{ Row.Key }}</td>
                                            <td>{{ Row.EditedCol }}</td>
                                            <td>{{ Row.EditedVal }}</td>
                                            <td>{{ Row.Previous }}</td>
                                        </tr>
                                        </tbody>
                                    </table>
                                </transition>
                            </div>    
                            <div class="card" v-if="Products.New">
                                <span class="card-title">New Product</span>
                                <div class="product-grid mt-3">
                                    <div>
                                        <label>Label (ex. "2 x 4")</label>
                                        <input type="text" class="form-control" v-model="Products.New.ProdLabel" />
                                    </div>
                                    <div>
                                        <label>Grade</label>
                                        <select class="form-control" v-model.number="Products.New.GradeID">
                                            <option value="-1">All</option>
                                            <option v-for="grade in Grades.List" v-bind:value="grade.GradeID">{{grade.GradeLabel}}</option>
                                        </select>
                                    </div>
                                    <div>
                                        <span class="card-title">Thickness</span>
                                        <select class="form-control" v-model.number="Products.New.ThicknessID">
                                            <option value="0">Min - Nom - Max</option>
                                            <option v-for="thick in ThickSortedByNom" v-bind:value="thick.ID">{{thick.Minimum.toFixed(3) + ' - ' + thick.Nominal.toFixed(3) + ' - ' + thick.Maximum.toFixed(3)}}</option>
                                        </select>
                                    </div>
                                    <div>
                                        <span class="card-title">Width</span>
                                         <select class="form-control" v-model.number="Products.New.WidthID">
                                            <option value="0">Min - Nom - Max</option>
                                            <option v-for="width in WidthSortedByNom" v-bind:value="width.ID">{{width.Minimum.toFixed(3) + ' - ' + width.Nominal.toFixed(3) + ' - ' + width.Maximum.toFixed(3)}}</option>
                                        </select>
                                    </div>   
                                    <div>
                                        <input type="button" class="btn-raptor" value="Save" v-on:click="AddNewProduct" v-bind:disabled="SecurityEnabled" />
                                        <input type="button" class="btn-raptor" value="Cancel" v-on:click="CancelNew(Products)" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div>
                            <input 
                                placeholder="Filter..." 
                                class="form-control" 
                                type="text" 
                                v-model="Products.Filter"
                                @focus="Products.Focused = true; Products.Filter = ''" 
                                @blur="Products.Focused = false" 
                                spellcheck="false">
   	                        <div style="max-height:18px;">
                                <div v-if="Products.Focused" class="selection-container">
                                    <div class="selection-list" v-for="Dim in ProductsList" @mousedown="Products.Filter = Dim">{{ Dim }}</div>
                                </div>
                            </div>
                        </div>                        
                    </div>
                    <div class="row" @keyup.enter="Products.Editing = false" tabindex="0" style="outline:none;">
                        <div class="col-12 d-flex justify-content-center">
                            <transition name="fade" mode="out-in">
                            <table style="max-width: 1300px;" class="table" v-if="Products.List.length > 0 && Grades.List.length > 0">
                                <caption>Product Details</caption>
                                <thead>
                                    <tr>
                                        <th style="width:5%;"></th>
                                        <th style="width:5%;" v-on:click="Sort('ProdID', Products)">ID</th>
                                        <th style="width:25%;" v-on:click="Sort('ProdLabel', Products)">Label</th>
                                        <th style="width:15%;" v-on:click="Sort('GradeID', Products)">Grade</th>
                                        <th style="width:25%;" v-on:click="Sort('ThickNominal', Products)">
                                            Thickness<br />
                                            <div class="d-flex justify-content-around">
                                                <span>Nom</span>
                                                <span>Min</span>
                                                <span>Max</span>
                                            </div>                                            
                                        </th>                                        
                                        <th style="width:25%;" v-on:click="Sort('WidthNominal', Products)">
                                            Width<br />
                                            <div class="d-flex justify-content-around">
                                                <span>Nom</span>
                                                <span>Min</span>
                                                <span>Max</span>
                                            </div>     
                                        </th>                                        
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="product in FilteredProducts">
                                        <td>
                                            <button type="button" v-on:click="DeleteProduct(product)" class="btn-trash" v-bind:disabled="SecurityEnabled">
                                                <i class="gg-trash-empty"></i>
                                            </button>
                                        </td>
                                        <td>{{product.ProdID}}</td>
                                        <td @click="EditingCell(Products, 'ProdLabel', product.ProdID)">                                            
                                            <input
                                                v-if="Products.Editing == product.ProdID + '_ProdLabel'"
                                                v-model="product.ProdLabel"
                                                v-on:blur="Update('ProdLabel', product.ProdLabel, product, Products, 'ProdID');"
                                                v-on:focus="Prev(product.ProdLabel, Products)"
                                                type="text"
                                                spellcheck="false"
                                                class="form-control">
                                            <div v-else>
                                                <label>{{product.ProdLabel}}</label>
                                            </div>
                                        </td>
                                        <td @click="EditingCell(Products, 'GradeID', product.ProdID)">
                                            <select 
                                                class="form-control" 
                                                v-model="product.GradeID" 
                                                v-if="Products.Editing == product.ProdID + '_GradeID'" 
                                                v-on:change="Update('GradeID', product.GradeID, product, Products, 'ProdID');"
                                                v-on:focus="Prev(product.GradeID, Products)">
                                                <option v-for="grade in Grades.List" v-bind:value="grade.GradeID">{{grade.GradeLabel}}</option>
                                            </select>
                                            <div v-else>
                                                {{Grades.List[product.GradeID].GradeLabel}}
                                            </div>                                            
                                        </td>
                                        <td @click="EditingCell(Products, 'ThicknessID', product.ProdID)">
                                            <select 
                                                class="form-control" 
                                                v-model="product.ThicknessID" 
                                                v-if="Products.Editing == product.ProdID + '_ThicknessID'" 
                                                v-on:change="Update('ThicknessID', product.ThicknessID, product, Products, 'ProdID');"
                                                v-on:focus="Prev(product.ThicknessID, Products)">
                                                <option v-for="thick in ThickSortedByNom" v-bind:value="thick.ID">{{thick.Nominal.toFixed(3) + ' - ' + thick.Minimum.toFixed(3) + ' - ' + thick.Maximum.toFixed(3)}}</option>
                                            </select>
                                            <div v-else class="d-flex justify-content-around">
                                                <span>{{product.ThickNominal.toFixed(3)}}</span>
                                                <span>{{product.ThickMin.toFixed(3)}}</span>
                                                <span>{{product.ThickMax.toFixed(3)}}</span>
                                            </div> 
                                        </td>   
                                        <td @click="EditingCell(Products, 'WidthID', product.ProdID)">
                                            <select 
                                                class="form-control" 
                                                v-model="product.WidthID" 
                                                v-if="Products.Editing == product.ProdID + '_WidthID'" 
                                                v-on:change="Update('WidthID', product.WidthID, product, Products, 'ProdID');"
                                                v-on:focus="Prev(product.WidthID, Products)">
                                                <option v-for="width in WidthSortedByNom" v-bind:value="width.ID">{{width.Nominal.toFixed(3) + ' - ' + width.Minimum.toFixed(3) + ' - ' + width.Maximum.toFixed(3)}}</option>
                                            </select>
                                            <div v-else class="d-flex justify-content-around">
                                                <span>{{product.WidthNominal.toFixed(3)}}</span>
                                                <span>{{product.WidthMin.toFixed(3)}}</span>
                                                <span>{{product.WidthMax.toFixed(3)}}</span>
                                            </div> 
                                        </td>      
                                    </tr>
                                </tbody>
                            </table>
                            </transition>
                        </div>                        
                    </div>
                </div>
                <div v-else-if="Tab === 'Grades'" key="1" class="grades" >
                    <div class="above-table mb-3">
                        <div>
                            <input type="button" class="btn-raptor mr-4" value="Add New Grade" v-on:click="ShowNewGrade" v-bind:disabled="SecurityEnabled" />
                            <input type="button" class="btn-raptor" value="Save Change(s)" v-on:click="Save(Grades, 'grades')" v-if="Grades.Edited" />
                        </div>                        
                    </div>  
                    <div class="row" @keyup.enter="Grades.Editing = false" tabindex="0" style="outline:none;">
                        <div class="grade-grid">
                            <div class="card grade-card" v-if="Grades.New">
                                <span class="card-title">New Grade</span>
                                <div class="product-grid mt-3">
                                    <div>
                                        <label>Label</label>
                                        <input type="text" class="form-control" v-model="Grades.New.GradeLabel" />
                                    </div>
                                    <div>
                                        <label>Description</label>
                                        <input type="text" class="form-control" v-model="Grades.New.GradeDescription" />                                            
                                    </div>
                                    <div>
                                        <input type="button" class="btn-raptor" value="Save" v-on:click="AddNew(Grades, 'grade')" v-bind:disabled="SecurityEnabled" />
                                        <input type="button" class="btn-raptor" value="Cancel" v-on:click="CancelNew(Grades)" />
                                    </div>
                                </div>
                            </div>
                            <transition name="fade" mode="out-in">
                                <table style="max-width: 600px;" class="table" v-if="Grades.List.length > 0">
                                    <thead>
                                        <tr>
                                            <th style="width:30%;" v-on:click="Sort('GradeID', Grades)">ID</th>
                                            <th style="width:30%;" v-on:click="Sort('GradeLabel', Grades)">Grade</th>
                                            <th style="width:30%;" v-on:click="Sort('GradeDescription', Grades)">Description</th>                                            
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="grade in GradesSorted">
                                            <td>{{grade.GradeID}}</td>
                                            <td @click="EditingCell(Grades, 'GradeLabel', grade.GradeID)">                                            
                                                <input
                                                    v-if="Grades.Editing === grade.GradeID + '_GradeLabel'"
                                                    v-model="grade.GradeLabel"
                                                    v-on:blur="Update('GradeLabel', grade.GradeLabel, grade, Grades, 'GradeID');"
                                                    v-on:focus="Prev(grade.GradeLabel, Grades)"
                                                    type="text"
                                                    spellcheck="false"
                                                    class="form-control">
                                                <div v-else>
                                                    <label>{{grade.GradeLabel}}</label>
                                                </div>
                                            </td>
                                            <td @click="EditingCell(Grades, 'GradeDescription', grade.GradeID)">                                            
                                                <input
                                                    v-if="Grades.Editing === grade.GradeID + '_GradeDescription'"
                                                    v-model="grade.GradeDescription"
                                                    v-on:blur="Update('GradeDescription', grade.GradeDescription, grade, Grades, 'GradeID');"
                                                    v-on:focus="Prev(grade.GradeDescription, Grades)"
                                                    type="text"
                                                    spellcheck="false"
                                                    class="form-control">
                                                <div v-else>
                                                    <label>{{grade.GradeDescription}}</label>
                                                </div>
                                            </td>                                   
                                        </tr>
                                    </tbody>
                                </table>
                            </transition>
                            <div class="table-sm">
                                <table class="table table-edits" v-if="Grades.SaveResponse.ChangedList.length">
                                    <caption v-if="Grades.SaveResponse.ChangedList.length">{{ Grades.SaveResponse.ChangedList.length }} Changes Made</caption>
                                    <thead>
                                    <tr>
                                        <th scope="col" style="width: 25%">Grade ID</th>
                                        <th scope="col">Changed Column</th>
                                        <th scope="col">Changed To</th>
                                        <th scope="col">Previous</th>
                                    </tr>
                                    </thead>
                                    <tbody>
                                    <tr v-for="Row in Grades.SaveResponse.ChangedList">
                                        <td>{{ Row.Key }}</td>
                                        <td>{{ Row.EditedCol }}</td>
                                        <td>{{ Row.EditedVal }}</td>
                                        <td>{{ Row.Previous }}</td>
                                    </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
                <div v-else-if="Tab === 'Thicknesses'" key="2" class="grades">
                     <div class="above-table mb-3">
                        <div>
                            <input type="button" class="btn-raptor mr-4" value="Add New Thickness" v-on:click="ShowNewThickness" v-bind:disabled="SecurityEnabled" />
                            <input type="button" class="btn-raptor" value="Save Change(s)" v-on:click="Save(Thicks, 'thicks')" v-if="Thicks.Edited" />
                        </div>                        
                    </div>  
                    <div class="row" @keyup.enter="Thicks.Editing = false" tabindex="0" style="outline:none;">
                        <div class="grade-grid">
                             <div class="card grade-card" v-if="Thicks.New">
                                <span class="card-title">New Thickness</span>
                                <div class="product-grid mt-3">
                                    <div>
                                        <label>Label</label>
                                        <input type="text" class="form-control" v-model="Thicks.New.Label" />        
                                    </div>
                                    <div>
                                        <label>Nominal</label>
                                        <input type="number" class="form-control" v-model.number="Thicks.New.Nominal" />                                            
                                    </div>
                                    <div>
                                        <label>Min</label>
                                        <input type="number" class="form-control" v-model.number="Thicks.New.Minimum" />                                            
                                    </div>
                                    <div>
                                        <label>Max</label>
                                        <input type="number" class="form-control" v-model.number="Thicks.New.Maximum" />                                            
                                    </div>
                                    <div>
                                        <input type="button" class="btn-raptor" value="Save" v-on:click="AddNew(Thicks, 'thick')" v-bind:disabled="SecurityEnabled" />
                                        <input type="button" class="btn-raptor" value="Cancel" v-on:click="CancelNew(Thicks)" />
                                    </div>
                                </div>
                            </div>
                            <transition name="fade" mode="out-in">
                                <table style="max-width: 800px;" class="table" v-if="Thicks.List.length > 0">
                                    <thead>
                                        <tr>
                                            <th style="width:10%;" v-on:click="Sort('ID', Thicks)">ID</th>
                                            <th v-on:click="Sort('Label', Thicks)">Label</th>
                                            <th style="width:20%;" v-on:click="Sort('Nominal', Thicks)">Nominal</th>
                                            <th style="width:20%;" v-on:click="Sort('Minimum', Thicks)">Min</th>
                                            <th style="width:20%;" v-on:click="Sort('Maximum', Thicks)">Max</th>                                         
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="thick in ThicksSorted">
                                            <td>{{thick.ID}}</td>
                                            <td @click="EditingCell(Thicks, 'Label', thick.ID)">                                            
                                                <input
                                                    v-if="Thicks.Editing === thick.ID + '_Label'"
                                                    v-model="thick.Label"
                                                    v-on:blur="Update('Label', thick.Label, thick, Thicks, 'ID');"
                                                    v-on:focus="Prev(thick.Label, Thicks)"
                                                    type="text"
                                                    spellcheck="false"
                                                    class="form-control">
                                                <div v-else>
                                                    <label>{{thick.Label}}</label>
                                                </div>
                                            </td> 
                                            <td @click="EditingCell(Thicks, col, thick.ID)" v-for="col in Thicks.Cols">                                            
                                                <input
                                                    v-if="Thicks.Editing === thick.ID + '_' + col"
                                                    v-model.number="thick[col]"
                                                    v-on:blur="Update(col, thick[col], thick, Thicks, 'ID');"
                                                    v-on:focus="Prev(thick[col], Thicks)"
                                                    type="number"
                                                    spellcheck="false"
                                                    class="form-control">
                                                <div v-else>
                                                    <label>{{thick[col]}}</label>
                                                </div>
                                            </td>                                   
                                        </tr>
                                    </tbody>
                                </table>
                            </transition>   
                            <div class="table-sm">
                                <table class="table table-edits" v-if="Thicks.SaveResponse.ChangedList.length">
                                    <caption v-if="Thicks.SaveResponse.ChangedList.length">{{ Thicks.SaveResponse.ChangedList.length }} Changes Made</caption>
                                    <thead>
                                    <tr>
                                        <th scope="col" style="width: 25%">Length ID</th>
                                        <th scope="col">Changed Column</th>
                                        <th scope="col">Changed To</th>
                                        <th scope="col">Previous</th>
                                    </tr>
                                    </thead>
                                    <tbody>
                                    <tr v-for="Row in Thicks.SaveResponse.ChangedList">
                                        <td>{{ Row.Key }}</td>
                                        <td>{{ Row.EditedCol }}</td>
                                        <td>{{ Row.EditedVal }}</td>
                                        <td>{{ Row.Previous }}</td>
                                    </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                       
                    </div>
                </div>
                <div v-else-if="Tab === 'Widths'" key="3" class="grades">
                     <div class="above-table mb-3">
                        <div>
                            <input type="button" class="btn-raptor mr-4" value="Add New Width" v-on:click="ShowNewWidth" v-bind:disabled="SecurityEnabled" />
                            <input type="button" class="btn-raptor" value="Save Change(s)" v-on:click="Save(Widths, 'widths')" v-if="Widths.Edited" />
                        </div>                        
                    </div>  
                    
                    <div class="row" @keyup.enter="Widths.Editing = false" tabindex="0" style="outline:none;">
                        <div class="grade-grid">
                             <div class="card grade-card" v-if="Widths.New">
                                <span class="card-title">New Width</span>
                                <div class="product-grid mt-3">
                                    <div>
                                        <label>Label</label>
                                        <input type="text" class="form-control" v-model="Widths.New.Label" />        
                                    </div>
                                    <div>
                                        <label>Nominal</label>
                                        <input type="number" class="form-control" v-model.number="Widths.New.Nominal" />                                            
                                    </div>
                                    <div>
                                        <label>Min</label>
                                        <input type="number" class="form-control" v-model.number="Widths.New.Minimum" />                                            
                                    </div>
                                    <div>
                                        <label>Max</label>
                                        <input type="number" class="form-control" v-model.number="Widths.New.Maximum" />                                            
                                    </div>
                                    <div>
                                        <input type="button" class="btn-raptor" value="Save" v-on:click="AddNew(Widths, 'width')" v-bind:disabled="SecurityEnabled" />
                                        <input type="button" class="btn-raptor" value="Cancel" v-on:click="CancelNew(Widths)" />
                                    </div>
                                </div>
                            </div>
                            <transition name="fade" mode="out-in">
                                <table style="max-width: 600px;" class="table" v-if="Widths.List.length > 0">
                                    <thead>
                                        <tr>
                                            <th style="width:10%;" v-on:click="Sort('ID', Widths)">ID</th>
                                            <th v-on:click="Sort('Label', Widths)">Label</th>
                                            <th style="width:20%;" v-on:click="Sort('Nominal', Widths)">Nominal</th>
                                            <th style="width:20%;" v-on:click="Sort('Minimum', Widths)">Min</th>
                                            <th style="width:20%;" v-on:click="Sort('Maximum', Widths)">Max</th>                                         
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="width in WidthsSorted">
                                            <td>{{width.ID}}</td>
                                            <td @click="EditingCell(Widths, 'Label', width.ID)">                                            
                                                <input
                                                    v-if="Widths.Editing === width.ID + '_Label'"
                                                    v-model="width.Label"
                                                    v-on:blur="Update('Label', width.Label, width, Widths, 'ID');"
                                                    v-on:focus="Prev(width.Label, Widths)"
                                                    type="text"
                                                    spellcheck="false"
                                                    class="form-control">
                                                <div v-else>
                                                    <label>{{width.Label}}</label>
                                                </div>
                                            </td> 
                                            <td @click="EditingCell(Widths, col, width.ID)" v-for="col in Widths.Cols">                                            
                                                <input
                                                    v-if="Widths.Editing === width.ID + '_' + col"
                                                    v-model.number="width[col]"
                                                    v-on:blur="Update(col, width[col], width, Widths, 'ID');"
                                                    v-on:focus="Prev(width[col], Widths)"
                                                    type="number"
                                                    spellcheck="false"
                                                    class="form-control">
                                                <div v-else>
                                                    <label>{{width[col]}}</label>
                                                </div>
                                            </td>                                   
                                        </tr>
                                    </tbody>
                                </table>
                            </transition>   
                            <div class="table-sm">
                                <table class="table table-edits" v-if="Widths.SaveResponse.ChangedList.length">
                                    <caption v-if="Widths.SaveResponse.ChangedList.length">{{ Widths.SaveResponse.ChangedList.length }} Changes Made</caption>
                                    <thead>
                                    <tr>
                                        <th scope="col" style="width: 25%">Length ID</th>
                                        <th scope="col">Changed Column</th>
                                        <th scope="col">Changed To</th>
                                        <th scope="col">Previous</th>
                                    </tr>
                                    </thead>
                                    <tbody>
                                    <tr v-for="Row in Widths.SaveResponse.ChangedList">
                                        <td>{{ Row.Key }}</td>
                                        <td>{{ Row.EditedCol }}</td>
                                        <td>{{ Row.EditedVal }}</td>
                                        <td>{{ Row.Previous }}</td>
                                    </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                       
                    </div>
                </div>
                <div v-else-if="Tab === 'Lengths'" key="4" class="grades">
                    <div class="above-table mb-3">
                        <div>
                            <input type="button" class="btn-raptor mr-4" value="Add New Length" v-on:click="ShowNewLength" v-bind:disabled="SecurityEnabled" />
                            <input type="button" class="btn-raptor" value="Save Change(s)" v-on:click="Save(Lengths, 'lengths')" v-if="Lengths.Edited" />
                        </div>                        
                    </div>  
                    <div class="row" @keyup.enter="Lengths.Editing = false" tabindex="0" style="outline:none;">
                        <div class="grade-grid">
                             <div class="card grade-card" v-if="Lengths.New">
                                <span class="card-title">New Length</span>
                                <div class="product-grid mt-3">
                                    <div>
                                        <label>Label</label>
                                        <input type="text" class="form-control" v-model="Lengths.New.LengthLabel" />
                                    </div>
                                    <div>
                                        <label>Nominal</label>
                                        <input type="number" class="form-control" v-model.number="Lengths.New.LengthNominal" />                                            
                                    </div>
                                    <div>
                                        <label>Min</label>
                                        <input type="number" class="form-control" v-model.number="Lengths.New.LengthMin" />                                            
                                    </div>
                                    <div>
                                        <label>Max</label>
                                        <input type="number" class="form-control" v-model.number="Lengths.New.LengthMax" />                                            
                                    </div>
                                    <div>
                                        <input type="button" class="btn-raptor" value="Save" v-on:click="AddNew(Lengths, 'length')" v-bind:disabled="SecurityEnabled" />
                                        <input type="button" class="btn-raptor" value="Cancel" v-on:click="CancelNew(Lengths)" />
                                    </div>
                                </div>
                            </div>
                            <transition name="fade" mode="out-in">
                                <table style="max-width: 600px;" class="table" v-if="Lengths.List.length > 0">
                                    <thead>
                                        <tr>
                                            <th style="width:12%;" v-on:click="Sort('LengthID', Lengths)">ID</th>
                                            <th style="width:22%;" v-on:click="Sort('LengthLabel', Lengths)">Label</th>
                                            <th style="width:22%;" v-on:click="Sort('LengthNominal', Lengths)">Nominal</th>
                                            <th style="width:22%;" v-on:click="Sort('LengthMin', Lengths)">Min</th>
                                            <th style="width:22%;" v-on:click="Sort('LengthMax', Lengths)">Max</th>                                         
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="length in LengthsSorted">
                                            <td>{{length.LengthID}}</td>
                                            <td @click="EditingCell(Lengths, 'LengthLabel', length.LengthID)">                                            
                                                <input
                                                    v-if="Lengths.Editing === length.LengthID + '_LengthLabel'"
                                                    v-model="length.LengthLabel"
                                                    v-on:blur="Update('LengthLabel', length.LengthLabel, length, Lengths, 'LengthID');"
                                                    v-on:focus="Prev(length.LengthLabel, Lengths)"
                                                    type="text"
                                                    spellcheck="false"
                                                    class="form-control">
                                                <div v-else>
                                                    <label>{{length.LengthLabel}}</label>
                                                </div>
                                            </td>
                                            <td @click="EditingCell(Lengths, col, length.LengthID)" v-for="col in Lengths.Cols">                                            
                                                <input
                                                    v-if="Lengths.Editing === length.LengthID + '_' + col"
                                                    v-model.number="length[col]"
                                                    v-on:blur="Update(col, length[col], length, Lengths, 'LengthID');"
                                                    v-on:focus="Prev(length[col], Lengths)"
                                                    type="number"
                                                    spellcheck="false"
                                                    class="form-control">
                                                <div v-else>
                                                    <label>{{length[col]}}</label>
                                                </div>
                                            </td>                                   
                                        </tr>
                                    </tbody>
                                </table>
                            </transition>   
                            <div class="table-sm">
                                <table class="table table-edits" v-if="Lengths.SaveResponse.ChangedList.length">
                                    <caption v-if="Lengths.SaveResponse.ChangedList.length">{{ Lengths.SaveResponse.ChangedList.length }} Changes Made</caption>
                                    <thead>
                                    <tr>
                                        <th scope="col" style="width: 25%">Length ID</th>
                                        <th scope="col">Changed Column</th>
                                        <th scope="col">Changed To</th>
                                        <th scope="col">Previous</th>
                                    </tr>
                                    </thead>
                                    <tbody>
                                    <tr v-for="Row in Lengths.SaveResponse.ChangedList">
                                        <td>{{ Row.Key }}</td>
                                        <td>{{ Row.EditedCol }}</td>
                                        <td>{{ Row.EditedVal }}</td>
                                        <td>{{ Row.Previous }}</td>
                                    </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                       
                    </div>
                </div>
                <div v-else-if="Tab === 'PETLengths' && false" key="5" class="grades" >
                    <div class="above-table mb-3">
                        <div>
                            <input type="button" class="btn-raptor mr-4" value="Add New PETLength" v-on:click="ShowNewPETLength" v-bind:disabled="SecurityEnabled" />
                            <input type="button" class="btn-raptor" value="Save Change(s)" v-on:click="Save(PETLengths, 'PETLengths')" v-if="PETLengths.Edited" />
                        </div>        
                        <div class="near-saw-grid">
                            <label>Near Saw Offset</label>
                            <input type="number" v-model="NearSawOffSet" min="-0.5" max="0.5" step="0.1" />
                            <input type="button" class="btn-raptor" value="save" v-on:click="SaveNearSaw" />
                        </div>
                    </div>  
                    <div class="row" @keyup.enter="PETLengths.Editing = false" tabindex="0" style="outline:none;">
                        <div class="grade-grid">
                             <div class="card grade-card" v-if="PETLengths.New">
                                <span class="card-title">New PETLength</span>
                                <div class="product-grid mt-3">
                                    <div>
                                        <label>Label</label>
                                        <input type="text" class="form-control" v-model="PETLengths.New.LengthLabel" />
                                    </div>
                                    <div>
                                        <label>Saw Index</label>
                                        <input type="number" class="form-control" v-model.number="PETLengths.New.SawIndex" />                                            
                                    </div>
                                    <div>
                                        <label>Nominal</label>
                                        <input type="number" class="form-control" v-model.number="PETLengths.New.LengthNominal" />                                            
                                    </div>
                                    <div>
                                        <label>PET Position</label>
                                        <input type="number" class="form-control" v-model.number="PETLengths.New.PETPosition" />                                            
                                    </div>
                                    <div>
                                        <input type="button" class="btn-raptor" value="Save" v-on:click="AddNew(PETLengths, 'PETLength')" v-bind:disabled="SecurityEnabled" />
                                        <input type="button" class="btn-raptor" value="Cancel" v-on:click="CancelNew(PETLengths)" />
                                    </div>
                                </div>
                            </div>
                            <transition name="fade" mode="out-in">
                                <table style="max-width: 600px;" class="table" v-if="PETLengths.List.length > 0">
                                    <thead>
                                        <tr>
                                            <th style="width:12%;" v-on:click="Sort('LengthID', Lengths)">ID</th>
                                            <th style="width:22%;" v-on:click="Sort('LengthLabel', Lengths)">Label</th>
                                            <th style="width:22%;" v-on:click="Sort('LengthNominal', Lengths)">Saw Index</th>
                                            <th style="width:22%;" v-on:click="Sort('LengthMin', Lengths)">Nominal</th>
                                            <th style="width:22%;" v-on:click="Sort('LengthMax', Lengths)">PET Position</th>                                         
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="length in PETLengthsSorted">
                                            <td>{{length.PETLengthID}}</td>
                                            <td @click="EditingCell(PETLengths, 'LengthLabel', length.PETLengthID)">                                            
                                                <input
                                                    v-if="PETLengths.Editing === length.PETLengthID + '_LengthLabel'"
                                                    v-model="length.LengthLabel"
                                                    v-on:blur="Update('LengthLabel', length.LengthLabel, length, PETLengths, 'PETLengthID');"
                                                    v-on:focus="Prev(length.LengthLabel, PETLengths)"
                                                    type="text"
                                                    spellcheck="false"
                                                    class="form-control">
                                                <div v-else>
                                                    <label>{{length.LengthLabel}}</label>
                                                </div>
                                            </td>
                                            <td @click="EditingCell(PETLengths, col, length.PETLengthID)" v-for="col in PETLengths.Cols">                                            
                                                <input
                                                    v-if="PETLengths.Editing === length.PETLengthID + '_' + col"
                                                    v-model.number="length[col]"
                                                    v-on:blur="Update(col, length[col], length, PETLengths, 'PETLengthID');"
                                                    v-on:focus="Prev(length[col], PETLengths)"
                                                    type="number"
                                                    spellcheck="false"
                                                    class="form-control">
                                                <div v-else>
                                                    <label>{{length[col]}}</label>
                                                </div>
                                            </td>                                   
                                        </tr>
                                    </tbody>
                                </table>
                            </transition>   
                            <div class="table-sm">
                                <table class="table table-edits" v-if="PETLengths.SaveResponse.ChangedList.length">
                                    <caption v-if="PETLengths.SaveResponse.ChangedList.length">{{ PETLengths.SaveResponse.ChangedList.length }} Changes Made</caption>
                                    <thead>
                                    <tr>
                                        <th scope="col" style="width: 25%">PETLength ID</th>
                                        <th scope="col">Changed Column</th>
                                        <th scope="col">Changed To</th>
                                        <th scope="col">Previous</th>
                                    </tr>
                                    </thead>
                                    <tbody>
                                    <tr v-for="Row in PETLengths.SaveResponse.ChangedList">
                                        <td>{{ Row.Key }}</td>
                                        <td>{{ Row.EditedCol }}</td>
                                        <td>{{ Row.EditedVal }}</td>
                                        <td>{{ Row.Previous }}</td>
                                    </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>                       
                    </div>
                </div>
                <div v-else-if="Tab === 'Graders'" key="5" class="grades" >
                    <div class="above-table mb-3">
                        <div>
                            <input type="button" class="btn-raptor mr-4" value="Add New Grader" v-on:click="ShowNewGrader" v-bind:disabled="SecurityEnabled" />
                            <input type="button" class="btn-raptor" value="Save Change(s)" v-on:click="Save(Graders, 'graders')" v-if="Graders.Edited" />
                        </div>                        
                    </div>  
                    <div class="row" @keyup.enter="Graders.Editing = false" tabindex="0" style="outline:none;">
                        <div class="grade-grid">
                             <div class="card grade-card" v-if="Graders.New">
                                <span class="card-title">New Grader</span>
                                <div class="product-grid mt-3">
                                    <div>
                                        <label>Description</label>
                                        <input type="text" class="form-control" v-model="Graders.New.GraderDescription" />
                                    </div>                                    
                                    <div>
                                        <input type="button" class="btn-raptor" value="Save" v-on:click="AddNew(Graders, 'graders')" v-bind:disabled="SecurityEnabled" />
                                        <input type="button" class="btn-raptor" value="Cancel" v-on:click="CancelNew(Graders)" />
                                    </div>
                                </div>
                            </div>
                            <transition name="fade" mode="out-in">
                                <table style="max-width: 600px;" class="table" v-if="Graders.List.length > 0">
                                    <thead>
                                        <tr>
                                            <th style="width:20%;" v-on:click="Sort('GraderID', Graders)">Grader/Colour ID</th>
                                            <th style="width:80%;" v-on:click="Sort('GraderDescription', Graders)">Grader/Colour Description</th>                                        
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="grader in GradersSorted">
                                            <td>{{grader.GraderID}}</td>
                                            <td @click="EditingCell(Graders, 'GraderDescription', grader.GraderID)">                                            
                                                <input
                                                    v-if="Graders.Editing === grader.GraderID + '_GraderDescription'"
                                                    v-model="grader.GraderDescription"
                                                    v-on:blur="Update('GraderDescription', grader.GraderDescription, grader, Graders, 'GraderID');"
                                                    v-on:focus="Prev(grader.GraderDescription, Graders)"
                                                    type="text"
                                                    spellcheck="false"
                                                    class="form-control">
                                                <div v-else>
                                                    <label>{{grader.GraderDescription}}</label>
                                                </div>
                                            </td>                                                                    
                                        </tr>
                                    </tbody>
                                </table>
                            </transition>   
                            <div class="table-sm">
                                <table class="table table-edits" v-if="Graders.SaveResponse.ChangedList.length">
                                    <caption v-if="Graders.SaveResponse.ChangedList.length">{{ Graders.SaveResponse.ChangedList.length }} Changes Made</caption>
                                    <thead>
                                    <tr>
                                        <th scope="col" style="width: 25%">Grader ID</th>
                                        <th scope="col">Changed Column</th>
                                        <th scope="col">Changed To</th>
                                        <th scope="col">Previous</th>
                                    </tr>
                                    </thead>
                                    <tbody>
                                    <tr v-for="Row in Graders.SaveResponse.ChangedList">
                                        <td>{{ Row.Key }}</td>
                                        <td>{{ Row.EditedCol }}</td>
                                        <td>{{ Row.EditedVal }}</td>
                                        <td>{{ Row.Previous }}</td>
                                    </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>                       
                    </div>
                </div>
            </transition>
        </div>
    </div>    
</asp:Content>
<asp:Content ContentPlaceHolderID="JavaScript" runat="server">
    <script type="text/javascript" src="Scripts/axios/axios.min.js"></script>
    <script type="text/javascript" src="Scripts/vue/vue.js"></script>
    <script type="text/javascript" src="Scripts/Products.js?v= <%= GetVersion() %>"></script>
</asp:Content>