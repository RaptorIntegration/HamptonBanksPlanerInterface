<%@ Page Title="" Language="C#" MasterPageFile="~/WebSort.Master" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" ViewStateEncryptionMode="Never" AutoEventWireup="true" CodeBehind="WebSort.aspx.cs" Inherits="WebSort.WebSortPage" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=10.5.3700.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Assembly="EO.Web" Namespace="EO.Web" TagPrefix="eo" %>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="head">
    <link href="CSS/WebSort.css?v= <%= GetVersion() %>" rel="stylesheet" />
    <link href="Scripts/Chart.js/Chart.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="vue-wrapper">
        <div class="row">
            <div class="col-12">
                <div class="d-flex justify-content-between tab">
                    <div>
                        <input type="button" id="edit-btn" class="tablinks active" onclick="openView(event, 'Edit')" value="Edit View" />
                        <input type="button" id="sort-btn" class="tablinks" onclick="openView(event, 'Sort')" value="Sort View" />
                    </div>

                    <div>
                        <div class="d-flex" id="ContainerLabel">
                            <div class="d-flex" id="SpareLabel">
                                <span>Spare</span>
                                <div></div>
                            </div>
                            <div class="d-flex" id="ActiveLabel">
                                <span>Active</span>
                                <div></div>
                            </div>
                            <div class="d-flex" id="FullLabel">
                                <span>Full</span>
                                <div></div>
                            </div>
                            <div class="d-flex" id="DisabledLabel">
                                <span>Disabled</span>
                                <div></div>
                            </div>
                            <div class="d-flex" id="RejectLabel">
                                <span>Reject</span>
                                <div></div>
                            </div>
                            <div class="d-flex" id="VirtualLabel">
                                <span>Virtual</span>
                                <div></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-12">
                <div id="Edit" class="tabcontent" style="display: block;">
                    <div class="row justify-content-between" style="min-height: 20rem;">
                        <div class="col-9">
                            <div id="ContainerChart" class="chart">
                                <canvas id="BinChart" style="height: 20rem; width: 300px"></canvas>
                            </div>
                        </div>
                        <div class="col-3">
                            <div id="ContainerPie" class="chart">
                                <canvas id="PieChart" style="height: 20rem; width: 300px"></canvas>
                            </div>
                        </div>
                    </div>

                    <div id="vue-content" @keyup.enter="Editing=false" tabindex="0">
                        <div class="row colour-row col-12 mb-3">
                            <div>
                                <input type="button" v-bind:value="ShowColoursValue" v-on:click="ToggleColours" class="btn-raptor btn-colours" />
                            </div>
                            <div v-for="val in StatusColours" v-if="ShowColours">
                                <label v-bind:style="{ color: val.colour }">
                                    {{val.status}}
                                </label>
                                <input type="color"
                                    class="colour-box"
                                    list="colour-list"
                                    v-model="val.colour"
                                    v-on:input="SaveColours()" />
                                <datalist id="colour-list">
                                    <option v-for="Colour in Colours">{{ Colour }}</option>
                                </datalist>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-2">
                                <transition name="component-fade" mode="out-in">
                                    <div v-if="Edited">
                                        <input class="btn-save" type="button" v-on:click="Save" value="Save" />
                                        <input class="btn-cancel" type="button" v-on:click="Cancel" value="Cancel" />
                                    </div>
                                </transition>
                            </div>
                            <div class="col-2">
                                <transition name="component-fade" mode="out-in">
                                    <div id="overlay" v-if="Loading">
                                        <div class="lds-ellipsis">
                                            <div></div>
                                            <div></div>
                                            <div></div>
                                            <div></div>
                                        </div>
                                    </div>
                                    <span v-if="SaveResponse" class="save-msg">{{SaveResponse.Message}}</span>
                                </transition>
                            </div>
                            <div class="col-5 table-sm scroll1">
                                <transition name="component-fade" mode="out-in">
                                    <table class="table table-edits" v-if="SaveResponse.ChangedList">
                                        <caption v-if="SaveResponse.ChangedList.length">{{ SaveResponse.ChangedList.length }} Changes Made</caption>
                                        <thead>
                                            <tr>
                                                <th scope="col" style="width: 15%">Bay</th>
                                                <th scope="col">Changed Column</th>
                                                <th scope="col">Changed To</th>
                                                <th scope="col">Previous</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr v-for="Row in SaveResponse.ChangedList">
                                                <td>{{Row.Key}}</td>
                                                <td>{{Row.EditedCol}}</td>
                                                <td>{{Row.EditedVal}}</td>
                                                <td>{{Row.Previous}}</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </transition>
                            </div>
                            <div class="col-1"></div>
                            <div class="col-12 col-lg-2 col-md-12 col-sm-12" v-if="DropDown">
                                <auto-complete :data="DropDown" v-model="Filter"></auto-complete>
                            </div>
                            <div class="col-12 d-flex justify-content-center">
                                <table class="table">
                                    <thead>
                                        <tr>
                                            <th scope="col" style="width: 2%;" @click="Sort('BinID')">Bay</th>
                                            <th scope="col" style="width: 20%;" @click="Sort('BinLabel')">Label</th>
                                            <th scope="col" style="width: 5%;" @click="Sort('BinStatus')">Label</th>
                                            <th scope="col" v-for="Col in Columns" style="width: 2%;" @click="Sort(Col.DataSource)">{{Col.Header}}</th>
                                            <th scope="col" style="width: 2%;" @click="Sort('BinPercent')">Full</th>
                                            <th scope="col" style="width: 2%;" @click="Sort('SortID')">Sort ID</th>
                                            <th scope="col" @click="Sort('SecProd')">Secondary Product</th>
                                            <th scope="col" style="width:2%;" @click="Sort('SecSize')">Secondary Size %</th>
                                            <th scope="col" style="width: 20%" @click="Sort('BinStamps')">Stamps</th>
                                            <th scope="col" @click="Sort('ProductsLabel')">Products</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <template v-for="Row in FilterBays">
                                            <tr>
                                                <th scope="row">{{Row.BinID}}</th>
                                                <td @click="EditingCell(Row, 'BinLabel')" style="white-space:nowrap;">
                                                    <input
                                                        v-if="Editing==Row.BinID + '_BinLabel'" v-model="Row.BinLabel"
                                                        v-on:blur="Update('BinLabel', Row.BinLabel, Row);"
                                                        v-on:focus="Prev(Row, 'BinLabel')"
                                                        type="text"
                                                        spellcheck="false"
                                                        class="form-control">
                                                    <div v-else>
                                                        <label>{{Row.BinLabel}}</label>
                                                    </div>
                                                </td>
                                                <td @click="EditingCell(Row, 'BinStatus')">
                                                    <select
                                                        v-if="Editing == Row.BinID + '_BinStatus'"
                                                        v-model.number="Row.BinStatus"
                                                        v-on:change="Update('BinStatus', Row.BinStatus, Row);"
                                                        v-on:focus="Prev(Row, 'BinStatus')">
                                                        <option v-for="(status, index) in StatusList" v-bind:value="index">{{ status }}</option>
                                                    </select>
                                                    <div v-else>
                                                        <label>{{StatusList[Row.BinStatus]}}</label>
                                                    </div>
                                                </td>
                                                <td v-for="Col in Columns" @click="EditingCell(Row, Col.DataSource)">
                                                    <input
                                                        v-if="Editing==Row.BinID + '_' + Col.DataSource && Col.DataSource != 'BinStatusLabel'"
                                                        v-model="Row[Col.DataSource]"
                                                        v-on:blur="Update(Col.DataSource, Row[Col.DataSource], Row);"
                                                        v-on:focus="Prev(Row, Col.DataSource)"
                                                        type="text"
                                                        spellcheck="false"
                                                        class="form-control">
                                                    <div v-if="Editing != Row.BinID + '_' + Col.DataSource">
                                                        <label>{{Row[Col.DataSource]}}</label>
                                                    </div>
                                                </td>
                                                <td>
                                                    {{ Row.BinPercent }}%
                                                </td>
                                                <td>
                                                    {{ Row.SortID }}
                                                </td>
                                                <td @click="EditingCell(Row, 'SecProdID')">
                                                    <select class="form-control"
                                                        v-model.number="Row.SecProdID"
                                                        v-if="Editing == Row.BinID + '_SecProdID'"
                                                        v-on:blur="Editing = false"
                                                        v-on:change="Update('SecProdID', Row.SecProdID, Row)">
                                                        <option value="0">None</option>
                                                        <option v-for="prod in ProductGrades" v-bind:value="prod.ID">{{prod.Label}}</option>
                                                    </select>
                                                    <div v-else>
                                                        <label>{{ Row.SecProdID == 0 ? 'None' : ProductGrades.find(f => f.ID == Row.SecProdID).Label }}</label>
                                                    </div>
                                                </td>
                                                <td @click="EditingCell(Row, 'SecSize')">
                                                    <input
                                                        v-if="Editing == Row.BinID + '_SecSize'"
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
                                                 <td @click="EditingCell(Row, 'BinStamps')">
                                                    <div class="stamp-container">
                                                        <div v-for="stamp in Row.SelectedStamps" class="stamp-inner-container">
                                                            <input
                                                                v-bind:id="'stamp-' + Row.SortID + '-' + stamp.ID"
                                                                v-on:change="Update('BinStamps', stamp.Selected, Row);"
                                                                v-model="stamp.Selected"
                                                                type="checkbox"
                                                                class="check">
                                                            <label v-bind:for="'stamp-' + Row.SortID + '-' + stamp.ID">{{stamp.Description}}</label>
                                                        </div>
                                                    </div>
                                                </td>
                                                <td @click="EditingCell(Row, 'ProductsLabel'); ">
                                                    <div>
                                                        <label>{{Row.ProductsLabel}}</label>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr class="no-hover" v-if="Editing === Row.BinID + '_ProductsLabel'">
                                                <td colspan="12">
                                                    <div class="row" style="padding:5px;" v-on:click="Editing = null">
                                                        <div class="col-10">
                                                            <span style="font-size:14px; font-weight:700;">Products</span>
                                                        </div>
                                                        <div class="col-1">
                                                            <span style="font-size:14px; font-weight:700;">Lengths</span>
                                                        </div>
                                                    </div>
                                                    <div class="row" v-if="Row.ProdLen">
                                                        <div class="col-10 product-grid">
                                                            <div class="checkboxlist" v-for="Prod in Row.ProdLen.ProductsList" v-bind:key="Row.BinID + '_' + Prod.ID">
                                                                <input type="checkbox" class="check" v-bind:id="'prod_' + Prod.Label" v-model="Prod.Selected" @change="ProductsChange(Prod, Row)" />
                                                                <label :for="'prod_' + Prod.Label">{{Prod.Label}}</label>
                                                            </div>
                                                        </div>
                                                        <div class="col-1 flex-col">
                                                            <div class="checkboxlist" v-for="Length in Row.ProdLen.LengthsList" v-bind:key="Length.ID">
                                                                <input type="checkbox" class="check" v-bind:id="'len_' + Length.Label" v-model="Length.Selected" @change="ProductsChange(Length, Row)" />
                                                                <label :for="'len_' + Length.Label">{{Length.Label}}</label>
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
                <div id="Sort" class="tabcontent">
                    <div class="row">
                        <div class="sort-filter-grid col-12 justify-content-between" v-if="DropDown">
                            <div class="d-flex">
                                <select class="form-control" v-model="SortBy">
                                    <option>Sort By...</option>
                                    <option>BinLabel</option>
                                    <option>BinStatus</option>
                                    <option>BinSize</option>
                                    <option>BinCount</option>
                                    <option>BinPercent</option>
                                </select>
                                <button class="btn-clear" type="button" v-on:click="SortByAsc = !SortByAsc">
                                    <i class="gg-arrows-v-alt"></i>
                                </button>
                            </div>
                            <div class="incriment-container">
                                <label>Increment/Decrement By:</label>
                                <input class="incriment-btn" type="button" value="-"  v-on:click="DecreaseInc" />
                                <input class="incriment-amount" type="number" v-model.number="IncDec" />
                                <input class="incriment-btn" type="button" value="+" v-on:click="IncreaseInc" />
                            </div>
                            <auto-complete :data="DropDown" v-model="Filter"></auto-complete>
                        </div>
                    </div>
                    <div class="sorts-container">
                        <div
                            class="sort card"
                            v-for="(bay, index) in FilteredSorter"
                            v-if="Bays.length > 0"
                            v-bind:style="{'border-color': StatusColours[bay.BinStatus].colour}">
                            <sorter inline-template
                                v-on:editing="CancelAutoUpdate"
                                v-on:resume="SetAutoUpdate"
                                v-if="FilteredSorter[index]"
                                v-model="FilteredSorter[index]"
                                v-bind:header="Headers"
                                v-bind:colours="StatusColours"
                                v-bind:incdec="IncDec">
                                <div>
                                    <div class="card-title d-flex justify-content-between" v-on:click="ToggleEdit">
                                        <span>#{{value.BinID}} - {{value.BinLabel}}</span>
                                        <span>count: {{value.BinCount}}, size: {{value.BinSize}}</span>
                                    </div>
                                    <div class="d-flex justify-content-between" v-on:click="ToggleEdit">
                                        <div v-bind:style="Bar"></div>
                                        <span>{{value.BinPercent}}%</span>
                                    </div>
                                    <div class="pt-5" v-if="Editing">
                                        <div class="switch-field" v-bind:style="{'background-color': Background}">
		                                    <input type="radio" v-bind:id="RadioIDs[0]" value="0" v-model.number="value.BinStatus" />
		                                    <label v-bind:for="RadioIDs[0]">Spare</label>
		                                    <input type="radio" v-bind:id="RadioIDs[1]" value="1" v-model.number="value.BinStatus" />
		                                    <label v-bind:for="RadioIDs[1]">Active</label>
		                                    <input type="radio" v-bind:id="RadioIDs[2]" value="2" v-model.number="value.BinStatus" />
		                                    <label v-bind:for="RadioIDs[2]">Full</label>
                                            <input type="radio" v-bind:id="RadioIDs[3]" value="3" v-model.number="value.BinStatus" />
		                                    <label v-bind:for="RadioIDs[3]">Disabled</label>
                                            <input type="radio" v-bind:id="RadioIDs[4]" value="4" v-model.number="value.BinStatus" />
		                                    <label v-bind:for="RadioIDs[4]">Reject</label>
	                                    </div>
                                    </div>
                                    <div class="pt-5 d-flex align-items-center" v-if="Editing">
                                        <span class="editing-label">Count</span>
                                        <input type="button" class="btn-arrow" value="-" v-on:click="Decrease('BinCount')" />
                                        <span class="editing-value">{{value.BinCount}}</span>
                                        <input type="button" class="btn-arrow" value="+" v-on:click="Increase('BinCount')" />
                                    </div>
                                    <div class="pt-5 d-flex align-items-center" v-if="Editing">
                                        <span class="editing-label">Size</span>
                                        <input type="button" class="btn-arrow" value="-" v-on:click="Decrease('BinSize')" />
                                        <span class="editing-value">{{value.BinSize}}</span>
                                        <input type="button" class="btn-arrow" value="+" v-on:click="Increase('BinSize')" />
                                    </div>
                                    <div class="pt-3" v-if="Editing" style="font-size:1.25rem;">
                                        <input type="button" class="btn-raptor" value="Save" v-on:click="Save" />
                                    </div>
                                </div>
                            </sorter>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div>

        <CR:CrystalReportSource ID="CrystalReportSource1" runat="server">
            <Report FileName="C:\DeltaC V200\Configuration.rpt"></Report>
        </CR:CrystalReportSource>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="JavaScript" ID="SimJs" runat="server">
    <script type="text/javascript" src="Scripts/axios/axios.min.js"></script>
    <script type="text/javascript" src="Scripts/Chart.js/Chart.min.js"></script>
    <script type="text/javascript" src="Scripts/vue/vue.js"></script>
    <script type="text/javascript" src="Scripts/WebSort.js?v= <%= GetVersion() %>"></script>
</asp:Content>