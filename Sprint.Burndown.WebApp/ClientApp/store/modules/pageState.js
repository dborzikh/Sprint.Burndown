/* eslint-disable promise/param-names */
import axios from 'axios'
import moment from 'moment'
import { Promise } from 'core-js';

import {
    RECENTLY_UPDATED_STARTED, RECENTLY_UPDATED_FINISHED,
    GET_PREFERENCES, GET_SPRINT_ISSUES, 
    SET_SPRINT_ISSUES, SET_ISSUES_SUMMARIES,
    REFRESH_ISSUES, RELOAD_ISSUES, REFRESH_GANTT, REFRESH_GANTT_STARTED, REFRESH_GANTT_FINISHED, 
    SET_SPRINT_SUMMARY, SET_SPRINT_DATES, SET_SPRINT_VELOCITY
} from '../actions/pageState'

import calendarService from '../../calendarService'
import estimateService from '../../estimateService'
import issuesService from '../../issuesService'
import {FILTER_STARRED_ISSUES} from "../actions/pageFilters";

const $http = axios;

const state = {
    unfilteredIssues: [],
    //issues: [],
    issuesSummaries: [],
    lastUpdated: null,

    calendar: {
        inProgress: false,
        holidays: []
    },

    sprintSummary: {
        boardId: 0,
        boardName: '',

        sprintId: 0,
        sprintName: '',

        developBeginDate: '',
        developEndDate: '',
        developWorkDays: [],
        developVelocity: 0,
        developSingleVelocity: (6 * 2 + 4),

        testingBeginDate: '',
        testingEndDate: '',
        testingWorkDays: [],
        testingVelocity: 0,
        testingSingleVelocity: (6 * 2 + 4),

        regressionBeginDate: '',
        regressionEndDate: '',
        regressionWorkDays: []
    },

    recentlyUpdated: {
        inProgress: false,
        issuesCount: 0
    },

    reloadIssuesCount: 0
}

const getters = {
    issues: (state) => {
        if (state.unfilteredIssues === undefined) {
            return [];
        }

        let filteredIssues = [];
        const text = state.searchText.toUpperCase();
        const statuses = state.filterByStatus;

        for (let issue of state.unfilteredIssues) {
            if (state.searchText !== '') {
                const hasTextFound = issue.key.toUpperCase().includes(text) || issue.summary.toUpperCase().includes(text);

                if (!hasTextFound) {
                    continue;
                }
            }

            if (state.filterByStatus.length > 0) {
                if (!statuses.includes(issue.statusName.toUpperCase())) {
                    continue;
                }
            }

            if (state.collapsedSubtasks) {
                if (issue.parentId !== null) {
                    continue;
                }
            }

            filteredIssues.push(issue);
        }

        return filteredIssues;
    },

    issuesSummaries: (state) => (state.issuesSummaries),

    lastUpdated: (state) => state.lastUpdated,

    recentlyUpdatedInProgress: (state) => state.recentlyUpdated.inProgress,
    recentlyUpdatedIssues: (state) => state.recentlyUpdated.issuesCount,
    reloadIssuesCount: (state) => state.reloadIssuesCount,

    getSprintSummary: (state) => state.sprintSummary,

    calendarInProgress: (state) => state.calendar.inProgress,
    calendarHolidays: (state) => state.calendar.holidays
}

const actions = {
    
    refreshRecentlyUpdated: ({ commit, dispatch }, sprintId) => {
        if (state.recentlyUpdated.inProgress) {
            return null;
        }

        commit(RECENTLY_UPDATED_STARTED);

        return new Promise((resolve, reject) => {
            $http.get('/api/sprint/' + sprintId + '/updates')
                .then(response => {
                    commit(RECENTLY_UPDATED_FINISHED, response.data.recentlyUpdatedIssues);
                    resolve();
                })
                .catch(error => {
                    commit(RECENTLY_UPDATED_FINISHED, 0);
                    resolve();
                });
        });
    },

    refreshIssues: ({ commit, dispatch }, issues) => {
        commit(REFRESH_ISSUES, issues);

        return null;
    },

    searchIssues: ({ commit, dispatch }, searchText) => {
        commit(SEARCH_TEXT_CHANGED, searchText);
    },

    [GET_PREFERENCES]: ({ commit, dispatch }) => {

        return new Promise((resolve, reject) => {
            $http.get('/api/board/getPreferences')
                .then(response => {
                    const loadedSubViews = response.data.preferredSubViews;

                    if (!loadedSubViews.includes(pageSubView.sprintTimeline)) {
                        commit(CHANGE_VISIBILITY_SPRINT_TIMELINE, true);
                    }
                    if (!loadedSubViews.includes(pageSubView.taskStatusSummary)) {
                        commit(CHANGE_VISIBILITY_TASK_STATUS, true);
                    }
                    if (!loadedSubViews.includes(pageSubView.issuesGrid)) {
                        commit(CHANGE_VISIBILITY_ISSUES, true);
                    }
                    if (!loadedSubViews.includes(pageSubView.burnDownCharts)) {
                        commit(CHANGE_VISIBILITY_BURNDOWN, true);
                    }

                    resolve();
                }).catch(error => {
                    console.warn("Error getting preferences.\n", error);
                    reject();
                });
        });
    },

    [GET_SPRINT_ISSUES]: ({ commit, dispatch }, payload) => {
        return new Promise((resolve, reject) => {
            issuesService
                .getIssues(payload.sprintId, payload.invalidateCache, payload.partialInvalidation)
                .then(issuesData => {
                    commit(SET_SPRINT_ISSUES, issuesData.devIssues);
                    commit(SET_ISSUES_SUMMARIES, issuesData.taskStatusSummaries);

                    resolve(issuesData);
                })
                .catch(error => {
                    console.warn("Error getting issues.\n", error);
                    reject();
                });
        });
    },

    [REFRESH_GANTT]: ({ commit, dispatch }) => {
        commit(REFRESH_GANTT_STARTED);

        return new Promise((resolve, reject) => {
            $http.get('/api/calendar/holidays')
                .then(response => {
                    const holidays = response.data.holidays.map((value) => moment(value).toDate());

                    commit(REFRESH_GANTT_FINISHED, holidays);
                    commit(SET_SPRINT_VELOCITY);

                    resolve();
                }).catch(error => {
                    console.warn("Error getting calendar holidays.\n", error);
                    commit(REFRESH_GANTT_FINISHED, []);
                    reject();
                });
        });
    },

    [SET_SPRINT_DATES]: ({ commit, state }, payload ) => {
        return new Promise((resolve, reject) => {
            let newSummary = {
                ...state.sprintSummary
            }

            newSummary[payload.category + 'BeginDate'] = moment(payload.beginDate).format('D/MMM/YYYY');
            newSummary[payload.category + 'EndDate'] = moment(payload.endDate).format('D/MMM/YYYY');

            commit(SET_SPRINT_SUMMARY, newSummary);
            commit(SET_SPRINT_VELOCITY);

            resolve();
        });
    }
}

