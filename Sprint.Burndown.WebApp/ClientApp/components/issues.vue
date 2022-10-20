<template>
    <div class="component-content">
        <div class="ui vertical stripe segment" v-show="loadingInProgress" style="z-index:3">
            <div class="ui middle aligned stackable grid container">
                <div class="row" style="min-height: 85vh;">
                    <div class="ui active inverted dimmer">
                        <div class="ui text loader">Loading...</div>
                        <div class="ui blue progress" v-show="loadingProgressBar" :data-percent="loadingPercents" style="margin: 20% 20% 0 20%; height: 8px; min-width:20vw;" id="loadingProgress">
                            <div class="bar" style="height: 8px;"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <message v-show="message.hasErrors" v-bind:title="message.title" v-bind:details="message.details"></message>

        <modal v-bind:title="'Update Issues'" v-bind:text="'Update the issues in this Sprint?'" v-bind:issuesCount="recentlyUpdatedIssues" ref="modalDialog" />

        <div id="reportContent" v-bind:class="{ 'grid-collapsed': loadingInProgress }">

            <issues-toolbar />

            <issues-search />

            <breadcrumb />

            <div class="ui grid" style="width: 100%;" v-show="hasSprintTimeline">
                <div class="five wide column">
                    <sprint-summary />
                </div>
                <div class="eleven wide last column">
                    <sprint-gantt />
                </div>
            </div>

            <div class="ui grid" v-show="hasTaskStatusSummary">
                <issues-view-switch class="ui left floated most-left-aligned" />

                <issues-collapse-subtasks class="ui right floated" />

                <issues-phases class="ui right floated" />
            </div>

            <div v-show="hasTaskStatusSummary">

                <task-status-summary v-show="hasIndicators" />

                <div class="ui grid" v-show="hasEstimates">
                    <div class="five wide column">
                        <scope-estimates ref="scopeEstimates" />
                    </div>

                    <div class="eleven wide column">
                        <plan-estimates ref="planEstimates" />
                    </div>
                </div>

                <issues-versions v-show="hasVersions" />

                <issues-reporters v-show="hasReporters" />
            </div>

            <div class="ui secondary pointing menu" style="width: 100%;" v-show="hasIssuesGrid">
                <a class="item" v-bind:class="{ active: isDevelopment }" v-on:click="showDevelopmentTasks()">
                    Development
                    <div class="ui gray horizontal label" v-show="devIssues.length > 0">{{devIssues.length}}</div>
                </a>
                <a class="item" v-bind:class="{ active: isQA }" v-on:click="showQaTasks()">
                    QA
                    <div class="ui gray horizontal label" v-show="qaIssues.length > 0">{{qaIssues.length}}</div>
                </a>
                <a class="item" v-bind:class="{ active: isOutstanding }" v-on:click="showOutstandingTasks()">
                    Outstanding
                    <div class="ui red horizontal label" v-show="problemIssues.length > 0">{{problemIssues.length}}</div>
                </a>
                <a class="item" v-bind:class="{ active: isUnplanned}" v-on:click="showUnplannedTasks()">
                    Unplanned
                    <div class="ui teal horizontal label" v-show="unplannedIssues.length > 0">{{unplannedIssues.length}}</div>
                </a>
            </div>

            <issues-grid gridName="issuesGrid" gridSize="large" v-bind:gridData="devGridData" ref="issuesGrid" v-show="hasIssuesGrid" />

            <div class="ui link card" style="padding: 0px; margin-bottom: 16px; width:49%; display: inline-block; margin-right: 8px;" v-show="hasBurnDownCharts">
                <div id="devBurnDown" style="height: 45vh; width: 95%; overflow: hidden; margin: 8px;"></div>
            </div>

            <div class="ui link card" style="padding: 0px; margin-bottom: 16px; width:50%; display: inline-block;" v-show="hasBurnDownCharts">
                <div id="crBurnDown" style="height: 45vh; width: 95%; overflow: hidden; margin: 8px;"></div>
            </div>

            <div class="ui dividing header" v-show="false">
                Testing Phase
                <a class="anchor" id="content"></a>
                <div class="ui right floated" style="float: right; margin-top: -8px; cursor: pointer; color: #005bb3; font-weight: normal">
                    <div id="testingPeriod" class="ui breadcrumb">
                        <span style="border-bottom: dotted 1px #005bb3; ">{{sprintDates.testingBeginDate}} - {{sprintDates.testingEndDate}} </span>
                    </div>
                </div>
            </div>

            <div class="ui segment" style="padding: 0px; margin-bottom: 16px; width:49%; display: inline-block; margin-right: 8px;" v-show="hasBurnDownCharts">
                <div id="testingBurnDown" style="height: 45vh; width: 95%; overflow: hidden; margin: 8px;"></div>
            </div>

            <!--Can't be hidden component for a while-->
            <div class="ui segment" style="padding: 0px; margin-bottom: 16px; width:260px; height: 150px; display: inline-block; margin-right: 8px;">
                <div id="devMiniChart" style="height: 90%; width: 90%; overflow: hidden; margin: 8px;"></div>
            </div>
        </div>
    </div>
