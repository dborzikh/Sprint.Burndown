<template>
    <div class="component-content">
        <div class="ui vertical stripe segment" v-show="loadingInProgress" style="z-index:3">
            <div class="ui middle aligned stackable grid container">
                <div class="row" style="min-height: 85vh;">
                    <div class="ui active inverted dimmer">
                        <div class="ui text loader">Loading...</div>
                        <div class="ui blue progress" v-show="loadingProgressBar" :data-percent="loadingPercents" style="margin: 20% 20% 0 20%; height: 8px; min-width:20vw;" id="loadingProgress">
                            <div class="bar" style="height: 8px;">
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>

        <div id="reportContent" v-bind:class="{ 'grid-collapsed': loadingInProgress }">
            <div class="ui right floated main menu" style="margin-bottom: -25px;">
                <a class="undo popup icon item" data-content="Refresh List" v-on:click="reloadData()">
                    <i class="undo icon"></i>
                </a>

                <a class="filter popup icon item" data-content="Filter Sprints">
                    <i class="filter icon"></i>
                </a>

                <a class="filter popup item" data-content="Estimates" v-on:click="goToIssues()">
                    Burndown Charts
                </a>

                <div class="ui pointing dropdown link item" data-content="More Options">
                    <i class="ellipsis horizontal icon"></i>
                    <div class="ui menu" style="margin-left: -70%;">
                        <div class="item" v-on:click="exportToExcel()">Export to Excel</div>
                        <div class="item" v-on:click="exportToPdf()">Export to PDF</div>
                        <div class="divider"></div>
                        <div class="item" v-on:click="exportToPdf()">Estimates Analysis</div>
                    </div>
                </div>
            </div>

            <h1 class="ui header" style="width:50%; float: left; display: inline-block; margin: 0 0 0 12px;">{{sprintName}}<a class="anchor" id="content"></a></h1>

            <issues-grid gridName="issuesGrid" gridSize="large" ref="issuesGrid" />
        </div>
    </div>
</template>

<script>
import router from '../router'
import { setTimeout } from 'core-js';