const mutations = {
    [RECENTLY_UPDATED_STARTED]: (state) => {
        state.recentlyUpdated.inProgress = true;
        state.recentlyUpdated.issuesCount = 0;
    },

    [RECENTLY_UPDATED_FINISHED]: (state, count) => {
        state.recentlyUpdated.inProgress = false;
        state.recentlyUpdated.issuesCount = count;
    },

    [RELOAD_ISSUES]: (state) => {
        state.reloadIssuesCount += 1;
    },

    [REFRESH_ISSUES]: (state, issues) => {
        state.unfilteredIssues = issues;
    },

    [REFRESH_GANTT]: (state, issues) => {
        console.info('Mitation: REFRESH_GANTT');
    },

    [REFRESH_GANTT_STARTED]: (state, issues) => {
        state.calendar.inProgress = true;
    },

    [REFRESH_GANTT_FINISHED]: (state, holidays) => {
        state.calendar.inProgress = false;
        state.calendar.holidays = holidays;
    },

    [SET_SPRINT_SUMMARY]: (state, newSprintinfo) => {
        let summary = {
            ...state.sprintSummary 
        };

        summary.boardId = newSprintinfo.boardId;
        summary.boardName = newSprintinfo.boardName;

        summary.sprintId = newSprintinfo.sprintId;
        summary.sprintName = newSprintinfo.sprintName;
        
        summary.developBeginDate = newSprintinfo.developBeginDate;
        summary.developEndDate = newSprintinfo.developEndDate;
        
        summary.testingBeginDate = newSprintinfo.testingBeginDate;
        summary.testingEndDate = newSprintinfo.testingEndDate;
        
        summary.regressionBeginDate = newSprintinfo.regressionBeginDate;
        summary.regressionEndDate = newSprintinfo.regressionEndDate;

        state.sprintSummary = summary;
    },

    [SET_SPRINT_VELOCITY]: (state) => {
        state.sprintSummary.developWorkDays = calendarService.getWorkDays(
            moment(state.sprintSummary.developBeginDate, 'D/MMM/YYYY').toDate(),
            moment(state.sprintSummary.developEndDate, 'D/MMM/YYYY').toDate());

        state.sprintSummary.testingWorkDays = calendarService.getWorkDays(
            moment(state.sprintSummary.testingBeginDate, 'D/MMM/YYYY').toDate(),
            moment(state.sprintSummary.testingEndDate, 'D/MMM/YYYY').toDate());

        state.sprintSummary.regressionWorkDays = calendarService.getWorkDays(
            moment(state.sprintSummary.regressionBeginDate, 'D/MMM/YYYY').toDate(),
            moment(state.sprintSummary.regressionEndDate, 'D/MMM/YYYY').toDate());

        state.sprintSummary.developVelocity = calendarService.getVelocity(
            moment(state.sprintSummary.developBeginDate, 'D/MMM/YYYY').toDate(),
            moment(state.sprintSummary.developEndDate, 'D/MMM/YYYY').toDate(),
            state.sprintSummary.developSingleVelocity);

        state.sprintSummary.testingVelocity = calendarService.getVelocity(
            moment(state.sprintSummary.testingBeginDate, 'D/MMM/YYYY').toDate(),
            moment(state.sprintSummary.testingEndDate, 'D/MMM/YYYY').toDate(),
            state.sprintSummary.testingSingleVelocity);
    },

    [SET_SPRINT_ISSUES]: (state, issues) => {
        state.unfilteredIssues = issues;
    },

    [SET_ISSUES_SUMMARIES]: (state, issuesSummaries) => {
        state.issuesSummaries = issuesSummaries.map((value) => {
            return {
                checked: false,
                ...value
            };
        });

        state.filterByPhase = _.orderBy(state.issuesSummaries, ['group'])
            .map((obj) => {
                return {
                    group: obj.group,
                    title: obj.title,
                    indicatorValue: obj.indicatorValue,
                    checked: obj.checked
                }
            });

        state.lastUpdated = new Date();
    }
}

export default {
    $http,
    state,
    getters,
    actions,
    mutations
}