<template>
    <div class="ui segment plan-estimates">
        <div id="planEstimatesChart"></div>
    </div>
</template>

<script>
import estimateService from '../estimateService'

export default {
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
            const seriesData = this.getSeriesData();
            this.createEstimatesChart(seriesData);
        },

        getSeriesData() {
            const self = this;

            let estimatesByStatus = new Map();
            for (let issue of self.issues) {
                const group = issue.statusName;
                const estimateInHrs = issue.isChildrenEstimates ? 0 : issue.developmentEstimateSeconds / 3600;

                if (!estimatesByStatus.has(group)) {
                    estimatesByStatus.set(group, estimateInHrs);
                } else {
                    const oldValue = estimatesByStatus.get(group);
                    estimatesByStatus.set(group, oldValue + estimateInHrs);
                }
            }

            let seriesData = [{
                name: 'Team Velocity',
                data: [0, self.developVelocity]
            }];

            estimatesByStatus.forEach((value, key, map) => {
                if (value > 0) {
                    seriesData.push({
                        name: key,
                        data: [value, 0]
                    });
                }
            })

            return seriesData;
        },

        createEstimatesChart(seriesData) {
            return Highcharts.chart('planEstimatesChart', {
                credits: false,
                chart: {
                    type: 'bar'
                },
                title: {
                    text: null
                },
                xAxis: {
                    categories: ['Estimates', 'Velocity']
                },
                yAxis: {
                    min: 0,
                    stackLabels: {
                        enabled: true
                    }
                },
                legend: {
                    layout: 'vertical',
                    align: 'right',
                    verticalAlign: 'middle',
                    itemMarginTop: 0,
                    itemMarginBottom: 0,
                    reversed: true,
                    style: {
                        fontWeight: 'normal'
                    }
                },
                plotOptions: {
                    series: {
                        stacking: 'normal',
                        animation: {
                            duration: 0
                        }
                    }
                },
                series: seriesData
            });
        }
    }
}
</script>

<style>
    .ui.plan-estimates.segment {
        padding: 0px;
        margin-bottom: 16px;
        width: 100%;
        height: 300px;
        display: block;
        margin-right: 8px;
    }

    #planEstimatesChart {
        height: 95%;
        width: 95%;
        overflow: hidden;
    }
</style>
