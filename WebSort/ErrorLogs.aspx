<%@ Page Title="" Language="C#" MasterPageFile="~/WebSort.Master" AutoEventWireup="true" CodeBehind="ErrorLogs.aspx.cs" Inherits="WebSort.ErrorLogs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="CSS/ErrorLogs.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="vue">
        <div style="margin-bottom:300px;">
            <div class="row mb-5 d-flex justify-content-between">
                <div class="col-12 col-lg-4">
                    <input type="text" class="form-control" placeholder="Search..." v-model="Search" />
                </div>   
                <div class="col-12 col-lg-4">
                    <input type="button" class="btn-cancel" v-on:click="Flush" value="Flush Errors" />
                </div>   
            </div>
            <div class="card-list">
                <card inline-template v-bind:log="log" v-for="log in Filtered">
                    <article class="card">
                        <div class="card-header">
                          <p>{{log.LoggedString}}</p>
                            <h2 v-if="Title[0]">{{Title[0]}}</h2>
                            <h2 v-if="Title[1]">{{Title[1].split('\n')[0]}}</h2>
                        </div>
                        <div class="stack-trace">   
                            <span v-if="InnerException" v-for="trace in InnerException">{{trace}}<br /></span>
                            <span v-else>{{Exception[1]}}</span>
                        </div>
                    </article>
                </card>
                
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JavaScript" runat="server">
    <script src="Scripts/vue/vue.js"></script>
    <script src="Scripts/axios/axios.min.js"></script>
    <script type="text/javascript">
        Vue.component('card', {
            name: 'Card',
            props: {
                log: { type: Object, required: false },
            },
            data() {
                return {
                    focused: false
                }
            },
            created() {

            },
            computed: {
                Exception: function () {
                    if (this.log && this.log.Exception.includes("--->")) {
                        return this.log.Exception.split("--->")
                    } else {
                        return this.log.Exception
                    }
                },
                Title: function () {
                    if (this.Exception && this.Exception[0].includes(":")) {
                        return this.Exception[0].split(":")
                    } else if (this.Exception.includes(":")) {
                        return this.Exception.split(":")
                    } else {
                        return this.Exception
                    }
                },
                InnerException: function () {
                    if (this.Exception.length > 1 && this.Exception[1].includes("--- End of inner exception stack trace ---")) {
                        return this.Exception[1].split("--- End of inner exception stack trace ---")
                    } else if (!Array.isArray(this.Exception)) {
                        return this.Exception.split("\n")
                    } else {
                        return this.Exception
                    }
                }

            },
            methods: {
                
            }
        });

        const v = new Vue({
            el: '#vue',
            name: 'Alarms',
            data: function () {
                return {
                    Logs: [],

                    Search: '',

                    Headers: {
                        headers: {
                            'Content-Type': 'application/json; charset=UTF-8',
                            'Accept': 'application/json'
                        }
                    }
                }
            },
            mounted: function () {
                this.GetLogs();
            },
            computed: {
                Filtered: function () {
                    if (this.Search && this.Logs.length) {
                        return this.Logs.filter(f =>
                            f.Exception.toLowerCase().includes(this.Search.toLowerCase()) || 
                            f.LoggedString.toLowerCase().includes(this.Search.toLowerCase())
                        )
                    } else {
                        return this.Logs
                    }
                }
            },
            methods: {
                GetLogs: function () {
                    axios.post('ErrorLogs.aspx/GetLogs', this.Headers)
                        .then(response => {
                            this.Logs = JSON.parse(response.data.d);
                        })
                        .catch(error => {
                            console.error(error);
                        });
                },
                Flush: function () {
                    axios.post('ErrorLogs.aspx/Flush', this.Headers)
                        .then(response => {
                            this.GetLogs()
                        })
                        .catch(error => {
                            console.error(error);
                        });
                }
            }
        })
    </script>
</asp:Content>
