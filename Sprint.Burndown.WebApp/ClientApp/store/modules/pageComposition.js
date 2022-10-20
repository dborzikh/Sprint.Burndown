/* eslint-disable promise/param-names */
import axios from 'axios'
import { Promise } from 'core-js';

import {
    CHANGE_VISIBILITY_SPRINT_TIMELINE, CHANGE_VISIBILITY_TASK_STATUS, CHANGE_VISIBILITY_ISSUES, CHANGE_VISIBILITY_BURNDOWN,
    DISPLAY_INDICATORS, DISPLAY_ESTIMATES, DISPLAY_FEATURES, DISPLAY_VERSIONS, DISPLAY_REPORTERS
} from '../actions/pageComposition'

const $http = axios;

const issuesViewType = {
    indicators: 0,
    estimates: 1,
    features: 2,
    versions: 3,
    reporters: 4
}

const pageSubView = {
    sprintTimeline: 0,
    taskStatusSummary: 1,
    issuesGrid: 2,
    burnDownCharts: 3
}

const state = {
    issuesView: 0,
    pageSubViews: [0, 2, 3] 
}

const getters = {
    hasSprintTimeline: (state) => state.pageSubViews.indexOf(pageSubView.sprintTimeline) !== -1,
    hasTaskStatusSummary: (state) => state.pageSubViews.indexOf(pageSubView.taskStatusSummary) !== -1,
    hasIssuesGrid: (state) => state.pageSubViews.indexOf(pageSubView.issuesGrid) !== -1,
    hasBurnDownCharts: (state) => state.pageSubViews.indexOf(pageSubView.burnDownCharts) !== -1,

    currentIssuesView: (state) => state.issuesView,
    hasIndicators: (state) => state.issuesView === issuesViewType.indicators,
    hasEstimates: (state) => state.issuesView === issuesViewType.estimates,
    hasFeatures: (state) => state.issuesView === issuesViewType.features,
    hasVersions: (state) => state.issuesView === issuesViewType.versions,
    hasReporters: (state) => state.issuesView === issuesViewType.reporters,
}

function switchArrayElement(array, element) {
    const index = array.indexOf(element);
    if (index === -1) {
        array.push(element);
    } else {
        array.splice(index, 1);
    }

    return array;
}

const actions = {
    [DISPLAY_INDICATORS]: ({ commit, dispatch }, user) => {
        return new Promise((resolve, reject) => {
            commit(DISPLAY_INDICATORS);

            // TODO do something

            resolve();
        });
    },

    [DISPLAY_ESTIMATES]: ({ commit, dispatch }) => {
        return new Promise((resolve, reject) => {
            commit(DISPLAY_ESTIMATES);

            // TODO do something

            resolve();
        });
    }
}

const mutations = {
    [DISPLAY_INDICATORS]: (state) => {
        state.issuesView = issuesViewType.indicators;
    },

    [DISPLAY_ESTIMATES]: (state) => {
        state.issuesView = issuesViewType.estimates;
    },

    [DISPLAY_FEATURES]: (state) => {
        state.issuesView = issuesViewType.features;
    },

    [DISPLAY_VERSIONS]: (state) => {
        state.issuesView = issuesViewType.versions;
    },

    [DISPLAY_REPORTERS]: (state) => {
        state.issuesView = issuesViewType.reporters;
    },

    [CHANGE_VISIBILITY_SPRINT_TIMELINE]: (state, skipUpdate) => {
        state.pageSubViews = switchArrayElement(state.pageSubViews, pageSubView.sprintTimeline);

        if (!!skipUpdate) {
            $http.post('/api/board/savePreferences', { preferredSubViews: state.pageSubViews });
        }
    },

    [CHANGE_VISIBILITY_TASK_STATUS]: (state, skipUpdate) => {
        state.pageSubViews = switchArrayElement(state.pageSubViews, pageSubView.taskStatusSummary);

        if (!skipUpdate) {
            $http.post('/api/board/savePreferences', { preferredSubViews: state.pageSubViews });
        }
    },

    [CHANGE_VISIBILITY_ISSUES]: (state, skipUpdate) => {
        state.pageSubViews = switchArrayElement(state.pageSubViews, pageSubView.issuesGrid);

        if (!skipUpdate) {
            $http.post('/api/board/savePreferences', { preferredSubViews: state.pageSubViews });
        }
    },

    [CHANGE_VISIBILITY_BURNDOWN]: (state, skipUpdate) => {
        state.pageSubViews = switchArrayElement(state.pageSubViews, pageSubView.burnDownCharts);
        if (!skipUpdate) {
            $http.post('/api/board/savePreferences', { preferredSubViews: state.pageSubViews });
        }
    }
}

export default {
    $http,
    state,
    getters,
    actions,
    mutations
}