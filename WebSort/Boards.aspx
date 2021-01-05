<%@ Page Language="C#" EnableEventValidation="false" ViewStateEncryptionMode="Never" MasterPageFile="~/WebSort.Master" AutoEventWireup="true" CodeBehind="Boards.aspx.cs" Inherits="WebSort.Boards" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Scripts/Chart.js/Chart.min.css" rel="stylesheet" />
    <link href="CSS/Boards.css?v= <%= GetVersion() %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="app">
        <div class="row" v-show="ShowStats">
            <div id="ChartContainer" class="col-6">
                <div class="row">
                    <div class="col-12 col-lg-6 chart">
                        <canvas id="FirstChart"></canvas>
                    </div>
                    <div class="col-12 col-lg-6 chart">
                        <canvas id="SecondChart"></canvas>
                    </div>
                </div>
            </div>
            <div class="col-6 align-items-baseline" style="display: grid; grid-gap: 1rem; grid-template-columns: repeat(3, 1fr);">
                <div>
                    <table class="table table-sm" v-if="prodStats && prodStats.Grades" style="height: 100%;">
                        <thead>
                            <tr>
                                <th>Grade</th>
                                <th>Pieces</th>
                                <th>Volume</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="value in prodStats.Grades">
                                <td>{{value.Grade}}</td>
                                <td>{{value.Pieces}}</td>
                                <td>{{value.Volume}}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div>
                    <table class="table table-sm" v-if="prodStats && prodStats.Lengths" style="height: 100%;">
                        <thead>
                            <tr>
                                <th>Length</th>
                                <th>Pieces</th>
                                <th>Volume</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="value in prodStats.Lengths">
                                <td>{{value.Length}}</td>
                                <td>{{value.Pieces}}</td>
                                <td>{{value.Volume}}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div>
                    <table class="table table-sm" v-if="prodStats && prodStats.Products" style="height: 100%;">
                        <thead>
                            <tr>
                                <th>Product</th>
                                <th>Pieces</th>
                                <th>Volume</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="value in prodStats.Products">
                                <td>{{value.Product}}</td>
                                <td>{{value.Pieces}}</td>
                                <td>{{value.Volume}}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <div class="row mb-3 mt-3">
            <div class="col-12">
                <input type="button" class="btn-raptor" v-bind:value="BoardsIntervalStatus" v-on:click="ToggleBoardsInterval" />
                <input type="button" class="btn-raptor" v-bind:value="StatsStatus" v-on:click="ToggleStats" />
                <input type="button" class="btn-raptor" v-bind:value="RejectsStatus" v-on:click="ToggleRejects" />
            </div>
        </div>
        <div class="row mb-3" v-if="ShowRejects">
            <div class="col-12">
                <div style="display:grid; grid-gap:1rem; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr))">
                    <div v-for="reject in Rejects.slice(1)" class="d-flex align-items-center justify-content-between">
                        <span>{{ reject.RejectFlag }}</span>
                        <input type="text"
                            class="form-control"
                            v-bind:style="{ color: reject.Colour }"
                            style="font-weight: bold;"
                            v-model="reject.RejectDescription"
                            v-on:input="SaveRejects(reject)" />
                        <input type="color"
                            class="form-control p-0"
                            list="colour-list"
                            v-model="reject.Colour"
                            v-on:input="SaveRejects(reject)" />
                        <datalist id="colour-list">
                            <option v-for="Colour in Colours">{{ Colour }}</option>
                        </datalist>
                    </div>
                </div>                
            </div>
        </div>
        <div class="row">
            <div class="col-12 d-flex align-items-center justify-content-center">
                <table class="table" v-if="Rejects" style="max-width:1600px;">
                    <thead>
                        <tr>
                            <th>TimeStamp</th>
                            <th>Bay</th>
                            <th>Reason</th>
                            <th v-for="header in colHeaders">{{ header.text }}</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="board in boards">
                            <td>{{board.TimeString}}</td>
                            <td>{{board.BinLabel}}</td>
                            <td v-bind:style="{ color: Rejects[board.Flags + 1].Colour }">{{ Rejects[board.Flags + 1].RejectDescription }}</td>
                            <td v-for="header in colHeaders">{{ board[header.source] }}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>    
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="JavaScript">
    <script src="Scripts/Chart.js/Chart.min.js"></script>
    <script src="Scripts/chartjs-plugin-annotation/chartjs-plugin-annotation.min.js"></script>
    <script src="Scripts/vue/vue.js"></script>
    <script src="Scripts/Boards.js?v= <%= GetVersion() %>"></script>
</asp:Content>