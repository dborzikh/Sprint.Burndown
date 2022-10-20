<template>
    <div class="ui segment issues-versions">
        <div id="versionsChart"></div>
    </div>
</template>
<script>
    import { mapGetters } from 'vuex';
    import estimateService from '../estimateService'

    export default {
        props: [],

        data: function () {
            return {
                versions: []
            };
        },

        mounted() {
        },

        computed: {
            ...mapGetters([
                'hasVersions',
                'issues'
            ]),
        },

        watch: {
            hasVersions() {
                this.refreshChart();
            },

            issues() {
                this.refreshChart();
            }
        },

        methods: {
            refreshChart() {
                if (!this.hasVersions) return;

                const { seriesData, categories } = this.getSeriesData();
                this.createVersionsChart(seriesData, categories);
            },

            getSeriesData() {
                const self = this;
                const SECONDS_IN_HOUR = 3600;

                let estimatedIssues = _.map(self.issues, (issue) => {
                    return {
                        id: issue.id,
                        key: issue.key,
                        parentId: issue.parentId,
                        summary: issue.summary,
                        version: issue.versionName,
                        estimateInHrs: issue.isChildrenEstimates ? 0 : issue.developmentEstimateSeconds / SECONDS_IN_HOUR
                    }
                });

                // collapse sub-tasks
                if (true) {
                    const parents = _.filter(estimatedIssues, (o) => o.parentId == null);
                    const children = _.filter(estimatedIssues, (o) => o.parentId != null);

                    for (let issue of children) {
                        let parentIssue = _.find(parents, (o) => o.id == issue.parentId);

                        if (parentIssue == undefined) {
                            console.warn('parent not found for: ', issue);
                            continue;
                        }

                        parentIssue.estimateInHrs += issue.estimateInHrs;
                    }

                    estimatedIssues = _.orderBy(parents, "key");
                }

                estimatedIssues = _.orderBy(_.filter(estimatedIssues, (o) => o.estimateInHrs > 0), 'key');

                const versions = _.orderBy(_.uniq(_.map(estimatedIssues, 'version')));

                let taskEstimates = [];
                for (let issue of estimatedIssues) {
                    let item = {
                        name: issue.key + ' ' + issue.summary.substring(0, 40) + '...',
                        data: []
                    };

                    for (let version of versions) {
                        item.data.push(issue.version === version ? issue.estimateInHrs : 0);
                    }

                    taskEstimates.push(item);
                }

                return {
                    seriesData: taskEstimates,
                    categories: versions
                };
            },

            createVersionsChart(seriesData, categories) {
                Highcharts.chart('versionsChart', {
                    chart: {
                        type: 'bar'
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
                        }
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: 'Estimate in Hrs',
                            align: 'high'
                        },
                        labels: {
                            overflow: 'justify'
                        },
                        stackLabels: {
                            enabled: true
                        }
                    },
                    tooltip: {
                        valueSuffix: ' hrs'
                    },
                    plotOptions: {
                        series: {
                            stacking: 'normal'
                        }
                    },
                    legend: {
                        enabled: false,
                    },
                    credits: {
                        enabled: false
                    },
                    series: seriesData
                });
            }
        }
    }
</script>
<style>
    .ui.issues-versions.segment {
        padding: 8px;
        margin-bottom: 16px;
        width: 100%;
        min-height: 100px;
        display: block;
        margin-right: 8px;
        margin-top: 28px;
    }

    .ui.issues-versions .labels {
        max-width: 960px;
    }

    .ui.issues-versions .ui.image.label {
        margin-bottom: 4px;
    }
</style>
