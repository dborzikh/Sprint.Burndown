import Vue from 'vue'
import axios from 'axios'
import { AUTH_LOGOUT } from '../actions/auth'
import { USER_REQUEST, USER_ERROR, USER_SUCCESS } from '../actions/user'

const $http = axios;

const state = {
    status: '',
    profile: {}
}

const getters = {
    getProfile: state => state.profile,
    isProfileLoaded: state => !!state.profile.name,
}

const actions = {
    [USER_REQUEST]: ({ commit, dispatch }) => {
        commit(USER_REQUEST);

        $http({ url: 'user/me' })
            .then(response => {
                commit(USER_SUCCESS, response);
            })
            .catch(error => {
                commit(USER_ERROR);
                // if resp is unauthorized, logout, to
                dispatch(AUTH_LOGOUT);
            });
    },
}

const mutations = {
    [USER_REQUEST]: (state) => {
        state.status = 'loading';
    },
    [USER_SUCCESS]: (state, resp) => {
        state.status = 'success';
        Vue.set(state, 'profile', resp);
    },
    [USER_ERROR]: (state) => {
        state.status = 'error';
    },
    [AUTH_LOGOUT]: (state) => {
        state.profile = {}
    }
}

export default {
    $http,
    state,
    getters,
    actions,
    mutations
}