import Vue from 'vue'

export default {
    SECONDS_IN_HOUR: 3600,

    toStringEstimate(estimateInSeconds) {
        let hours = Math.round(estimateInSeconds / this.SECONDS_IN_HOUR);
        let minutes = Math.round((estimateInSeconds - (hours * this.SECONDS_IN_HOUR)) / 60);

        return `${hours}h` + (minutes > 0 ? ` ${minutes}m` : '');
    },

    groupEstimatesByScope(issues) {
        let estimatesByScope = new Map();

        for (let issue of issues) {
            const group = issue.tags;
            const estimateInHrs = issue.isChildrenEstimates ? 0 : issue.developmentEstimateSeconds / this.SECONDS_IN_HOUR;

            if (!estimatesByScope.has(group)) {
                estimatesByScope.set(group, estimateInHrs);
            } else {
                const oldValue = estimatesByScope.get(group);
                estimatesByScope.set(group, oldValue + estimateInHrs);
            }
        }

        return estimatesByScope;
    },

    groupCountByReporters(issues) {
        const groupedResult = this.groupBy(issues, p => p.reporter, () => 1);
        const sortedResult = new Map([...groupedResult.entries()].sort((a, b) => b[1] - a[1]));

        return sortedResult;
    },

    groupBy(elements, keySelector, valueSelector) {
        let result = new Map();

        for (let element of elements) {
            const key = keySelector(element);
            const value = valueSelector(element);

            if (!result.has(key)) {
                result.set(key, value);
            } else {
                const oldValue = result.get(key);
                result.set(key, oldValue + value);
            }
        }

        return result;
    },

    calculateTotalsFor(issues) {
        const self = this;

        let result = {
            issuesCount: 0,

            developmentEstimateSeconds: 0,
            testingEstimateSeconds: 0,
            timeSpentSeconds: 0,

            get developmentEstimate() {
                return self.toStringEstimate(this.developmentEstimateSeconds);
            },

            get testingEstimate() {
                return self.toStringEstimate(this.testingEstimateSeconds);
            },

            get timeSpent() {
                return self.toStringEstimate(this.timeSpentSeconds);
            }
        };

        for (let issue of issues) {
            result.issuesCount += 1;
            result.developmentEstimateSeconds += issue.developmentEstimateSeconds;
            result.testingEstimateSeconds += issue.testingEstimateSeconds;
            result.timeSpentSeconds += issue.timeSpentSeconds;
        }

        return result;
    },

    updateTags(issues, velocity, developBeginDate) {
        return issues;

        for (let issue of issues) {
            issue['$cellCss'] = {
                votes: 'tag-' + issue.tagsShortName
            };
        }


        const beginDate = moment(developBeginDate, 'D/MMM/YYYY').toDate().getTime();

        const sortedIssues = issues.sort(function(p, idx) {
            return moment(p.developmentDate).toDate().getTime() < beginDate ? 0 : 1;
        });

        for (let issue of issues) {
            if (issue.developmentDate == '' || issue.developmentDate == null) {
                issue.tags = 'In the sprint scope';
            }
            else if (moment(issue.developmentDate, 'D/MMM/YYYY').toDate().getTime() < beginDate) {
                issue.tags = 'Moved from previous sprints';
            } else {
                issue.tags = 'In the sprint scope';
            }
        }

        return sortedIssues;
    }
}