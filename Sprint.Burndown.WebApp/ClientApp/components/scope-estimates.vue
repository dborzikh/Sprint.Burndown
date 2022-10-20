<template>
    <div class="ui segment scope-estimates">
        <div id="scopeEstimatesChart"></div>
    </div>
</template>

<script>
    import estimateService from '../estimateService'
    import chartUtils from '../chartUtils'

    export default {
        //chart: null,

        data: function () {
            return {
                issues: []
            };
        },

        computed: {
            developVelocity() {
                return this.$store.getters.getSprintSummary.developVelocity;
            }
        },

        watch: {
            developVelocity() {
                this.refreshChart();
            },

            issues() {
                this.refreshChart();
            }
        },

        mounted() {
        },

        methods: {
            refreshChart() {
                const self = this;
                const seriesData = self.getSeriesData();
                self.createEstimatesChart(seriesData);

                //if (self.$options.chart == null) {
                //    self.$options.chart = self.createEstimatesChart(seriesData)
                //} else {
                //    self.$options.chart.update({
                //        series: seriesData
                //    });
                //}
            },

            getSeriesData() {
                const self = this;
                const estimatesByScope = estimateService.groupEstimatesByScope(self.issues)

                estimatesByScope.set('Velocity', self.developVelocity > 0 ? self.developVelocity : 1);

                return chartUtils.convertToBarSeries(estimatesByScope);
            },

            createEstimatesChart(seriesData) {
                return Highcharts.chart('scopeEstimatesChart', {
                    credits: false,
                    chart: {
                        type: 'bar'
                    },
                    title: {
                        text: 'Development Scope'
                    },
                    xAxis: {
                        labels: false
                        //categories: ['hrs']
                    },
                    yAxis: {
                        min: 0,
                        stackLabels: {
                            enabled: true
                        }
                    },
                    plotOptions: {
                        bar: {
                            dataLabels: {
                                enabled: true
                            }
                        },
                        series: {
                            events: {
                                legendItemClick: function (event) {
                                }
                            },
                            animation: {
                                duration: 0
                            }
                        }
                    },
                    legend: {
                        reversed: true,
                        style: {
                            fontWeight: 'normal'
                        }
                    },
                    series: seriesData
                });
            }
        }
    }
</script>

<style>
    .ui.scope-estimates.segment {
        padding: 0px;
        margin-bottom: 16px;
        width: 100%;
        height: 300px;
        display: block;
        margin-right: 8px;
    }

    #scopeEstimatesChart {
        height: 95%;
        width: 95%;
        overflow: hidden;
    }
</style>
