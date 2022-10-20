import axios from 'axios'
import moment from 'moment'
import { Promise } from 'core-js';

import $store from './store'
import { SET_SPRINT_SUMMARY } from './store/actions/viewState'

export default {
    $http: axios,

    getIssues(sprintId, invalidateCache, partialInvalidation) {
        const self = this;

        invalidateCache = !!invalidateCache;
        partialInvalidation = partialInvalidation == undefined ? true : (!!partialInvalidation);

        var cacheHeaders = {
            'Cache-Control': invalidateCache ? 'no-cache' : 'only-if-cached',
            'X-Cache-Update': partialInvalidation ? 'partial' : 'full'
        };

        var result = new Promise(function(resolve, reject) {
            self.$http
                .get('/api/sprint/' + sprintId,
                    {
                        params: {},
                        headers: cacheHeaders
                    })
                .then(response => {
                    const sprintData = {
                        devIssues: response.data.devIssues.map(self.issueProjection),
                        qaIssues: response.data.qaIssues.map(self.issueProjection),
                        problemIssues: response.data.problemIssues.map(self.issueProjection),
                        unplannedIssues: response.data.unplannedIssues.map(self.issueProjection),

                        message: {
                            hasErrors: response.data.hasIncompleteData,
                            title: 'JIRA response is incomplete',
                            details: 'Only first page of the issues data was received'
                        },

                        devChartData: self.chartDataProjection(response.data.devChartData),
                        crChartData: self.chartDataProjection(response.data.crChartData),
                        testingChartData: self.chartDataProjection(response.data.testingChartData),

                        devEstimates: response.data.devTotals,
                        qaEstimates: response.data.qaTotals,
                        problemEstimates: response.data.problemTotals,
                        unplannedEstimates: response.data.unplannedTotals,
                        taskStatusSummaries: response.data.taskStatusSummaries
                    };

                    const sprintInfo = {
                        boardId: response.data.boardId,
                        boardName: response.data.boardName,
                        sprintId: response.data.sprintId,
                        sprintName: response.data.sprintName,
                        developBeginDate: moment(response.data.developBeginDate).format('D/MMM/YYYY'),
                        developEndDate: moment(response.data.developEndDate).format('D/MMM/YYYY'),
                        testingBeginDate: moment(response.data.testingBeginDate).format('D/MMM/YYYY'),
                        testingEndDate: moment(response.data.testingEndDate).format('D/MMM/YYYY'),
                        regressionBeginDate: moment(response.data.regressionBeginDate).format('D/MMM/YYYY'),
                        regressionEndDate: moment(response.data.regressionEndDate).format('D/MMM/YYYY')
                    }

                    $store.commit(SET_SPRINT_SUMMARY, sprintInfo);

                    resolve(sprintData);

                })
                .catch(error => {
                    console.warn("Getting issues failed.\n", error);
                    reject(error);
                });
        });

        return result;
    },

    issueProjection(value) {
        return {
            id: value.id,
            key: value.key,
            tags: value.tags,
            tagsShortName: value.tagsShortName,
            parentId: value.parentId,
            parentKey: value.parentKey,
            priorityId: value.priorityId,
            priorityName: value.priorityName,
            priorityIconUrl: value.priorityIconUrl,
            summary: value.summary,
            assignee: value.assigneeName,
            reporter: value.reporterName,
            reporterName: value.reporterDisplayName,
            reporterEmail: value.reporterEmail,
            fixVersions: value.fixVersions,
            issueTypeName: value.issueTypeName,
            issueTypeIconUrl: value.issueTypeIconUrl,
            statusName: value.statusName,
            statusIconUrl: value.statusIconUrl,
            developmentEstimate: value.developmentEstimate,
            developmentEstimateSeconds: value.developmentEstimateSeconds,
            testingEstimate: value.testingEstimate,
            testingEstimateSeconds: value.testingEstimateSeconds,
            isChildrenEstimates: value.isChildrenEstimates,
            timeSpent: value.timeSpent,
            timeSpentSeconds: value.timeSpentSeconds,
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
    },

    chartDataProjection(sourceChartData) {
        return {
            sprintName: sourceChartData.sprintName,
            sprintDates: moment(sourceChartData.planBeginDate).format('D/MMM/YYYY') + ' - ' + moment(sourceChartData.planEndDate).format('D/MMM/YYYY'),
            categories: sourceChartData.categories,
            factData: sourceChartData.data.map(this.chartItemProjection),
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
    },

    chartItemProjection(value) {
        var tasks = '';

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
    }
}