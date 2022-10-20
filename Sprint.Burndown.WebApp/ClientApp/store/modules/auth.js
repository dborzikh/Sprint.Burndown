/* eslint-disable promise/param-names */
import axios from 'axios'
import { AUTH_REQUEST, AUTH_ERROR, AUTH_SUCCESS, AUTH_LOGOUT } from '../actions/auth'

const state = {
    token: localStorage.getItem('user-token') || '',
    status: '',
    hasLoadedOnce: false
}

const getters = {
    isAuthenticated: (state) => !!state.token,
    hasAuthenticationErrors: (state) => state.hasLoadedOnce && state.status === 'error',
    authenticationInProgress: (state) => state.status === 'loading',
    authorizationToken: (state) => state.token
}

const actions = {
    [AUTH_REQUEST]: ({ commit, dispatch }, user) => {
        return new Promise((resolve, reject) => {
            commit(AUTH_REQUEST);
            axios
                .post('/api/token', user)
                .then(response => {
                    const userToken = response.data;
                    if (userToken === undefined || response.status !== 200) {
                        commit(AUTH_ERROR, 'Unknown authentication error');
                        localStorage.removeItem('user-token');
                        delete axios.defaults.headers.common['Authorization'];
                        reject('Unknown authentication error');
                        return;
                    }

                    commit(AUTH_SUCCESS, userToken);
                    localStorage.setItem('user-token', userToken);
                    axios.defaults.headers.common['Authorization'] = 'Bearer ' + userToken;
                    resolve(userToken);
                })
                .catch(error => {
                    commit(AUTH_ERROR, error);
                    localStorage.removeItem('user-token');
                    delete axios.defaults.headers.common['Authorization'];
                    reject(error);
                });
        });
    },
    [AUTH_LOGOUT]: ({ commit, dispatch }) => {
        return new Promise((resolve, reject) => {
            commit(AUTH_LOGOUT);
            localStorage.removeItem('user-token');
            delete axios.defaults.headers.common['Authorization'];
            resolve();
        });
    }
}

const mutations = {
    [AUTH_REQUEST]: (state) => {
        console.warn('AUTH_REQUEST committed');
        state.status = 'loading';
    },
    [AUTH_SUCCESS]: (state, token) => {
        console.warn('AUTH_SUCCESS committed');
        state.status = 'success';
        state.token = token;
        state.hasLoadedOnce = true;
    },
    [AUTH_ERROR]: (state) => {
        console.warn('AUTH_ERROR committed');
        state.status = 'error';
        state.token = '';
        state.hasLoadedOnce = true;
    },
    [AUTH_LOGOUT]: (state) => {
        console.warn('AUTH_LOGOUT committed');
        state.status = '',
        state.token = '';
        state.hasLoadedOnce = false;
    }
}

export default {
    state,
    getters,
    actions,
    mutations
}