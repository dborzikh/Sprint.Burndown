<template>
        <table class="ui celled table issues-versions">
            <thead>
                <tr>
                    <th>&nbsp;</th>
                    <th>Fix Version</th>
                    <th>Development, hrs</th>
                    <th>Testing, hrs</th>
                    <th>Tasks</th>
                </tr>
            </thead>

            <tbody>
                <tr>
                    <td style="width: 3em; text-align: center;">
                        <div class="ui checkbox">
                            <input type="checkbox">
                            <label>&nbsp;</label>
                        </div>
                    </td>
                    <td style="min-width: 10em;">&nbsp;</td>
                    <td style="min-width: 8em;">&nbsp;</td>
                    <td style="min-width: 8em;">&nbsp;</td>
                    <td style="min-width: 6em;">&nbsp;</td>
                </tr>
            </tbody>
        </table>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
import estimateService from '../estimateService'

export default {
    props: [],

    data: function () {
        return {
            issues: [],
            velocity: 0
        };
    },

    mounted() {
    },

    watch: {
    },

    methods: {
        refreshChart() {
            return;

            const self = this;

            const estimatesByScope = estimateService.groupEstimatesByScope(self.issues)
            estimatesByScope.set('Velocity', self.velocity);

            let seriesData = [];
            estimatesByScope.forEach((value, key, map) => {
                if (value > 0) {
                    seriesData.push({
                        name: key,
                        data: [value]
                    });
                }
            })

            Highcharts.chart('scopeEstimatesChart', {
                credits: false,
                chart: {
                    type: 'bar'
                },
                title: {
                    text: 'Development Scope'
                },
                xAxis: {
                    labels: false
                },
                yAxis: {
                    min: 0,
                    stackLabels: {
                        enabled: true
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
