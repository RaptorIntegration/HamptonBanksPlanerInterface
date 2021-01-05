<%@ Page Language="C#" MasterPageFile="~/WebSort.Master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="Alarms.aspx.cs" Inherits="WebSort.Alarms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="CSS/Alarms.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="vue">
        <div>
            <transition name="fade">
                <div class="toast" v-bind:class="ToastClass" v-if="Toast.Message">{{ Toast.Message }}</div>
            </transition>
            <div class="tab-grid tab mb-5">
                <input type="button" class="tablinks" v-bind:class="{active: Tab == 0}" value="Current Alarms" v-on:click="Tab = 0" />
                <input type="button" class="tablinks" v-bind:class="{active: Tab == 1}" value="Alarms History" v-on:click="Tab = 1" />
                <input type="button" class="tablinks" v-bind:class="{active: Tab == 2}" value="Alarm Properties" v-on:click="Tab = 2" />
                <input type="button" class="tablinks" v-bind:class="{active: Tab == 3}" value="Default Messages" v-on:click="Tab = 3" />
                <input type="button" class="tablinks" v-bind:class="{active: Tab == 4}" value="Display Board Settings " v-on:click="Tab = 4" />
                <input type="button" class="tablinks" v-bind:class="{active: Tab == 5}" value="Display Log" v-on:click="Tab = 5" />
            </div>
            <div>
                <transition name="fade" mode="out-in">
                    <div v-if="Tab === 0" key="0" class="d-flex justify-content-center">
                        <table v-if="Settings.List.length" class="table" style="max-width:1200px;">
                            <thead>
                                <tr>
                                    <th style="width:5%;">ID</th>
                                    <th>Text</th>
                                    <th style="width:30%;">Start Time</th>
                                    <th style="width:5%;">Severity</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="alarm in CurrentAlarms.List">
                                    <td>{{alarm.AlarmID}}</td>
                                    <td>{{Settings.List[alarm.AlarmID].AlarmText}}</td>
                                    <td>{{alarm.StartTimeString}}</td>
                                    <td>                                        
                                        <div v-if="Settings.List[alarm.AlarmID].Severity === 1">
                                            <svg class="table-icon warning-text" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 21v-4m0 0V5a2 2 0 012-2h6.5l1 1H21l-3 6 3 6h-8.5l-1-1H5a2 2 0 00-2 2zm9-13.5V9"></path></svg>
                                        </div>
                                        <div v-else-if="Settings.List[alarm.AlarmID].Severity === 2">
                                            <svg class="table-icon error-text" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"></path></svg>
                                        </div>
                                        <div v-else>
                                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div v-else-if="Tab === 1" key="1" class="d-flex justify-content-center">
                        <table v-if="Settings.List.length" class="table" style="max-width:1400px;">
                            <thead>
                                <tr>
                                    <th style="width:5%;" v-on:click="Sort('AlarmID', History)">ID</th>
                                    <th>Text</th>
                                    <th style="width:20%;" v-on:click="Sort('StartTime', History)">Start Time</th>
                                    <th style="width:20%;" v-on:click="Sort('StopTime', History)">Stop Time</th>
                                    <th style="width:5%;" v-on:click="Sort('Downtime', History)">Downtime</th>
                                    <th style="width:5%;" v-on:click="Sort('Duration', History)">Duration</th>
                                    <th style="width:5%;">Severity</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="alarm in HistorySorted">
                                    <td>{{alarm.AlarmID}}</td>
                                    <td>{{Settings.List[alarm.AlarmID].AlarmText}}</td>
                                    <td>{{alarm.StartTimeString}}</td>
                                    <td>{{alarm.StopTimeString}}</td>
                                    <td>{{alarm.Downtime ? 'Yes' : 'No'}}</td>
                                    <td>{{alarm.Duration}}</td>
                                    <td>
                                        <div v-if="Settings.List[alarm.AlarmID].Severity === 1">
                                            <svg class="table-icon warning-text" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 21v-4m0 0V5a2 2 0 012-2h6.5l1 1H21l-3 6 3 6h-8.5l-1-1H5a2 2 0 00-2 2zm9-13.5V9"></path></svg>
                                        </div>
                                        <div v-if="Settings.List[alarm.AlarmID].Severity === 2">
                                            <svg class="table-icon error-text" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"></path></svg>
                                        </div>
                                        <div v-else>
                                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div v-else-if="Tab === 2" key="2">
                        <nav class="mb-2">
			                <ul class="pagination text-center">
				                <li class="page-item">
					                <a class="btn-raptor" v-on:click="PrevPage">
						                <span>Previous</span>
					                </a>
				                </li>

                                <li class="page-item" style="width:25px;" v-if="Settings.CurrentPage < 2"></li>

				                <li class="page-item" v-bind:class="{active: 0 == Settings.CurrentPage}">
					                <a class="btn-raptor btn-sm round" v-on:click="Settings.CurrentPage = 0">1</a>
				                </li>
				                <li class="page-item ellipsis" v-if="Settings.CurrentPage > 1"></li>

				                <li class="page-item"
                                    v-for="page in SettingsNumPages" 
                                    :key="page" 
                                    v-if="SettingsNumPages > 4 && ((page != 1 && page != SettingsNumPages) && ((Settings.CurrentPage + 2) == page || Settings.CurrentPage == page || (Settings.CurrentPage + 1) == page))" 
                                    v-bind:class="{active: (page - 1) == Settings.CurrentPage}">
					                <a class="btn-raptor btn-sm round"
					                   v-on:click="Settings.CurrentPage = (page - 1)"
					                   v-if="(page != 1 && page != SettingsNumPages) && ((Settings.CurrentPage + 2) == page || Settings.CurrentPage == page || (Settings.CurrentPage + 1) == page)">
						                {{ page }}
					                </a>
				                </li>

				                <li class="page-item ellipsis" v-if="(SettingsNumPages - 1) - Settings.CurrentPage > 1"></li>

				                <li class="btn-raptor btn-sm round" v-bind:class="{active: (SettingsNumPages - 1) == Settings.CurrentPage}" v-if="SettingsNumPages > 1">
					                <a v-on:click="Settings.CurrentPage = (SettingsNumPages - 1)">{{ SettingsNumPages }}</a>
				                </li>

                                <li class="page-item" style="width:25px;" v-if="Settings.CurrentPage < 2 && Settings.CurrentPage != 1"></li>

				                <li class="page-item">
					                <a class="btn-raptor" v-on:click="NextPage">
						                <span>Next</span>
					                </a>
				                </li>
			                </ul>
		                </nav>
                        <div class="above-table mb-3">
                            <div>
                                <input type="button" class="btn-save" value="Save Change(s)" v-on:click="SaveSettings()" v-if="Settings.Edited" />
                            </div>
                            <div>
                                <div class="table-sm" v-if="Settings.SaveResponse.ChangedList.length">
                                    <transition name="fade" mode="out-in">
                                        <table class="table table-edits" v-if="Settings.SaveResponse.ChangedList">
                                            <caption v-if="Settings.SaveResponse.ChangedList.length">{{ Settings.SaveResponse.ChangedList.length }} Changes Made</caption>
                                            <thead>
                                            <tr>
                                                <th scope="col" style="width: 25%">Alarm ID</th>
                                                <th scope="col">Changed Column</th>
                                                <th scope="col">Changed To</th>
                                                <th scope="col">Previous</th>
                                            </tr>
                                            </thead>
                                            <tbody>
                                            <tr v-for="Row in Settings.SaveResponse.ChangedList">
                                                <td>{{ Row.Key }}</td>
                                                <td>{{ Row.EditedCol }}</td>
                                                <td>{{ Row.EditedVal }}</td>
                                                <td>{{ Row.Previous }}</td>
                                            </tr>
                                            </tbody>
                                        </table>
                                    </transition>
                                </div>
                            </div>
                            <div>
                                <input
                                    placeholder="Filter..."
                                    class="form-control"
                                    type="text"
                                    v-model="Settings.Filter"
                                    @focus="Settings.Focused = true; Settings.Filter = ''"
                                    @blur="Settings.Focused = false"
                                    spellcheck="false">
   	                            <div style="max-height:18px;">
                                    <div v-if="Settings.Focused" class="selection-container">
                                        <div class="selection-list" v-for="help in Settings.FilterHelp" @mousedown="Settings.Filter = help">{{ help }}</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="d-flex justify-content-center">
                            <table v-if="Settings.List.length" class="table" style="max-width:1400px;">
                                <thead>
                                    <tr>
                                        <th style="width:5%;" v-on:click="Sort('AlarmID', Settings)">ID</th>
                                        <th style="width:5%;" v-on:click="Sort('Active', Settings)">Active</th>
                                        <th style="width:5%;" v-on:click="Sort('Priority', Settings)">Priority</th>
                                        <th v-on:click="Sort('AlarmText', Settings)">Text</th>
                                        <th v-on:click="Sort('DisplayText', Settings)">Display Text</th>
                                        <th style="width:5%;" v-on:click="Sort('Severity', Settings)">Severity</th>
                                        <th style="width:5%;" v-on:click="Sort('Downtime', Settings)">Downtime Eligible</th>
                                        <th style="width:5%;" v-on:click="Sort('DataRequired', Settings)">Data Required</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="alarm in SettingsPaged">
                                        <td>{{alarm.AlarmID}}</td>
                                        <td>
                                            <input
                                                v-model="alarm.Active"
                                                v-on:change="Update('Active', alarm.Active, alarm, Settings, 'AlarmID');"
                                                type="checkbox"
                                                class="check">
                                        </td>
                                        <td @click="EditingCell(Settings, 'Priority', alarm.AlarmID)">
                                            <input
                                                v-if="Settings.Editing == alarm.AlarmID + '_Priority'"
                                                v-model="alarm.Priority"
                                                v-on:blur="Update('Priority', alarm.Priority, alarm, Settings, 'AlarmID');"
                                                v-on:focus="Prev(alarm.Priority, Settings)"
                                                type="number" min="0"
                                                spellcheck="false"
                                                class="form-control">
                                            <div v-else>
                                                <label>{{alarm.Priority}}</label>
                                            </div>
                                        </td>
                                        <td @click="EditingCell(Settings, 'AlarmText', alarm.AlarmID)">
                                            <input
                                                v-if="Settings.Editing == alarm.AlarmID + '_AlarmText'"
                                                v-model="alarm.AlarmText"
                                                v-on:blur="Update('AlarmText', alarm.AlarmText, alarm, Settings, 'AlarmID');"
                                                v-on:focus="Prev(alarm.AlarmText, Settings)"
                                                type="text"
                                                spellcheck="false"
                                                class="form-control">
                                            <div v-else>
                                                <label>{{alarm.AlarmText}}</label>
                                            </div>
                                        </td>
                                        <td @click="EditingCell(Settings, 'DisplayText', alarm.AlarmID)">
                                            <input
                                                v-if="Settings.Editing == alarm.AlarmID + '_DisplayText'"
                                                v-model="alarm.DisplayText"
                                                v-on:blur="Update('DisplayText', alarm.DisplayText, alarm, Settings, 'AlarmID');"
                                                v-on:focus="Prev(alarm.DisplayText, Settings)"
                                                type="text"
                                                spellcheck="false"
                                                class="form-control">
                                            <div v-else>
                                                <label>{{alarm.DisplayText}}</label>
                                            </div>
                                        </td>
                                        <td @click="EditingCell(Settings, 'Severity', alarm.AlarmID)">
                                            <select
                                                class="form-control"
                                                v-model="alarm.Severity"
                                                v-if="Settings.Editing == alarm.AlarmID + '_Severity'"
                                                v-on:change="Update('Severity', Severity[alarm.Severity], alarm, Settings, 'AlarmID');"
                                                v-on:focus="Prev(Severity[alarm.Severity], Settings)">
                                                <option v-for="(s, index) in Severity" v-bind:value="index">{{s}}</option>
                                            </select>
                                            <div v-else>
                                                <span>{{Severity[alarm.Severity]}}</span>
                                            </div>
                                        </td>
                                        <td>
                                            <input
                                                v-model="alarm.Downtime"
                                                v-on:change="Update('Downtime', alarm.Downtime, alarm, Settings, 'AlarmID');"
                                                type="checkbox"
                                                class="check">
                                        </td>
                                        <td>
                                            <input
                                                v-model="alarm.DataRequired"
                                                v-on:change="Update('DataRequired', alarm.DataRequired, alarm, Settings, 'AlarmID');"
                                                type="checkbox"
                                                class="check">
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div v-else-if="Tab === 3" key="3">
                        <div class="d-flex justify-content-center">
                            <span>Default Messages marked as active below will be displayed when all other alarms have cleared.</span>
                        </div>

                        <div class="above-table mb-3 mt-3">
                            <div>
                                <input type="button" class="btn-save" value="Save Change(s)" v-on:click="SaveDefaults()" v-if="Defaults.Edited" />
                            </div>
                            <div>
                                <div class="table-sm" v-if="Defaults.SaveResponse.ChangedList.length">
                                    <transition name="fade" mode="out-in">
                                        <table class="table table-edits" v-if="Defaults.SaveResponse.ChangedList">
                                            <caption v-if="Defaults.SaveResponse.ChangedList.length">{{ Defaults.SaveResponse.ChangedList.length }} Changes Made</caption>
                                            <thead>
                                            <tr>
                                                <th scope="col" style="width: 25%">Alarm ID</th>
                                                <th scope="col">Changed Column</th>
                                                <th scope="col">Changed To</th>
                                                <th scope="col">Previous</th>
                                            </tr>
                                            </thead>
                                            <tbody>
                                            <tr v-for="Row in Defaults.SaveResponse.ChangedList">
                                                <td>{{ Row.Key }}</td>
                                                <td>{{ Row.EditedCol }}</td>
                                                <td>{{ Row.EditedVal }}</td>
                                                <td>{{ Row.Previous }}</td>
                                            </tr>
                                            </tbody>
                                        </table>
                                    </transition>
                                </div>
                            </div>
                            <label style="min-height:34px;">
                                Use multiple display rows where possible
                                <input type="checkbox" v-model="Defaults.MultipleRows" />
                            </label>
                        </div>
                        <div class="d-flex justify-content-center">
                            <table v-if="Defaults.List.length" class="table" style="max-width:1000px;">
                                <thead>
                                    <tr>
                                        <th style="width:5%;" v-on:click="Sort('AlarmID', Defaults)">ID</th>
                                        <th style="width:5%;" v-on:click="Sort('Active', Defaults)">Active</th>
                                        <th v-on:click="Sort('Category', Defaults)">Category</th>
                                        <th v-on:click="Sort('Prefix', Defaults)">Main Prefix</th>
                                        <th v-on:click="Sort('InfomasterPrefix', Defaults)">Infomaster Prefix</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="d in Defaults.List">
                                        <td>{{d.AlarmID}}</td>
                                        <td>
                                            <input
                                                v-model="d.Active"
                                                v-on:change="Update('Active', d.Active, d, Defaults, 'AlarmID');"
                                                type="checkbox"
                                                class="check">
                                        </td>
                                        <td @click="EditingCell(Defaults, 'Category', d.AlarmID)">
                                            <input
                                                v-if="Defaults.Editing == d.AlarmID + '_Category'"
                                                v-model="d.Category"
                                                v-on:blur="Update('Category', d.Category, d, Defaults, 'AlarmID');"
                                                v-on:focus="Prev(d.Category, Defaults)"
                                                type="text"
                                                spellcheck="false"
                                                class="form-control">
                                            <div v-else>
                                                <label>{{d.Category}}</label>
                                            </div>
                                        </td>
                                        <td @click="EditingCell(Defaults, 'Prefix', d.AlarmID)">
                                            <input
                                                v-if="Defaults.Editing == d.AlarmID + '_Prefix'"
                                                v-model="d.Prefix"
                                                v-on:blur="Update('Prefix', d.Prefix, d, Defaults, 'AlarmID');"
                                                v-on:focus="Prev(d.Prefix, Defaults)"
                                                type="text"
                                                spellcheck="false"
                                                class="form-control">
                                            <div v-else>
                                                <label>{{d.Prefix}}</label>
                                            </div>
                                        </td>
                                        <td @click="EditingCell(Defaults, 'InfomasterPrefix', d.AlarmID)">
                                            <input
                                                v-if="Defaults.Editing == d.AlarmID + '_InfomasterPrefix'"
                                                v-model="d.InfomasterPrefix"
                                                v-on:blur="Update('InfomasterPrefix', d.InfomasterPrefix, d, Defaults, 'AlarmID');"
                                                v-on:focus="Prev(d.InfomasterPrefix, Defaults)"
                                                type="text"
                                                spellcheck="false"
                                                class="form-control">
                                            <div v-else>
                                                <label>{{d.InfomasterPrefix}}</label>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div v-else-if="Tab === 4" key="4" class="general-grid" style="max-width:1300px; margin:auto;">
                        <div v-if="General">
                            <label>Display Time Main (sec)</label>
                            <input type="text" v-model="General.DisplayTime" class="form-control mb-3" v-on:change="SaveGeneral" />
                            <label>Display Time ViewMarq (sec)</label>
                            <input type="text" v-model="General.DisplayTime1" class="form-control mb-3" v-on:change="SaveGeneral" />
                            <label>Display Time Infomaster (sec)</label>
                            <input type="text" v-model="General.DisplayTime2" class="form-control mb-3" v-on:change="SaveGeneral" />
                        </div>
                        <div v-if="General">
                            <label>Blank Time Between Messages Main (sec)</label>
                            <input type="text" v-model="General.BlankTime" class="form-control mb-3" v-on:change="SaveGeneral" />
                            <label>Blank Time Between Messages ViewMarq (sec)</label>
                            <input type="text" v-model="General.BlankTime1" class="form-control mb-3" v-on:change="SaveGeneral" />
                            <label>Blank Time Between Messages InfoMaster (sec)</label>
                            <input type="text" v-model="General.BlankTime2" class="form-control mb-3" v-on:change="SaveGeneral" />
                        </div>
                        <div v-if="General">
                            <label>Display Main Port #</label>
                            <input type="text" v-model="General.DisplayPortNumber" class="form-control mb-3" v-on:change="SaveGeneral" />
                            <label>Display ViewMarq Port #</label>
                            <input type="text" v-model="General.DisplayPortNumber" class="form-control mb-3" v-on:change="SaveGeneral" />
                            <label>Display Infomaster Port #</label>
                            <input type="text" v-model="General.DisplayPortNumber" class="form-control mb-3" v-on:change="SaveGeneral" />
                        </div>
                        <div v-if="General">
                            <label>Display Main IP Address</label>
                            <input type="text" v-model="General.DisplayIPAddress" class="form-control" />
                        </div>
                    </div>
                    <div v-else-if="Tab === 5" key="5" class="display-grid">
                        <div>
                            <div class="card d-flex justify-content-between">
                                <span class="card-title">Raptor Display Main Service: {{Services[0]}}</span>
                                <span></span>
                                <div>
                                    <input type="button" class="btn-save" value="Start" v-on:click="StartService('RaptorDisplay')" />
                                    <input type="button" class="btn-cancel" value="Stop" v-on:click="StopService('RaptorDisplay')" />
                                </div>
                            </div>
                            <div class="card d-flex justify-content-between mt-5">
                                <span class="card-title">Raptor Display ViewMarq Service: {{Services[1]}}</span>
                                <div>
                                    <input type="button" class="btn-save" value="Start" v-on:click="StartService('RaptorDisplayViewMarq')" />
                                    <input type="button" class="btn-cancel" value="Stop" v-on:click="StopService('RaptorDisplayViewMarq')" />
                                </div>
                            </div>
                            <div class="card d-flex justify-content-between mt-5">
                                <span class="card-title">Raptor Display Infomaster Service: {{Services[2]}}</span>
                                <div>
                                    <input type="button" class="btn-save" value="Start" v-on:click="StartService('RaptorDisplayInfomaster')" />
                                    <input type="button" class="btn-cancel" value="Stop" v-on:click="StopService('RaptorDisplayInfomaster')" />
                                </div>
                            </div>
                        </div>
                        <div>
                            <div class="d-flex justify-content-center">
                                <input type="text" class="form-control" v-model="DisplayLog.Filter" placeholder="Filter..." style="max-width:1000px;" />
                            </div>
                            <div class="d-flex justify-content-center">
                                <table v-if="DisplayLog.List.length" class="table" style="max-width:1000px;">
                                    <thead>
                                        <tr>
                                            <th style="width:20%">Time Stamp</th>
                                            <th>Message</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="log in LogsFiltered">
                                            <td>{{log.TimeString}}</td>
                                            <td style="text-align:left;">{{log.Text}}</td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
            </div>
            </transition>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="JavaScript">
    <script src="Scripts/axios/axios.min.js"></script>
    <script src="Scripts/vue/vue.js"></script>
    <script src="Scripts/Alarms.js"></script>
</asp:Content>