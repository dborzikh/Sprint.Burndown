/* eslint-disable promise/param-names */
import axios from 'axios'
import moment from 'moment'
import { Promise } from 'core-js';

import {
    CLEAR_FILTERS,
    SEARCH_TEXT_CHANGED, 
    SET_COLLAPSING_SUBTASKS, SET_COLLAPSING_BUGS, FILTER_STARRED_ISSUES,
    ADD_TASK_STATUS_FILTERS, REMOVE_TASK_STATUS_FILTERS
} from '../actions/pageFilters'

import calendarService from '../../calendarService'

const $http = axios;

const state = {
    searchText: '',
    filterByStatus: [],
    filterByPhase: [],
    collapsedSubtasks: false,
    collapsedBugs: false,
    displayStarred: false,

    hasAnyFilter() {
        return (state.searchText !== '')
            || (state.filterByStatus.length > 0)
            || state.collapsedSubtasks
            || state.collapsedBugs
            || state.displayStarred;
    }
}

const getters = {
    hasNoFilters: (state) => !state.hasAnyFilter(),
    hasFilters: (state) => state.hasAnyFilter(),

    searchText: (state) => state.searchText,
    
    collapsedSubtasks: (state) => state.collapsedSubtasks,
    collapsedBugs: (state) => state.collapsedBugs,
    displayStarred: (state) => state.displayStarred,
    currentFilterByStatus: (state) => state.filterByStatus,
    currentFilterByPhase: (state) => state.filterByPhase
}

const actions = {
    searchIssues: ({ commit, dispatch }, searchText) => {
        commit(SEARCH_TEXT_CHANGED, searchText);
    },
}

const mutations = {
    [CLEAR_FILTERS]: (state) => {
        state.searchText = '';
        state.filterByStatus = [];
        state.collapsedSubtasks = false;
        state.collapsedBugs = false;
        state.displayStarred = false;
        state.filterByPhase = _.forEach(state.filterByPhase, (v) => { v.checked = false; });
    },

    [SEARCH_TEXT_CHANGED]: (state, text) => {
        state.searchText = text;
    },

    [SET_COLLAPSING_SUBTASKS]: (state, newState) => {
        state.collapsedSubtasks = !state.collapsedSubtasks;
    },

    [SET_COLLAPSING_BUGS]: (state, newState) => {
        state.collapsedBugs = !state.collapsedBugs;
    },

    [FILTER_STARRED_ISSUES]: (state, newState) => {
        state.displayStarred = !state.displayStarred;
    },

    [ADD_TASK_STATUS_FILTERS]: (state, group) => {
        const taskStatusItem = _.find(state.issuesSummaries, (o) => o.group === group);
        if (taskStatusItem == undefined) {
            console.warn('Incorrect group on [ADD_TASK_STATUS_FILTERS]:', group);
            return;
        }

        let newFilters = state.filterByStatus.map((s) => { return s; });
        for (let serieItem of taskStatusItem.serieValues) {
            const statusName = serieItem.name.toUpperCase();
            if (!newFilters.includes(statusName)) {
                newFilters.push(statusName);
            }
        }

        taskStatusItem.checked = true;
        state.filterByStatus = newFilters;

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
    },

    [REMOVE_TASK_STATUS_FILTERS]: (state, group) => {
        const taskStatusItem = _.find(state.issuesSummaries, (o) => o.group === group);
        if (taskStatusItem == undefined) {
            console.warn('Incorrect group on [REMOVE_TASK_STATUS_FILTERS]:', group);
            return;
        }

        let newFilters = state.filterByStatus.map((s) => { return s; });
        for (let serieItem of taskStatusItem.serieValues) {
            const index = newFilters.indexOf(serieItem.name.toUpperCase());

            if (index > -1) {
                newFilters.splice(index, 1);
            }
        }

        taskStatusItem.checked = false;
        state.filterByStatus = newFilters;

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