const v = new Vue({
    el: '#vue',
    name: 'Alarms',
    data: function () {
        return {
            Tab: 0,

            CurrentAlarms: {
                Timer: null,
                List: [],
            },

            History: {
                Timer: null,
                List: [],
                SortByAsc: false,
                SortBy: '',
                Editing: null,
                Edited: false,
                SaveResponse: {
                    Message: '',
                    ChangedList: []
                },
            },

            Settings: {
                ItemsPerPage: 50,
                CurrentPage: 0,
                List: [],
                SortByAsc: false,
                SortBy: '',
                Filter: '',
                FilterHelp: ['Active', 'In-Active', 'Information', 'Warning', 'Critical'],
                Focused: false,
                Editing: null,
                Edited: false,
                Previous: null,
                SaveResponse: {
                    Message: '',
                    ChangedList: []
                },
            },

            Reasons: {
                Primary: {
                    List: [],
                    Editing: null,
                    Edited: false,
                    Previous: null,
                    SaveResponse: {
                        Message: '',
                        ChangedList: []
                    },
                },
                Secondary: {
                    List: [],
                    Editing: null,
                    Edited: false,
                    Previous: null,
                    SaveResponse: {
                        Message: '',
                        ChangedList: []
                    },
                }
            },

            Defaults: {
                MultipleRows: false,
                List: [],
                Editing: null,
                Edited: false,
                Previous: null,
                SaveResponse: {
                    Message: '',
                    ChangedList: []
                },
            },

            General: {
                "DisplayTime": "",
                "BlankTime": 0,
                "DisplayIPAddress": "",
                "DisplayPortNumber": "",
                "MultilineDefaults": false,
                "DisplayType": 0,
                "DisplayCount": 0,
                "DisplayTimeVorneScroll": 0,
                "DisplayTimeVorneStatic": 0,
                "DisplayTime1": 0,
                "BlankTime1": 0.,
                "DisplayPortNumber1": "",
                "DisplayTime2": 0,
                "BlankTime2": 0,
                "DisplayPortNumber2": "",
                "AccidentDateString": "",
                "DisplayPortNumber3": ""
            },

            DisplayLog: {
                Timer: null,
                List: [],
                Filter: ''
            },

            Services: [],

            Severity: ['Information', 'Warning', 'Critical'],

            Toast: {
                Timeout: null,
                Success: false,
                Message: null
            },

            Security: 0,

            Headers: {
                headers: {
                    'Content-Type': 'application/json; charset=UTF-8',
                    'Accept': 'application/json'
                }
            }
        }
    },
    mounted: function () {
        setTimeout(_ => this.GetAlarmSettings(), 0);
        setTimeout(_ => this.GetCurrentAlarms(), 100)
        setTimeout(_ => this.GetHistory(), 200)
        setTimeout(_ => this.GetPrimaryReasons(), 250)
        setTimeout(_ => this.GetSecondaryReasons(), 275)
        setTimeout(_ => this.GetDefaults(), 300)
        setTimeout(_ => this.GetGeneral(), 350)
        setTimeout(_ => this.GetDisplayLog(), 500)
        setTimeout(_ => this.GetSecurity(), 550)

        setInterval(this.GetServices, 3000);

        this.SetCurrentTimer();
    },
    watch: {
        Tab: function (val) {
            if (val == 0) {
                this.SetCurrentTimer();
            } else if (val == 1) {
                this.SetHistoryTimer()
            } else if (val == 5) {
                this.SetDisplayLogTimer()
            }
        },
        'Defaults.MultipleRows': function (val) {
            this.SaveMultiRow();
        },
    },
    computed: {
        ToastClass: function () {
            return {
                'toast-success': this.Toast.Success,
                'toast-error': !this.Toast.Success
            }
        },
        SecurityEnabled: function () {
            return this.Security != 1
        },

        HistorySorted: function () {
            if (this.History.List.length) {
                let SortDir = this.History.SortByAsc ? 'asc' : 'desc';

                if (this.History.SortBy) {
                    return [...this.History.List].sort(this.CompareValues(this.History.SortBy, SortDir));
                } else {
                    return this.History.List
                }
            } else {
                return null
            }
        },

        SettingsSorted: function () {
            if (this.Settings.List.length) {
                let SortDir = this.Settings.SortByAsc ? 'asc' : 'desc';

                if (this.Settings.SortBy) {
                    return [...this.Settings.List].sort(this.CompareValues(this.Settings.SortBy, SortDir));
                } else {
                    return this.Settings.List
                }
            } else {
                return null
            }
        },
        SettingsFiltered: function () {
            if (this.SettingsSorted && this.Settings.Filter) {
                return this.SettingsSorted.filter(f =>
                    f.AlarmText.toLowerCase().includes(this.Settings.Filter.toLowerCase()) ||
                    (this.Settings.Filter === 'Active' && f.Active) ||
                    (this.Settings.Filter === 'In-Active' && !f.Active) ||
                    (this.Settings.Filter === 'Information' && f.Severity === 0) ||
                    (this.Settings.Filter === 'Warning' && f.Severity === 1) ||
                    (this.Settings.Filter === 'Critical' && f.Severity === 2)
                )
            } else {
                return this.SettingsSorted
            }
        },
        SettingsPaged: function () {
            if (this.SettingsFiltered && this.SettingsFiltered.length > 0) {
                let start = this.Settings.CurrentPage * this.Settings.ItemsPerPage
                let end = start + this.Settings.ItemsPerPage
                return this.SettingsFiltered.slice(start, end)
            } else {
                return null
            }
        },
        SettingsNumPages: function () {
            if (this.SettingsFiltered) {
                return Math.ceil(this.SettingsFiltered.length / this.Settings.ItemsPerPage)
            } else {
                return null
            }
        },

        LogsFiltered: function () {
            if (this.DisplayLog.List.length) {
                return this.DisplayLog.List.filter(f =>
                    f.Text.toLowerCase().includes(this.DisplayLog.Filter.toLowerCase())
                )
            } else {
                return null
            }
        }
    },
    methods: {
        GetSecurity: function () {
            axios.post('Alarms.aspx/GetSecurity', this.Headers)
                .then(response => {
                    this.Security = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetAlarms: function () {
            axios.post('Alarms.aspx/GetAlarms', this.Headers)
                .then(response => {
                    this.Alarms.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetAlarmSettings: function () {
            axios.post('Alarms.aspx/GetAlarmSettings', this.Headers)
                .then(response => {
                    this.Settings.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetCurrentAlarms: function () {
            axios.post('Alarms.aspx/GetCurrentAlarms', this.Headers)
                .then(response => {
                    this.CurrentAlarms.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetHistory: function () {
            axios.post('Alarms.aspx/GetAlarmHistory', this.Headers)
                .then(response => {
                    let d = JSON.parse(response.data.d);
                    if (d) {
                        d.map(h => {
                            h.StartTime = new Date(Number(h.StartTime.replace("/Date(", "").replace(")/", "")))
                            h.StopTime = new Date(Number(h.StopTime.replace("/Date(", "").replace(")/", "")))
                        })
                    }
                    this.History.List = d
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetDefaults: function () {
            axios.post('Alarms.aspx/GetDefaults', this.Headers)
                .then(response => {
                    this.Defaults.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetGeneral: function () {
            axios.post('Alarms.aspx/GetGeneral', this.Headers)
                .then(response => {
                    this.General = JSON.parse(response.data.d)[0];
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetDisplayLog: function () {
            axios.post('Alarms.aspx/GetDisplayLog', this.Headers)
                .then(response => {
                    this.DisplayLog.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetServices: function () {
            axios.post('Alarms.aspx/GetServices', this.Headers)
                .then(response => {
                    this.Services = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetPrimaryReasons: function () {
            axios.post('Alarms.aspx/GetPrimaryReasons', this.Headers)
                .then(response => {
                    this.Reasons.Primary.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },
        GetSecondaryReasons: function () {
            axios.post('Alarms.aspx/GetSecondaryReasons', this.Headers)
                .then(response => {
                    this.Reasons.Secondary.List = JSON.parse(response.data.d);
                })
                .catch(error => {
                    console.error(error);
                });
        },

        Update: function (col, val, row, table, key) {
            if (this.SecurityEnabled) { return; }
            table.Edited = true
            if (typeof (val) === 'boolean') {
                table.Previous = !val;
            }

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
        },
        EditingCell: function (table, col, key) {
            if (this.SecurityEnabled) { return; }
            table.Editing = `${key}_${col}`;
        },
        Prev: function (val, table) {
            table.Previous = val;
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
        Sort: function (Col, table) {
            if (table.Edited) { return; }
            if (table.SortBy === Col) {
                table.SortByAsc = !table.SortByAsc
            } else {
                table.SortByAsc = true
            }
            table.SortBy = Col;
        },

        SaveSettings: function () {
            let changed = this.Settings.List.filter(p => p.EditsList.length)
            if (!changed || this.SecurityEnabled) { return; }

            let data = JSON.stringify({ settings: changed })

            axios.post("Alarms.aspx/SaveSettings", data, this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);

                    this.Settings.SaveResponse = Parsed;
                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.error(error);
                    this.Toast(JSON.parse(error.data.d));
                })
                .finally(_ => {
                    this.GetAlarmSettings()
                    this.Loading = false;
                    this.Settings.Edited = false;
                    this.Settings.Editing = false;
                });
        },

        NextPage: function () {
            if (this.Settings.CurrentPage < (this.SettingsNumPages - 1)) {
                this.Settings.CurrentPage++
            }
        },
        PrevPage: function () {
            if (this.Settings.CurrentPage > 0) {
                this.Settings.CurrentPage--
            }
        },

        SaveDefaults: function () {
            let changed = this.Defaults.List.filter(p => p.EditsList.length)
            if (!changed) { return; }

            let data = JSON.stringify({ defaults: changed })

            axios.post("Alarms.aspx/SaveDefaults", data, this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);

                    this.Defaults.SaveResponse = Parsed;
                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.error(error);
                    this.Toast(JSON.parse(error.data.d));
                })
                .finally(_ => {
                    this.GetDefaults()
                    this.Loading = false;
                    this.Defaults.Edited = false;
                    this.Defaults.Editing = false;
                });
        },
        SaveMultiRow: function () {
            axios.post("Alarms.aspx/SaveMultiRow", JSON.stringify({ MultipleRows: this.Defaults.MultipleRows }), this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);

                    this.Defaults.SaveResponse = Parsed;
                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.error(error);
                    this.Toast(JSON.parse(error.data.d));
                })
        },
        SaveGeneral: function () {
            axios.post("Alarms.aspx/SaveGeneral", JSON.stringify({ general: this.General }), this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);
                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.error(error);
                    this.Toast(JSON.parse(error.data.d));
                })
        },
        SaveReasons: function () {
            let primaryChanged = this.Reasons.Primary.List.filter(f => f.EditsList.length)
            let secondaryChanged = this.Reasons.Secondary.List.filter(f => f.EditsList.length)

            if (primaryChanged && primaryChanged.length) {
                let data = JSON.stringify({ reasons: primaryChanged })

                axios.post("Alarms.aspx/SavePrimaryReasons", data, this.Headers)
                    .then(response => {
                        let Parsed = JSON.parse(response.data.d);

                        this.Reasons.Primary.SaveResponse = Parsed;
                        this.SetToast(Parsed);
                    })
                    .catch(error => {
                        console.error(error);
                        this.Toast(JSON.parse(error.data.d));
                    })
                    .finally(_ => {
                        this.GetPrimaryReasons();
                        this.Loading = false;
                        this.Reasons.Primary.Edited = false;
                        this.Reasons.Primary.Editing = false;
                    });
            }

            if (secondaryChanged?.length) {
                let data = JSON.stringify({ reasons: secondaryChanged })

                axios.post("Alarms.aspx/SaveSecondaryReasons", data, this.Headers)
                    .then(response => {
                        let Parsed = JSON.parse(response.data.d);

                        this.Reasons.Primary.SaveResponse = Parsed;
                        this.SetToast(Parsed);
                    })
                    .catch(error => {
                        console.error(error);
                        this.Toast(JSON.parse(error.data.d));
                    })
                    .finally(_ => {
                        this.GetSecondaryReasons();
                        this.Loading = false;
                        this.Reasons.Secondary.Edited = false;
                        this.Reasons.Secondary.Editing = false;
                    });
            }
        },
        SaveHistory: function () {
            let changed = this.History.List.filter(p => p.EditsList.length)
            if (!changed || this.SecurityEnabled) { return; }

            let data = JSON.stringify({ history: changed })

            axios.post("Alarms.aspx/SaveHistory", data, this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);

                    this.History.SaveResponse = Parsed;
                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.error(error);
                    this.Toast(JSON.parse(error.data.d));
                })
                .finally(_ => {
                    this.SetHistoryTimer()
                    this.Loading = false;
                    this.History.Edited = false;
                    this.History.Editing = false;
                });
        },

        DeletePrimaryReason: function (reason) {
            let data = JSON.stringify({reason: reason})

            axios.post("Alarms.aspx/DeletePrimaryReason", data, this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);

                    this.Reasons.Primary.SaveResponse = Parsed;
                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.error(error);
                    this.Toast(JSON.parse(error.data.d));
                })
                .finally(_ => {
                    this.GetPrimaryReasons();
                });
        },
        DeleteSecondaryReason: function (reason) {
            let data = JSON.stringify({reason: reason})

            axios.post("Alarms.aspx/DeleteSecondaryReason", data, this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);

                    this.Reasons.Secondary.SaveResponse = Parsed;
                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.error(error);
                    this.Toast(JSON.parse(error.data.d));
                })
                .finally(_ => {
                    this.GetSecondaryReasons();
                });
        },

        AddPrimaryReason: function () {
             axios.post("Alarms.aspx/AddPrimaryReason", this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);

                    this.Reasons.Primary.SaveResponse = Parsed;
                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.error(error);
                    this.Toast(JSON.parse(error.data.d));
                })
                .finally(_ => {
                    this.GetPrimaryReasons();
                });
        },
        AddSecondaryReason: function () {
             axios.post("Alarms.aspx/AddSecondaryReason", this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);

                    this.Reasons.Secondary.SaveResponse = Parsed;
                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.error(error);
                    this.Toast(JSON.parse(error.data.d));
                })
                .finally(_ => {
                    this.GetSecondaryReasons();
                });
        },

        StartService: function (service) {
            axios.post("Alarms.aspx/StartService", JSON.stringify({ 'service': service }), this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);
                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.error(error);
                    this.Toast(JSON.parse(error.data.d));
                })
        },
        StopService: function (service) {
            axios.post("Alarms.aspx/StopService", JSON.stringify({ 'service': service }), this.Headers)
                .then(response => {
                    let Parsed = JSON.parse(response.data.d);
                    this.SetToast(Parsed);
                })
                .catch(error => {
                    console.error(error);
                    this.Toast(JSON.parse(error.data.d));
                })
        },

        CancelAutoUpdate: function (Timer) {
            clearInterval(this.CurrentAlarms.Timer);
            this.CurrentAlarms.Timer = null;

            clearInterval(this.History.Timer);
            this.History.Timer = null;
        },
        SetCurrentTimer: function () {
            this.CancelAutoUpdate();

            this.GetCurrentAlarms();
            this.CurrentAlarms.Timer = setInterval(this.GetCurrentAlarms, 1000 * 3);
        },
        SetHistoryTimer: function () {
            this.CancelAutoUpdate();
            this.GetHistory();
            this.History.Timer = setInterval(this.GetHistory, 1000 * 3);
        },
        SetDisplayLogTimer: function () {
            this.CancelAutoUpdate();
            this.GetDisplayLog();
            this.DisplayLog.Timer = setInterval(this.GetDisplayLog, 1000 * 3);
        }
    }
});