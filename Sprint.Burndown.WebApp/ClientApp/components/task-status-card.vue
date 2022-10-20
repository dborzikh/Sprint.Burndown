<template>
    <div class="ui link card task-summary" v-bind:class="{ blue: isActive }" v-on:click="toggleActive">
        <div class="content">
            <div class="ui sub header">
                {{title}}
                <a v-if="indicatorValue > 0" class="ui red empty circular label"></a>
                <div class="ui checkbox pull-right">
                    <input type="checkbox" name="activeFilter" :value="isActive">
                    <label>&nbsp;</label>
                </div>
            </div>
            
        </div>
        <div class="content" style="padding: 4px;">
            <div class="ui small feed" :id="chartId" style="height: 140px; width: 340px;">
            </div>
        </div>
        <div class="extra content">
            <div class="event">
                <div class="content">
                    <div class="summary">
                        {{summary}}
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex';
    import { ADD_TASK_STATUS_FILTERS, REMOVE_TASK_STATUS_FILTERS } from '../store/actions/viewState'

    export default {
        props: ['group'],

        data: function () {
            return {
                isActive: false,
                indicatorValue: 0,
                title: ''
            };
        },

        mounted() {
            $('.ui.checkbox').checkbox();
            this.refreshChart(); // why is this required? watcher must work instead!
        },

        computed: {
            chartId: function () {
                return 'pieChart' + this.group;
            },

            summary() {
                return this.indicatorValue == 0
                    ? 'All tasks done'
                    : '';
            },

            ...mapGetters([
                'currentFilterByPhase',
                'hasNoFilters',
                'issuesSummaries'
            ])
        },

        watch: {
            hasNoFilters() {
                if (this.hasNoFilters) {
                    //this.isActive = false;
                }
            },

            currentFilterByPhase(newFilters) {
                const item = _.find(newFilters, ['group', this.group]);
                if (item != undefined) {
                    this.isActive = item.checked;
                }
            },

            issuesSummaries() {
                this.refreshChart();
            }
        },

        methods: {
            toggleActive() {
                this.$store.commit(
                    this.isActive ? REMOVE_TASK_STATUS_FILTERS : ADD_TASK_STATUS_FILTERS,
                    this.group)
            },

            refreshChart() {
                const chartData = this.getSeriesData()
                this.createIndicatorChart(chartData);
            },

            getSeriesData() {
                const self = this;

                const dataItem = _.find(
                    self.issuesSummaries,
                    (o) => o.group == self.group);

                self.title = dataItem.title;

                let totals = 0;
                dataItem.serieValues.every(item => totals += item.value);
                self.indicatorValue = totals;

                let chartData = [];
                for (let t of dataItem.serieValues) {
                    chartData.push([t.name, t.value == 0 ? 0.00001 : t.value]);
                }

                return chartData;
            },

            createIndicatorChart(seriesData) {
                const self = this;

                Highcharts.chart(self.chartId, {
                    credits: false,
                    chart: {
                        align: 'left',
                        enableMouseTracking: false,
                        plotBackgroundColor: null,
                        plotBorderWidth: 0,
                        plotShadow: false,
                        enableMouseTracking: false,
                    },
                    legend: {
                        align: 'right',
                        enableMouseTracking: false,
                        labelFormat: '{name} - {y:.0f}',
                        itemStyle: { fontWeight: 'normal' },
                        maxHeight: 150,
                        maxWidth: 220,
                        height: 150,
                        width: 220,
                        verticalAlign: 'middle',
                        x: 30,
                        navigation: {
                            enabled: false
                        }
                    },
                    title: {
                        text: self.indicatorValue,
                        align: 'left',
                        verticalAlign: 'middle',
                        style: { fontSize: '42px', fontFamily: 'Arial', color: '#AAA' },
                        x: (self.indicatorValue < 10 ? 35 : 20),
                        y: 15
                    },
                    tooltip: {
                        enabled: false,
                    },
                    plotOptions: {
                        pie: {
                            dataLabels: {
                                enabled: false
                            },
                            center: ['40%', '50%'],
                            size: '110%',
                            showInLegend: true
                        }
                    },
                    series: [{
                        enableMouseTracking: false,
                        type: 'pie',
                        name: self.title,
                        innerSize: '75%',
                        data: seriesData
                    }]
                });
            }
        }
    }
</script>

<style>
    .ui.task-summary.card {
        margin: 8px;
        min-width: 359px;
    }

    .ui.task-summary.card.blue {
        box-shadow: 0 0 0 1px #d4d4d5, 0 3px 0 0 #2185d0, 0 1px 3px 0 #d4d4d5;
    }

    .ui.task-summary.card .sub.header {
        overflow: hidden;
        max-height: 22px;
        max-width: 330px;
        word-break: break-all;
    }

    .ui.task-summary.card .sub.header .ui.checkbox {
        display: inline;
        margin: 0;
        margin-right: -12px;
    }
</style>