export default {
    data: function () {
        return {
            sprintName: '',
            developBeginDate: '',
            developEndDate: '',
            loadingInProgress: true,
            loadingProgressBar: false,
            loadingPercents: 0,
            isDevelopment: true,
            devIssues: [],
            qaIssues: [],
            problemIssues: [],
            devEstimates: {},
            qaEstimates: {},
            problemEstimates: {},
            connection: null,
            message: null
        };
    },

    computed: {
        sprintId: function () {
            return this.$route.params.id;
        },

        isQA: function () {
            return !this.isDevelopment;
        },

        issues: function () {
            return this.isDevelopment ? this.devIssues : this.qaIssues;
        },

        estimates: function () {
            return this.isDevelopment ? this.devEstimates : this.qaEstimates;
        }
    },

    watch: {
        $route: function (val) {
            this.refreshData();
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

        this.connection
            .start()
            .catch(err => console.error(err.toString()))
            .then(function () {
                console.info('connection has been started');
            });

        this.connection.on('started', function (totalItems) {
            self.loadingPercents = 0;
            self.loadingInProgress = true;
            self.loadingProgressBar = true;
            $('#loadingProgress').progress({ percent: 0 });
        });

        this.connection.on('processed', function (currentItem, totalItems) {
            currentItem = currentItem > totalItems ? totalItems : currentItem;
            self.loadingPercents = totalItems <= 0 ? 0 : Math.round(currentItem / totalItems * 100);
            $('#loadingProgress').progress({ percent: self.loadingPercents });
            console.info('loading -> processed: ', self.loadingPercents);
        });

        this.connection.on('finished', function () {
            self.loadingInProgress = false;
            self.loadingProgressBar = false;
        });

        var calendar1 = {
            view: "calendar",
            container: "periodStartDate",
            id: "startDate",
            date: new Date(2018, 5, 3),
            weekHeader: true,
            width: 260,
            height: 220
        };

        var calendar2 = {
            view: "calendar",
            container: "periodEndDate",
            id: "endDate",
            date: new Date(2018, 5, 17),
            weekHeader: true,
            width: 260,
            height: 220
        };

        $('.ui.dropdown').dropdown({ action: 'hide' });
        $('#actualPeriod')
            .popup({
                inline: true,
                on: 'click',
                position: 'bottom center',
                setFluidWidth: false,
                delay: {
                    show: 300,
                    hide: 800
                },
                onVisible: function () {
                    if (!$('.webix_calendar', '#periodPopup').length) {
                        webix.ui(calendar1);
                        webix.ui(calendar2);
                    }

                    $$("startDate").selectDate(new Date(2018, 5, 3));
                    $$("endDate").selectDate(new Date(2018, 5, 17));
                }
            });

        this.refreshData();
    },

    methods: {
        goToIssues(sprintId) {
            router.push({ name: 'sprint', params: { id: this.sprintId } });
        },

        showDevelopmentTasks: function () {
            this.isDevelopment = true;
            this.refreshIssuesGrid();
        },

        showQaTasks: function () {
            this.isDevelopment = false;
            this.refreshIssuesGrid();
        },

        reloadData() {
            this.refreshData(true);
        },

        exportToExcel() {
            webix.toExcel(this.$refs.issuesGrid.grid);
        },

        exportToPdf() {
            webix.toPDF(this.$refs.issuesGrid.grid);
        },

        refreshData(invalidateCache) {
            const self = this;
            const sprintId = this.$route.params.id;
            invalidateCache = !!invalidateCache;

            var issueProjection = (value) => {
                return {
                    id: value.id,
                    key: value.key,
                    parentId: value.parentId,
                    parentKey: value.parentKey,
                    summary: value.summary,
                    assignee: value.assigneeName,
                    issueTypeName: value.issueTypeName,
                    issueTypeIconUrl: value.issueTypeIconUrl,
                    statusName: value.statusName,
                    statusIconUrl: value.statusIconUrl,
                    developmentEstimate: value.developmentEstimate,
                    testingEstimate: value.testingEstimate,
                    remainingEstimate: value.remainingEstimate,
                    timeSpent: value.timeSpent,
                    includedInSprintDate: value.includedInSprintDate != undefined
                        ? moment(value.includedInSprintDate).format('D/MMM/YY')
                        : '',
                    techAnalysisDate: value.technicalAnalysisCompletedDate != undefined
                        ? moment(value.technicalAnalysisCompletedDate).format('D/MMM/YY')
                        : '',
                    developmentDate: value.developmentCompletedDate != undefined
                        ? moment(value.developmentCompletedDate).format('D/MMM/YY')
                        : '',
                    codeReviewDate: value.codeReviewCompletedDate != undefined
                        ? moment(value.codeReviewCompletedDate).format('D/MMM/YY')
                        : '',
                    testingDate: value.testingCompletedDate != undefined
                        ? moment(value.testingCompletedDate).format('D/MMM/YY')
                        : ''
                };
            };

            var chartDataProjection = (sourceChartData) => {
                return {
                    sprintName: sourceChartData.sprintName,
                    sprintDates: moment(sourceChartData.planBeginDate).format('D/MMM/YYYY') + ' - ' + moment(sourceChartData.planEndDate).format('D/MMM/YYYY'),
                    categories: sourceChartData.categories,
                    factData: sourceChartData.data.map(chartItemProjection),
                    planData: [
                        {
                            x: sourceChartData.planBeginIndex,
                            y: sourceChartData.totalEstimate,
                            dayInfo: {}
                        },
                        {
                            x: sourceChartData.planEndIndex,
                            y: 0,
                            dayInfo: {}
                        }
                    ]
                }
            };

            var chartItemProjection = (value) => {
                var tasks = ''
                var maxLen = value.tasks.length > 5 ? 5 : value.tasks.length;

                if (value.tasks.length > 0) {
                    for (var i = 0; i < maxLen; i++) {
                        var task1 = value.tasks[i]
                        tasks += '<br/>' + '<a style="color:#2185d0;" href="' + task1.url + '" target="_blank">' + task1.key + '</a>';
                    }
                }

                if (maxLen < value.tasks.length) {
                    tasks += '<br/>' + '+' + (value.tasks.length - maxLen) + 'task(s)'
                }

                return {
                    x: value.index,
                    y: value.workDone,
                    dayInfo: {
                        workHours: value.workHours,
                        tasks: tasks
                    }
                };
            };

            self.loadingInProgress = true;
            var cacheHeaders = {
                'Cache-Control': invalidateCache ? 'no-cache' : 'only-if-cached'
            };

            self.$http.get('/api/sprint/' + sprintId, {
                params: {
                    //id: sprintId
                },
                headers: cacheHeaders
            })
                .then(response => {
                    self.sprintName = response.data.sprintName;
                    self.developBeginDate = moment(response.data.developBeginDate).format('D/MMM/YYYY');
                    self.developEndDate = moment(response.data.developEndDate).format('D/MMM/YYYY');
                    self.devIssues = response.data.devIssues.map(issueProjection);
                    self.qaIssues = response.data.qaIssues.map(issueProjection);
                    self.problemIssues = response.data.problemIssues.map(issueProjection);
                    self.unplannedIssues = response.data.unplannedIssues.map(issueProjection);

                    var devChartData = chartDataProjection(response.data.devChartData);
                    var crChartData = chartDataProjection(response.data.crChartData);
                    var testingChartData = chartDataProjection(response.data.testingChartData);

                    self.devEstimates = response.data.devEstimates;
                    self.qaEstimates = response.data.qaEstimates;
                    self.problemEstimates = response.data.problemEstimates;
                    self.unplannedEstimates = response.data.unplannedEstimates;

                    self.loadingInProgress = false;

                    self.refreshIssuesGrid();
                    self.refreshImpedimentsGrid();
                    self.refreshUnplannedGrid();

                    self.refreshCharts(
                        devChartData,
                        crChartData,
                        testingChartData);
                })
                .catch(e => {
                    console.warn(e);
                    self.$refs.issuesGrid.grid.showOverlay("Getting issues failed.");
                });
        },


        refreshIssuesGrid() {
            const self = this;
            const grid = self.$refs.issuesGrid.grid;

            self.refreshGridWithData(grid, self.issues, self.estimates);
        },

        refreshImpedimentsGrid() {
            const self = this;
            const grid = self.$refs.impedimentsGrid.grid;

            self.refreshGridWithData(grid, self.problemIssues, self.problemEstimates);
        },

        refreshUnplannedGrid() {
            const self = this;
            const grid = self.$refs.unplannedGrid.grid;

            self.refreshGridWithData(grid, self.unplannedIssues, self.unplannedEstimates);
        },

        refreshGridWithData(grid, issues, estimates) {
            grid.clearAll();
            grid.parse(issues);
            grid.hideOverlay();

            grid.config.columns[6].footer = { text: estimates.developmentEstimate, css: "totals" };
            grid.config.columns[7].footer = { text: estimates.testingEstimate, css: "totals" };
            grid.config.columns[8].footer = { text: estimates.remainingEstimate, css: "totals" };
            grid.config.columns[9].footer = { text: estimates.timeSpent, css: "totals" };
            grid.refreshColumns();
        },

        refreshCharts(devChartData, crChartData, testingChartData) {
            this.createBurndownChart(
                'devBurnDown',
                'Development Burndown Chart',
                'When task was moved to [Code Review] first time',
                devChartData);

            this.createBurndownChart(
                'crBurnDown',
                'CodeReview Burndown Chart',
                'When task was moved to [Ready For Testing] first time',
                crChartData);
        },

        createBurndownChart(chartName, chartTitle, description, chartData) {
            const self = this;
            const categories = chartData.categories;

            Highcharts.chart(chartName, {
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
        }
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
    
    .multiline {
        line-height: 20px !important;
        padding: 5px 10px;
        text-align: center;
    }

    .totals {
        font-weight: bold;
    }
</style>