</template>
<script>
    import router from '../router'
    import { mapGetters, mapActions } from 'vuex';

    import issuesService from '../issuesService'
    import estimateService from '../estimateService'
    import calendarService from '../calendarService'
    import logger from '../logger'

    import { GET_PREFERENCES, GET_SPRINT_ISSUES, REFRESH_GANTT } from '../store/actions/viewState'
    import { setTimeout } from 'core-js';

    export default {
        data: function () {
            return {
                loadingInProgress: true,
                loadingProgressBar: false,
                loadingPercents: 0,
                gridMode: 'Development',
                recentlyUpdatedInterval: null,
                devIssues: [],
                qaIssues: [],
                problemIssues: [],
                unplannedIssues: [],
                devEstimates: {},
                qaEstimates: {},
                problemEstimates: {},
                unplannedEstimates: {},
                connection: null,
                message: {},
                devChartData: {},
                crChartData: {},
                testingChartData: {}
            };
        },

        computed: {
            isDevelopment: function () {
                return this.gridMode == 'Development';
            },

            isQA: function () {
                return this.gridMode == 'QA';
            },

            isOutstanding: function () {
                return this.gridMode == 'Outstanding';
            },

            isUnplanned: function () {
                return this.gridMode == 'Unplanned';
            },

            issues: function () {
                switch (this.gridMode) {
                    case 'Development': return this.devIssues;
                    case 'QA': return this.qaIssues;
                    case 'Outstanding': return this.problemIssues;
                    case 'Unplanned': return this.unplannedIssues;
                }
            },

            estimates: function () {
                return this.isDevelopment ? this.devEstimates : this.qaEstimates;
            },

            devGridData: function () {
                return {
                    rows: this.devIssues,
                    totals: this.devEstimates
                };
            },

            qaGridData: function () {
                return {
                    rows: this.qaIssues,
                    totals: this.qaEstimates
                };
            },

            problemGridData: function () {
                return {
                    rows: this.problemIssues,
                    totals: this.problemEstimates
                };
            },

            unplannedGridData: function () {
                return {
                    rows: this.unplannedIssues,
                    totals: this.unplannedEstimates
                };
            },

            sprintId() {
                return this.$store.getters.getSprintSummary.sprintId;

            },

            sprintName() {
                return this.$store.getters.getSprintSummary.sprintName;
            },

            sprintDates() {
                return this.$store.getters.getSprintSummary;
            },

            ...mapGetters([
                'hasIndicators',
                'hasEstimates',
                'hasFeatures',
                'hasVersions',
                'hasReporters',

                'hasFilters',
                'hasNoFilters',

                'hasSprintTimeline',
                'hasTaskStatusSummary',
                'hasIssuesGrid',
                'hasBurnDownCharts',

                'recentlyUpdatedInProgress',
                'recentlyUpdatedIssues',
                'reloadIssuesCount',

                'searchText',
                'collapsedSubtasks',
                'collapsedBugs',
                'currentFilterByStatus'
            ])
        },

        watch: {
            $route: function (val) {
                this.refreshData();
            },

            recentlyUpdatedIssues: function (val) {
                $('.ui.label.counter')
                    .transition({
                        animation: 'drop',
                        duration: '350ms'
                    });
            },

            reloadIssuesCount: function () {
                const self = this;

                if (self.recentlyUpdatedInProgress) {
                    return;
                }

                this.$refs.modalDialog
                    .show()
                    .then((params) => {
                        const partialUpdate = !params.updateAllIssues;
                        self.refreshData(true, partialUpdate);
                    })
                    .catch(() => { });
            },

            hasNoFilters: function (newValue) {
                if (newValue) {
                    this.updateIssueFilter()
                }
            },

            searchText: function () {
                this.updateIssueFilter();
            },

            collapsedSubtasks: function () {
                this.updateIssueFilter();
            },

            currentFilterByStatus() {
                this.updateIssueFilter();
            }
        },

        created() {
            this.connection = new this.$signalR.HubConnectionBuilder()
                .withUrl("/notification")
                .configureLogging(this.$signalR.LogLevel.Error)
                .build();
        },

        updated() {
        },

        mounted() {
            const self = this;

            self.configureSignalR();
            self.configureCalendars();

            $('.ui.dropdown').dropdown({ action: 'hide' });

            /*
             * Periodically ping 'recentlyUpdated' issues
             * Current interval: 3 min.
             */
            self.recentlyUpdatedInterval = setInterval(function () {
                self.refreshRecentlyUpdated(self.sprintId);
            }, 180000);

            this.refreshData();
        },

        beforeDestroy() {
            if (this.recentlyUpdatedInterval != null) {
                clearInterval(this.recentlyUpdatedInterval);
            }
        },

        methods: {
            ...mapActions([
                'refreshRecentlyUpdated',
                'refreshIssues'
            ]),

            configureSignalR() {
                const self = this;

                self.connection
                    .start()
                    .catch(err => console.error(err.toString()))
                    .then(function () {
                        console.info('connection has been started');
                    });

                self.connection.on('started', function (totalItems, sprintId) {
                    if (sprintId != self.sprintId) {
                        console.info('loading -> start refused. sprintId = ', sprintId);
                        console.info('loading -> start refused. self.sprintId = ', self.sprintId);
                        return;
                    }

                    logger.loading('loading -> started (items): ', totalItems)
                    logger.loading('loading -> started (sprint): ', sprintId)

                    self.loadingPercents = 0;
                    self.loadingInProgress = true;
                    self.loadingProgressBar = true;
                    $('#loadingProgress').progress({ percent: 0 });
                });

                self.connection.on('processed', function (currentItem, totalItems, sprintId) {
                    if (sprintId != self.sprintId) {
                        return;
                    }

                    currentItem = currentItem > totalItems ? totalItems : currentItem;
                    self.loadingPercents = totalItems <= 0 ? 0 : Math.round(currentItem / totalItems * 100);
                    $('#loadingProgress').progress({ percent: self.loadingPercents });
                    console.info('loading -> processed: ', self.loadingPercents);
                });

                self.connection.on('finished', function (sprintId) {
                    if (sprintId != self.sprintId) {
                        return;
                    }

                    console.info('loading -> finished (sprint)', sprintId);
                    self.loadingInProgress = false;
                    self.loadingProgressBar = false;
                    self.refreshData();
                });
            },

            configureCalendars() {
                const self = this;

                var eventsFunc = function (date) {
                    const day = date.getDay();
                    const isWeekend = (day === 6) || (day === 0);

                    const currentTime = date.getTime();
                    const withTime = (element) => element.getTime() == currentTime;

                    const beginTime = moment(self.developBeginDate, 'D/MMM/YYYY').toDate().getTime();
                    const endTime = moment(self.developEndDate, 'D/MMM/YYYY').toDate().getTime();

                    let dayCss = webix.Date.isHoliday(date);

                    if ((currentTime >= beginTime) && (currentTime <= endTime)) {
                        dayCss += ' webix_cal_event_workday'
                    }

                    if (currentTime == beginTime) {
                        dayCss += ' webix_cal_event_sprint_start'
                    }

                    if (currentTime == endTime) {
                        dayCss += ' webix_cal_event_sprint_end'
                    }

                    return dayCss;
                }
            },

            // TODO replace with VUEX usage
            refreshEstimatesChart(newIssues) {
                const issues = (newIssues != undefined && newIssues.length > 0) ? newIssues : this.issues;

                this.$refs.scopeEstimates.issues = issues;

                this.$refs.planEstimates.issues = issues;
            },

            navigateToEstimates() {
                router.push({ name: 'estimates', params: { id: this.sprintId } });
            },

            updateIssueFilter() {
                if (this.hasFilters) {
                    this.showFilteredTasks()
                } else {
                    this.showDevelopmentTasks()
                }
            },

            showDevelopmentTasks: function () {
                this.gridMode = 'Development';

                this.refreshIssuesGrid();
                this.$refs.planEstimates.issues = this.issues;
                this.$refs.scopeEstimates.issues = this.issues;
            },

            showQaTasks: function () {
                this.gridMode = 'QA';

                this.refreshIssuesGrid();
                this.refreshEstimatesChart();
            },

            showOutstandingTasks: function () {
                this.gridMode = 'Outstanding';

                this.refreshIssuesGrid();
                this.refreshEstimatesChart();
            },

            showUnplannedTasks: function () {
                this.gridMode = 'Unplanned';

                this.refreshIssuesGrid();
                this.refreshEstimatesChart();
            },

            showFilteredTasks: function () {
                const self = this;

                self.gridMode = 'Development';

                const grid = self.$refs.issuesGrid.grid;
                const filterFunc = this.getFilterFunc();

                let filteredIssues = [];
                for (let issue of self.issues) {
                    if (filterFunc(issue)) {
                        filteredIssues.push(issue);
                    }
                }

                self.refreshGridWithData(grid, filteredIssues);
                self.refreshEstimatesChart(filteredIssues);
            },

            getFilterFunc() {
                let resultFunc1 = (issue) => true;
                let resultFunc2 = (issue) => true;
                let resultFunc3 = (issue) => true;

                if (this.searchText != '') {
                    const text = this.searchText.toUpperCase();
                    resultFunc1 = (issue) => issue.key.toUpperCase().includes(text) || issue.summary.toUpperCase().includes(text);
                }

                if (this.currentFilterByStatus.length > 0) {
                    const statuses = this.currentFilterByStatus.map(s => { return s.toUpperCase(); });
                    resultFunc2 = (issue) => (statuses.includes(issue.statusName.toUpperCase()));
                }

                if (this.collapsedSubtasks) {
                    resultFunc3 = (issue) => (issue.parentId == null);
                }

                return (task) => resultFunc1(task) && resultFunc2(task) && resultFunc3(task);
            },

            clearFiltersClick() {
                this.refreshIssuesGrid();
                this.refreshEstimatesChart();
            },

            filterClick() {
            },

            savePeriodClick() {

            },

            exportToExcel() {
                webix.toExcel(this.$refs.issuesGrid.grid);
            },

            exportToPdf() {
                webix.toPDF(this.$refs.issuesGrid.grid);
            },

            refreshData(invalidateCache, partialInvalidation) {
                const self = this;
                const sprintId = this.$route.params.id;

                self.hasErrors = true;
                self.loadingInProgress = true;

                this.$store.dispatch(GET_PREFERENCES);

                this.$store
                    .dispatch(
                        GET_SPRINT_ISSUES,
                        {
                            sprintId,
                            invalidateCache,
                            partialInvalidation
                        })
                    .then(issuesData => {
                        self.devIssues = issuesData.devIssues;
                        self.qaIssues = issuesData.qaIssues;
                        self.problemIssues = issuesData.problemIssues;
                        self.unplannedIssues = issuesData.unplannedIssues;

                        self.message = {
                            hasErrors: issuesData.hasErrors,
                            title: 'JIRA response in incomplete',
                            details: 'Only first page of the issue data was received',
                        }

                        self.devChartData = issuesData.devChartData;
                        self.crChartData = issuesData.crChartData;
                        self.testingChartData = issuesData.testingChartData;

                        self.devEstimates = issuesData.devEstimates;
                        self.qaEstimates = issuesData.qaEstimates;
                        self.problemEstimates = issuesData.problemEstimates;
                        self.unplannedEstimates = issuesData.unplannedEstimates;
                        self.loadingInProgress = false;
                        self.refreshIssuesGrid();

                        this.$store.dispatch(REFRESH_GANTT);

                        self.refreshCharts();
                        self.refreshRecentlyUpdated(sprintId);

                        self.refreshEstimatesChart()

                        self.refreshIssues(self.devIssues);
                    })
                    .catch(error => {
                        self.$refs.issuesGrid.grid.showOverlay("Getting issues failed.");
                    });
            },

            refreshIssuesGrid() {
                const self = this;
                const grid = self.$refs.issuesGrid.grid;

                self.refreshGridWithData(grid, self.issues);
            },

            refreshGridWithData(grid, issues) {
                const estimates = estimateService.calculateTotalsFor(issues)
                issues = estimateService.updateTags(issues, 220, this.sprintDates.developBeginDate);

                grid.clearAll();
                grid.parse(issues);
                grid.hideOverlay();

                const colShift = 1;

                grid.config.columns[colShift + 2].footer = { text: `${issues.length} task(s)`, css: "totals" };
                grid.config.columns[colShift + 7].footer = { text: estimates.developmentEstimate, css: "totals" };
                grid.config.columns[colShift + 8].footer = { text: estimates.testingEstimate, css: "totals" };
                grid.config.columns[colShift + 9].footer = { text: estimates.timeSpent, css: "totals" };
                grid.refreshColumns();
            },

            refreshCharts() {
                this.createBurndownChart(
                    'devBurnDown',
                    'Development Burndown Chart',
                    'When task moved to [Code Review] first time',
                    this.devChartData);

                this.createBurndownChart(
                    'crBurnDown',
                    'CodeReview Burndown Chart',
                    'When task moved to [Ready For Testing] first time',
                    this.crChartData);

                this.createBurndownChart(
                    'testingBurnDown',
                    'Testing Burndown Chart',
                    'When task moved to [Integration Testing] first time',
                    this.testingChartData);

                this.updateMiniChart(this.devChartData);
            },

            createBurndownChart(chartName, chartTitle, description, chartData) {
                const self = this;
                const categories = chartData.categories;

                Highcharts.chart(chartName, {
                    credits: false,
                    chart: {
                        type: 'line'
                    },
                    title: {
                        text: chartData.sprintName + ' - ' + chartTitle
                    },
                    tooltip: {
                        useHTML: true,
                        style: {
                            pointerEvents: 'auto'
                        },
                        shared: false,
                        formatter: function () {
                            return '<b>' + this.series.name + '</b><br/>' + '' + categories[this.point.x] + '' + ': ' +
                                this.point.dayInfo.workHours + ' hrs done' +
                                this.point.dayInfo.tasks;
                        }
                    },
                    subtitle: {
                        text: description
                    },
                    xAxis: {
                        categories: categories,
                        title: {
                            text: 'Sprints dates: ' + chartData.sprintDates
                        }
                    },
                    yAxis: {
                        title: {
                            text: 'Plan estimates (hrs)'
                        },
                        min: 0
                    },
                    plotOptions: {
                        spline: {
                            marker: {
                                enabled: true
                            }
                        }
                    },

                    series: [{
                        color: 'rgb(91,155,213)',
                        lineWidth: 4,
                        name: 'Plan',
                        data: chartData.planData,
                        enableMouseTracking: false
                    }, {
                        color: 'rgb(237,125,49)',
                        lineWidth: 4,
                        name: 'Fact',
                        data: chartData.factData
                    }]
                });
            },

            updateMiniChart(chartData) {
                const self = this;
                const categories = chartData.categories;
                const maxYValue = chartData.planData[0].y;

                let chart = Highcharts.chart('devMiniChart', {
                    credits: false,
                    chart: {
                        type: 'line'
                    },
                    title: {
                        text: null
                    },
                    subtitle: {
                        text: null
                    },
                    xAxis: {
                        categories: categories,
                        title: {
                            text: null
                        },
                        labels:
                        {
                            enabled: false
                        },
                        visible: false
                    },
                    yAxis: {
                        tickPixelInterval: 20,
                        gridLineWidth: 1,
                        title: {
                            text: null
                        },
                        labels:
                        {
                            enabled: false
                        },
                        min: 0,
                        max: maxYValue,
                        visible: true
                    },
                    plotOptions: {
                        spline: {
                            marker: {
                                enabled: true
                            }
                        }
                    },

                    series: [{
                        color: 'rgb(91,155,213)',
                        lineWidth: 4,
                        name: 'Plan',
                        data: chartData.planData,
                        enableMouseTracking: false,
                        showInLegend: false
                    }, {
                        color: 'rgb(237,125,49)',
                        lineWidth: 4,
                        name: 'Fact',
                        data: chartData.factData,
                        enableMouseTracking: false,
                        showInLegend: false,
                    }]
                });

                self.exportMiniChart(chart);
            },

            exportMiniChart(chart) {
                const self = this;
                var $svg = $('#devMiniChart svg').detach();
                $('defs clipPath', $svg).remove()

                const svgImage = $svg[0].outerHTML;

                self.$http
                    .put('/api/sprint/' + self.sprintId + '/saveChart', {
                        id: self.sprintId,
                        imageBody: svgImage
                    })
                    .catch(e => {
                        console.warn(e);
                    });
            },

        }
    }
