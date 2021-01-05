<%@ Page Title="" Language="C#" MasterPageFile="~/WebSort.Master" AutoEventWireup="true" CodeBehind="Audits.aspx.cs" Inherits="WebSort.Audits" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .card-list {
            display: grid;
            grid-gap: 2rem;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
        }

        .card {
            height: 300px;
            min-width: 300px;
            padding: 1.5rem;
            border-radius: 16px;
            background: var(--bg-secondary);
            box-shadow: -1rem 0 3rem var(--shadow);
            display: flex;
            flex-direction: column;
            transition: .1s;
            margin: 0;
            scroll-snap-align: start;
            clear: both;
            position: relative;
            overflow-y: auto;
        }
            .card::-webkit-scrollbar {
                width: 0.5rem;
                height: 0.5rem;
            }

            .card::-webkit-scrollbar-track {
                background: #1e1e24;
            }

            .card::-webkit-scrollbar-thumb {
                background: var(--raptor-light);
            }

            .card:focus-within ~ .card, .card:hover ~ .card {
                transform: translateY(250px);
            }

        .card {
            margin-bottom: -250px;
        }


        .card-header {
            margin-bottom: auto;
            background-color: var(--bg-secondary);
            text-align:center;
        }

            .card-header p {
                font-size: 14px;
                margin: 0 0 1rem;
                color: var(--text-primary);
            }

            .card-header h2 {
                font-size: 1.25rem;
                margin: .25rem 0 auto;
                text-decoration: none;
                color: var(--text-primary);
        
                border: 0;
                display: inline-block;
                cursor: pointer;
            }

                .card-header h2:hover {
                    background: linear-gradient(90deg,#ff8a00,#e52e71);
                    text-shadow: none;
                    -webkit-text-fill-color: transparent;
                    -webkit-background-clip: text;
                    background-clip: text;
                }
        .stack-trace {
            font-size: 0.9rem;
            display: grid;
            grid-gap: 1rem;
            grid-template-rows: auto;
            margin-top:2rem;
        }
    </style>
    

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="vue">
        <div style="margin-bottom:300px;">
            <div class="row mb-5 d-flex justify-content-between">
                <div class="col-12 col-lg-4">
                    <input type="text" class="form-control" placeholder="Search..." v-model="Search" />
                </div>    
            </div>
            <div class="card-list">
                <card inline-template v-bind:audit="audit" v-for="audit in Filtered">
                    <article class="card">
                        <div class="card-header">
                            <p>{{audit.TimeStampString}}</p>
                            <h2 v-if="!audit.Deleted">{{audit.TableName}} - {{audit.Col}}</h2>
                            <h2 v-if="audit.Deleted">{{audit.TableName}} - Deleted</h2>
                        </div>
                        <div class="stack-trace" v-if="!audit.Deleted && !audit.Added">  
                            <span><strong>User Name: </strong>{{audit.UserName}}</span>
                            <span><strong>{{audit.KeyName ? audit.KeyName : 'Key'}}:</strong> {{audit.TableKey}}</span>
                            <span><strong>Value: </strong>{{audit.Val}}</span>
                            <span><strong>Previous: </strong>{{audit.Prev}}</span>
                        </div>
                        <div class="stack-trace" v-if="audit.Deleted">  
                            <span>User Name: {{audit.UserName}}</span>
                            <span>{{audit.KeyName ? audit.KeyName : 'Key'}}: {{audit.TableKey}}</span>
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
                audit: { type: Object, required: false },
            },
            data() {
                return {
                    focused: false
                }
            },
            created() {

            },
            computed: {

            },
            methods: {
                
            }
        });

        const v = new Vue({
            el: '#vue',
            name: 'Alarms',
            data: function () {
                return {
                    Audit: [],

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
                this.GetAudit();
            },
            computed: {
                Filtered: function () {
                    if (this.Search && this.Audit.length) {
                        return this.Audit.filter(f =>
                            f.Col.toLowerCase().includes(this.Search.toLowerCase()) || 
                            f.Val.toLowerCase().includes(this.Search.toLowerCase()) ||
                            f.Prev.toLowerCase().includes(this.Search.toLowerCase()) ||
                            f.UserName.toLowerCase().includes(this.Search.toLowerCase()) ||
                            f.TableName.toLowerCase().includes(this.Search.toLowerCase())
                        )
                    } else {
                        return this.Audit
                    }
                }
            },
            methods: {
                GetAudit: function () {
                    axios.post('Audits.aspx/GetAudit', this.Headers)
                        .then(response => {
                            this.Audit = JSON.parse(response.data.d);
                        })
                        .catch(error => {
                            console.error(error);
                        });
                }                
            }
        })
    </script>
</asp:Content>
