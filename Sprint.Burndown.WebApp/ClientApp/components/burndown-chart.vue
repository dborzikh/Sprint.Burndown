<template>
    <div class="ui segment chart-container">
        <div :id="chartName" class="inner-chart" style=""></div>
    </div>
</template>

<script>
export default {
        props: ['chartName', 'chartTitle', 'chartDescription', 'chartData'],

    data: function () {
        return { }
    },

    mounted() {
        this.createBurndownChart(this.chartName, this.chartTitle, this.chartDescription, this.chartData);
    },

    methods: {
        createBurndownChart(chartName, chartTitle, description, chartData) {
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
    .ui.chart-container {
        padding: 0px;
        margin-bottom: 16px;
        width: 49%;
        display: inline-block;
        margin-right: 8px;
    }

    .ui.chart-container .inner-chart {
        height: 45vh;
        width: 95%;
        overflow: hidden;
        margin: 8px;
    }
</style>