</script>
<style>
    #periodPopup {
        min-height: 285px;
    }

        #periodPopup .ui.popup .column {
            min-width: 270px;
            min-height: 230px;
        }

            #periodPopup .ui.popup .column .webix_calendar {
                border: none !important;
            }

    .totals {
        font-weight: bold;
    }

    .ui.header > .ui.horizontal.label,
    .ui.pointing.menu .ui.horizontal.label {
        margin-top: -20px;
        padding: 4px 0px 4px 0px;
        min-width: 2em;
    }

    .ui.main.menu .ui.red.label {
        border-radius: 10px;
        padding: 3px 6px 3px 6px;
    }

    .webix_cal_body .webix_cal_select, .webix_cal_body .webix_cal_select.webix_cal_today, .webix_cal_body .webix_selected {
        background-color: #2185d0;
    }

    .webix_cal_body .webix_cal_today {
        border-color: #2185d0;
    }

    .webix_cal_event_holiday {
        color: #db2828;
        font-weight: bold;
    }

    .webix_cal_event_workday {
        background-color: #d2e3ef;
    }

    .webix_cal_body .webix_cal_select.webix_cal_event_sprint_start {
        border-bottom-right-radius: 0;
        border-top-right-radius: 0;
    }

    .webix_cal_body .webix_cal_select.webix_cal_event_sprint_end {
        border-bottom-left-radius: 0;
        border-top-left-radius: 0;
    }

    .ui.grid .last.column {
        padding-right: 0;
    }

    .ui.most-left-aligned {
        margin-right: 23.8em;
    }
</style>
